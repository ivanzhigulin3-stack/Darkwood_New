using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class InventorySaveData
{
    public List<InventorySlotData> slots = new List<InventorySlotData>();

    public InventorySaveData(int count)
    {
        for (int i = 0; i < count; i++)
        {
            slots.Add(new InventorySlotData());
        }
    }
}

[Serializable]
public class InventorySlotData
{
    public int id;
    public int count;

    public InventorySlotData()
    {
        id = 0;
        count = 0;
    }

    public InventorySlotData(int itemId, int itemCount)
    {
        id = itemId;
        count = itemCount;
    }
}
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

        LoadInventory();

        AddTestItems();
        /*
        for (int i = 0; i < maxCount; i++)  // тест, заполнить Rand ячейки
        {
            AddItem(i, data.item[Random.Range(0, data.item.Count)], Random.Range(1,5));
        }
        UpdateInventory();
        */
    }

    private bool isInventoryDirty = true;
    public void Update()
    {
        if (currentID != -1)
        {
            MoveObject();
        }
        

        if (Input.GetKeyDown(KeyCode.I))
        {
            backGround.SetActive(!backGround.activeSelf);
            if (backGround.activeSelf)
            {
                UpdateInventory();
                isInventoryDirty = false;
            }
        }

        if (backGround.activeSelf && isInventoryDirty)
        {
            UpdateInventory();
            isInventoryDirty = false;
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveInventory();
            Debug.Log("Инвентарь сохранен (F5)");
        }

        // Быстрая загрузка по F9
        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadInventory();
            Debug.Log("Инвентарь загружен (F9)");
        }
    }




    public void AddItem(int id, ItemData ItemData, int count)
    {
        // Проверяем границы массива
        if (id < 0 || id >= maxCount)
        {
            Debug.LogError($"Попытка добавить предмет в несуществующий слот {id}");
            return;
        }

        if (id >= item.Count)
        {
            Debug.LogError($"Слот {id} не существует в списке item!");
            return;
        }

        if (item[id] == null)
        {
            Debug.LogError($"item[{id}] равен null!");
            return;
        }

        if (ItemData == null)
        {
            Debug.LogError("ItemData равен null!");
            return;
        }

        // Проверяем, что UI элементы существуют
        Image image = item[id].itemGameObj.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError($"У item[{id}] нет компонента Image!");
            return;
        }

        Text text = item[id].itemGameObj.GetComponentInChildren<Text>();

        // Выполняем добавление
        item[id].id = ItemData.id;
        item[id].count = count;
        image.sprite = ItemData.image;

        if (count > 1 && ItemData.id != 0 && text != null)
        {
            text.text = count.ToString();
        }
        else if (text != null)
        {
            text.text = "";
        }

        isInventoryDirty = true;
    }
    /*
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

        isInventoryDirty = true;  
    }
    */

    public void AddInventoryItem(int id, InventoryItem invItem)
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

        isInventoryDirty = true;
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
        if (data == null || data.item == null)
        {
            Debug.LogError("База данных не инициализирована!");
            return;
        }

        for (int i = 0; i < maxCount; i++)
        {
            if (i >= item.Count)
            {
                Debug.LogError($"Индекс {i} выходит за границы списка item (размер: {item.Count})");
                break;
            }

            if (item[i] == null)
            {
                Debug.LogError($"item[{i}] равен null!");
                continue;
            }

            int itemId = item[i].id;

            // Проверяем, что ID корректен
            if (itemId < 0 || itemId >= data.item.Count)
            {
                Debug.LogWarning($"Некорректный ID предмета: {itemId} в слоте {i}");
                // Очищаем поврежденный слот
                item[i].id = 0;
                item[i].count = 0;
                itemId = 0;
            }

            // Обновляем текст
            Text countText = item[i].itemGameObj.GetComponentInChildren<Text>();
            if (countText != null)
            {
                if (item[i].id != 0 && item[i].count > 1)
                    countText.text = item[i].count.ToString();
                else
                    countText.text = "";
            }

            // Обновляем иконку
            Image icon = item[i].itemGameObj.GetComponent<Image>();
            if (icon != null && GetSafeItem(itemId) != null)
            {
                icon.sprite = GetSafeItem(itemId).image;
            }
        }
    }

    /*
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
    */
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
                    isInventoryDirty = true;  // Помечаем изменение
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
        ItemData itemToAdd = data.item[itemID];
        int remainingCount = count;

        for (int i = 0; i < maxCount; i++)
        {
            if (item[i].id == itemID && item[i].count < itemToAdd.stack)
            {
                int spaceLeft = itemToAdd.stack - item[i].count;
                int toAdd = Mathf.Min(remainingCount, spaceLeft);
                item[i].count += toAdd;
                remainingCount -= toAdd;
                isInventoryDirty = true;

                if (remainingCount <= 0)
                    return true;
            }
        }

        for (int i = 0; i < maxCount; i++)
        {
            if (item[i].id == 0)
            {
                int toAdd = Mathf.Min(remainingCount, itemToAdd.stack);
                AddItem(i, itemToAdd, toAdd);
                remainingCount -= toAdd;

                if (remainingCount <= 0)
                    return true;
            }
        }

        return false;
    }
    //проверка на ID
    private ItemData GetSafeItem(int id)
    {
        if (data == null)
        {
            Debug.LogError("DataBase не назначен в инспекторе!");
            return null;
        }

        if (id < 0 || id >= data.item.Count)
        {
            Debug.LogWarning($"Попытка получить предмет с ID {id}, но его нет в базе!");
            return data.item[0]; 
        }

        return data.item[id];
    }

    //new-------------------------------

    private const string SAVE_KEY = "InventoryData";  // Ключ для сохранения

    // Сохранение инвентаря
    public void SaveInventory()
    {
        try
        {
            InventorySaveData saveData = new InventorySaveData(maxCount);

            for (int i = 0; i < maxCount && i < item.Count; i++)
            {
                if (item[i] != null)
                {
                    saveData.slots[i].id = item[i].id;
                    saveData.slots[i].count = item[i].count;
                }
            }

            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();  // Немедленно сохраняем
            Debug.Log("Инвентарь сохранен!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка сохранения инвентаря: {e.Message}");
        }
    }

    // Загрузка инвентаря
    public void LoadInventory()
    {
        try
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

                if (saveData != null && saveData.slots.Count == maxCount)
                {
                    for (int i = 0; i < maxCount; i++)
                    {
                        if (saveData.slots[i].id != 0)
                        {
                            ItemData itemData = GetSafeItem(saveData.slots[i].id);
                            if (itemData != null)
                            {
                                AddItem(i, itemData, saveData.slots[i].count);
                            }
                        }
                        else
                        {
                            AddItem(i, data.item[0], 0);  // Пустой слот
                        }
                    }

                    Debug.Log("Инвентарь загружен!");
                    UpdateInventory();
                }
                else
                {
                    Debug.LogWarning("Сохраненные данные повреждены или несовместимы");
                }
            }
            else
            {
                Debug.Log("Нет сохраненного инвентаря, создаем новый");
                CreateEmptyInventory();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка загрузки инвентаря: {e.Message}");
            CreateEmptyInventory();
        }
    }

    // Создание пустого инвентаря
    private void CreateEmptyInventory()
    {
        for (int i = 0; i < maxCount; i++)
        {
            AddItem(i, data.item[0], 0);
        }
        UpdateInventory();
    }

    // Очистка всех сохранений (для тестирования)
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
        Debug.Log("Сохранение удалено");
        CreateEmptyInventory();
    }

    // Сохраняем при закрытии игры
    private void OnApplicationQuit()
    {
        SaveInventory();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveInventory();
        }
    }

    public void AddTestItems()
    {
        // Очищаем инвентарь
        for (int i = 0; i < maxCount; i++)
        {
            AddItem(i, data.item[0], 0);
        }

        // Добавляем тестовые предметы
        AddItemByID(1, 5); 
        AddItemByID(2, 10);  
        AddItemByID(3, 30);  
    }
}

[System.Serializable]
public class InventoryItem
{
    public int id;
    public GameObject itemGameObj;

    public int count;
}
//вызов из другого скрипта
/*
 // Найти инвентарь и сохранить
Inventory inv = FindObjectOfType<Inventory>();
if (inv != null)
    inv.SaveInventory();

// Загрузить инвентарь при старте уровня
inv.LoadInventory();
 */