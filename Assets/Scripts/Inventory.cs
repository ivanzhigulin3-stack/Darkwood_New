using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public DataBase data;
    
    public List<InventoryItem> item = new List<InventoryItem>();

    public GameObject gameObjShow;

    public GameObject inventoryMainObject;
    public int maxCount;

    public EventSystem es;

    public int currentID = -1;
    public InventoryItem currentItem;

    public RectTransform movingObject;
    public Vector3 offset;

    public GameObject backGround;

    public void Start()
    {
        if (item.Count == 0)
        {
            AddGraphics();
        }

        for (int i = 0; i < maxCount; i++)  // тест, заполнить Rand ячейки
        {
            AddItem(i, data.item[Random.Range(0, data.item.Count)], Random.Range(1,5));
        }
        UpdateInventory();
    }
    public void Update()
    {
        if (currentID != -1)
        {
            MoveObject();
        }
        else
        {
            UpdateInventory();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            backGround.SetActive(!backGround.activeSelf);
            if (backGround.activeSelf)
            {
                UpdateInventory();
            }
        }
    }

    /*
   public void SearchForSameItem(ItemData ItemData, int count)
    {
        for (int i = 0; i < maxCount; i++)
        {
            if (item[i].id == ItemData.id)
            {
                if (item[0].count < 99)
                {
                    item[i].count += count;
                    
                    if (item[i].count > 99)
                    {
                        count = item[i].count - 99;
                        item[i].count = 99;
                    }
                    else
                    {
                        count = 0;
                        i = maxCount;
                    }
                }
            }
        }

        if (count > 0)
        {
            for (int i = 0; i < maxCount; i++)
            {
                if (item[i].id == 0)
                {
                    AddItem(i, ItemData, count);
                    i = maxCount;
                }
            }
        }
    }
   */

    public void AddItem(int id, ItemData ItemData, int count)
    {
        item[id].id = ItemData.id;
        item[id].count = count;
        item[id].itemGameObj.GetComponent<Image>().sprite = ItemData.image;

        if (count > 1 && ItemData.id != 0)
        {
            item[id].itemGameObj.GetComponentInChildren<Text>().text = count.ToString();
        }
        else
        {
            item[id].itemGameObj.GetComponentInChildren<Text>().text = "";
        }
    }


    public void AddInventoryItem(int id, InventoryItem invItem )
    {
        item[id].id = invItem.id;
        item[id].count = invItem.count;
        item[id].itemGameObj.GetComponent<Image>().sprite = data.item[invItem.id].image;

        if (invItem.count > 1 && invItem.id != 0)
        {
            item[id].itemGameObj.GetComponentInChildren<Text>().text = invItem.count.ToString();
        }
        else
        {
            item[id].itemGameObj.GetComponentInChildren<Text>().text = "";
        }

        
    }

    public void AddGraphics()
    {
        for (int i = 0; i < maxCount; i++)
        {
            GameObject newItem = Instantiate(gameObjShow, inventoryMainObject.transform) as GameObject;

            newItem.name = i.ToString();

            InventoryItem ii = new InventoryItem();
            ii.itemGameObj = newItem;

            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, 0, 0);
            rt.localScale = new Vector3(1, 1, 1);
            newItem.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1);
            
            Button tempButton = newItem.GetComponent<Button>();

            tempButton.onClick.AddListener(delegate { SelectObject(); });

            item.Add(ii);
        }
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < maxCount; i++)
        {
            if (item[i].id != 0 && item[i].count > 1)
            {
                item[i].itemGameObj.GetComponentInChildren<Text>().text = item[i].count.ToString();
            }
            else
            {
                item[i].itemGameObj.GetComponentInChildren<Text>().text = "";
            }

            item[i].itemGameObj.GetComponent<Image>().sprite = data.item[item[i].id].image;
        }
    }
    public void SelectObject()
    {

        int selectedSlotID = int.Parse(es.currentSelectedGameObject.name);

        if (item[selectedSlotID].id == 0 && currentID == -1)
        {
            return;
        }

        if (currentID == -1)
        {
            currentID = int.Parse(es.currentSelectedGameObject.name);
            currentItem = CopyInventoryItem(item[currentID]);
            movingObject.gameObject.SetActive(true);
            movingObject.GetComponent<Image>().sprite = data.item[currentItem.id].image;

            AddItem(currentID, data.item[0], 0);
        }
        else
        {
            InventoryItem II = item[int.Parse(es.currentSelectedGameObject.name)];

            if (currentItem.id != II.id)
            {
                AddInventoryItem(currentID, II);
                AddInventoryItem(int.Parse(es.currentSelectedGameObject.name), currentItem);
            }
            else
            {
                if (II.count + currentItem.count <= data.item[II.id].stack)
                {
                    II.count += currentItem.count;
                }
                else
                {
                    AddItem(currentID, data.item[II.id], II.count + currentItem.count - data.item[II.id].stack);
                    II.count = data.item[II.id].stack;
                }

                II.itemGameObj.GetComponentInChildren<Text>().text = II.count.ToString();
            }
            currentID = -1;

            movingObject.gameObject.SetActive(false);
        }
    }

    public void MoveObject()
    {
        Vector3 pos = Input.mousePosition + offset;
        pos.z = inventoryMainObject.GetComponent<RectTransform>().position.z;
        movingObject.position = Camera.main.ScreenToWorldPoint(pos);
    }

    public InventoryItem CopyInventoryItem(InventoryItem old)
    {
        InventoryItem New = new InventoryItem();

        New.id = old.id;
        New.itemGameObj = old.itemGameObj;
        New.count = old.count;

        return New;
    }


    //Добавлен для поднятия предметов с пола
    public bool AddItemByID(int itemID, int count)
    {
        if (itemID == 0 || count <= 0) return false;
        
        ItemData itemToAdd = data.item[itemID];
        int remainingCount = count;

        for (int i = 0; i < maxCount; i++)
        {
            if (item[i].id == itemID)
            {
                int maxStack = itemToAdd.stack;
                int currentCount = item[i].count;

                if (currentCount < maxStack)
                {
                    int spaceLeft = maxStack - currentCount;
                    int toAdd = Mathf.Min(remainingCount, spaceLeft);

                    item[i].count += toAdd;
                    remainingCount -= toAdd;
                    UpdateInventory();

                    if (remainingCount <= 0) return true;
                }
            }
        }

        for (int i = 0; i < maxCount; i++)
        {
            if (item[i].id == 0)
            {
                int maxStack = itemToAdd.stack;
                int toAdd = Mathf.Min(remainingCount, maxStack);

                AddItem(i, itemToAdd, toAdd);
                remainingCount -= toAdd;

                if (remainingCount <= 0) return true;
            }
        }

        Debug.Log($"Недостаточно места для {itemToAdd.name} x{remainingCount}");
        //****
        return false;
    }

}

[System.Serializable]
public class InventoryItem
{
    public int id;
    public GameObject itemGameObj;

    public int count;
}