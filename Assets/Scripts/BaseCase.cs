using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public abstract class BaseCase : MonoBehaviour
{
    public DataBase data;

    public List<CaseItem> item = new List<CaseItem>();

    public GameObject gameObjShow;

    public GameObject caseMainObject;
    public int maxCount;

    public EventSystem es;

    public int currentID = -1;
    public CaseItem currentItem;

    public RectTransform movingObject;
    public Vector3 offset;

    public GameObject backGround;
    
    public bool caseUpdate = true;


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

        caseUpdate = true;
    }

    public void AddCaseItem(int id, CaseItem caseItem)
    {
        item[id].id = caseItem.id;
        item[id].count = caseItem.count;
        item[id].itemGameObj.GetComponent<Image>().sprite = data.item[caseItem.id].image;

        if (caseItem.count > 1 && caseItem.id != 0)
        {
            item[id].itemGameObj.GetComponentInChildren<Text>().text = caseItem.count.ToString();
        }
        else
        {
            item[id].itemGameObj.GetComponentInChildren<Text>().text = "";
        }

        caseUpdate = true;
    }

    public void AddGraphics()
    {
        for (int i = 0; i < maxCount; i++)
        {
            GameObject newItem = Instantiate(gameObjShow, caseMainObject.transform) as GameObject;

            newItem.name = i.ToString();

            CaseItem ci = new CaseItem();
            ci.itemGameObj = newItem;

            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, 0, 0);
            rt.localScale = new Vector3(1, 1, 1);
            newItem.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1);

            Button tempButton = newItem.GetComponent<Button>();

            tempButton.onClick.AddListener(delegate { SelectObject(); });

            item.Add(ci);
        }
    }

    public void CaseUpdate()
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
    /*
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
            currentItem = CopyCaseItem(item[currentID]);
            movingObject.gameObject.SetActive(true);
            movingObject.GetComponent<Image>().sprite = data.item[currentItem.id].image;

            AddItem(currentID, data.item[0], 0);
        }
        else
        {
            CaseItem ci = item[int.Parse(es.currentSelectedGameObject.name)];

            if (currentItem.id != ci.id)
            {
                AddCaseItem(currentID, ci);
                AddCaseItem(int.Parse(es.currentSelectedGameObject.name), currentItem);
            }
            else
            {
                if (ci.count + currentItem.count <= data.item[ci.id].stack)
                {
                    ci.count += currentItem.count;
                    caseUpdate = true;  
                }
                else
                {
                    AddItem(currentID, data.item[ci.id], ci.count + currentItem.count - data.item[ci.id].stack);
                    ci.count = data.item[ci.id].stack;
                }

                ci.itemGameObj.GetComponentInChildren<Text>().text = ci.count.ToString();
            }
            currentID = -1;
            movingObject.gameObject.SetActive(false);
        }
    }
    
    
    public void MoveObject()
    {
        Vector3 pos = Input.mousePosition + offset;
        pos.z = caseMainObject.GetComponent<RectTransform>().position.z;
        movingObject.position = Camera.main.ScreenToWorldPoint(pos);
    }
    */
    //new*****************************

    public CaseItem CopyCaseItem(CaseItem old)
    {
        CaseItem New = new CaseItem();

        New.id = old.id;
        New.itemGameObj = old.itemGameObj;
        New.count = old.count;

        return New;
    }

    public void SelectObject()
    {
        int selectedSlotID = int.Parse(es.currentSelectedGameObject.name);

        // Если ничего не выбрано и не перетаскивается
        if (currentID == -1 && !DragDropManager.Instance.IsDragging())
        {
            if (item[selectedSlotID].id == 0)
                return;

            // Начинаем перетаскивание через менеджер
            DragDropManager.Instance.StartDrag(this, selectedSlotID, item[selectedSlotID], movingObject.gameObject);
        }
        else if (DragDropManager.Instance.IsDragging())
        {
            // Сбрасываем на текущий контейнер
            DragDropManager.Instance.DropOn(this, selectedSlotID);
        }
    }

    public void MoveObject()
    {
        if (movingObject.gameObject.activeSelf)
        {
            Vector3 pos = Input.mousePosition + offset;
            movingObject.position = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10));
        }
    }

    // Обновляем Update в PlayerInventory и Container:
    public void HandleDragUpdate()
    {
        if (DragDropManager.Instance.IsDragging())
        {
            MoveObject();
        }
    }

    //******************************
}

[System.Serializable]
public class CaseItem
{
    public int id;
    public GameObject itemGameObj;

    public int count;
}