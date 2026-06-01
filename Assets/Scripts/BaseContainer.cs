using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// ============= БАЗОВЫЙ КЛАСС КОНТЕЙНЕРА =============
[Serializable]
public abstract class BaseContainer : MonoBehaviour
{
    [Header("Container Settings")]
    public DataBase data;
    public int maxCount = 20;
    public GameObject slotPrefab;      // Префаб слота
    public Transform slotsParent;      // Родительский объект для слотов

    [Header("UI References")]
    public RectTransform draggingObject;
    public Vector3 dragOffset;
    public EventSystem eventSystem;
    public GameObject backgroundPanel;

    protected List<ContainerSlot> slots = new List<ContainerSlot>();
    protected int currentDragSlotID = -1;
    protected ContainerSlot currentDragItem;
    protected bool isDirty = true;

    // События
    public System.Action<int, ContainerSlot> OnSlotChanged;
    public System.Action<int, ContainerSlot> OnSlotClicked;
    public System.Action OnContainerUpdated;

    protected virtual void Start()
    {
        if (slots.Count == 0)
            CreateSlots();

        LoadContainer();
    }

    protected virtual void Update()
    {
        HandleDrag();
        HandleContainerToggle();
        HandleDebugKeys();

        if (backgroundPanel != null && backgroundPanel.activeSelf && isDirty)
        {
            UpdateContainerUI();
            isDirty = false;
        }
    }

    // Создание слотов
    protected virtual void CreateSlots()
    {
        if (slotPrefab == null || slotsParent == null)
        {
            Debug.LogError("SlotPrefab или SlotsParent не назначены!");
            return;
        }

        for (int i = 0; i < maxCount; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotsParent);
            newSlot.name = i.ToString();

            ContainerSlot slot = new ContainerSlot
            {
                slotGameObject = newSlot,
                itemID = 0,
                count = 0
            };

            Button btn = newSlot.GetComponent<Button>();
            if (btn != null)
            {
                int slotIndex = i;
                btn.onClick.AddListener(() => OnSlotSelected(slotIndex));
            }

            slots.Add(slot);
        }
    }

    // Обработка перетаскивания
    protected virtual void HandleDrag()
    {
        if (currentDragSlotID != -1 && draggingObject != null)
        {
            Vector3 pos = Input.mousePosition + dragOffset;
            if (Camera.main != null)
            {
                pos.z = draggingObject.parent?.GetComponent<RectTransform>()?.position.z ?? 0;
                draggingObject.position = Camera.main.ScreenToWorldPoint(pos);
            }
        }
    }

    // Открытие/закрытие контейнера
    protected virtual void HandleContainerToggle()
    {
        if (backgroundPanel != null && Input.GetKeyDown(KeyCode.I))
        {
            backgroundPanel.SetActive(!backgroundPanel.activeSelf);
            if (backgroundPanel.activeSelf)
            {
                UpdateContainerUI();
                isDirty = false;
            }
        }
    }

    // Отладка
    protected virtual void HandleDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveContainer();
            Debug.Log($"{GetContainerName()} сохранен (F5)");
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadContainer();
            Debug.Log($"{GetContainerName()} загружен (F9)");
        }
    }

    // Выбор слота (виртуальный для переопределения)
    protected virtual void OnSlotSelected(int slotID)
    {
        OnSlotClicked?.Invoke(slotID, slots[slotID]);

        if (slots[slotID].itemID == 0 && currentDragSlotID == -1)
            return;

        if (currentDragSlotID == -1)
        {
            StartDrag(slotID);
        }
        else
        {
            EndDrag(slotID);
        }
    }

    // Начать перетаскивание
    protected virtual void StartDrag(int slotID)
    {
        currentDragSlotID = slotID;
        currentDragItem = CopySlot(slots[currentDragSlotID]);

        if (draggingObject != null)
        {
            draggingObject.gameObject.SetActive(true);
            Image dragImage = draggingObject.GetComponent<Image>();
            if (dragImage != null && GetItemData(currentDragItem.itemID) != null)
                dragImage.sprite = GetItemData(currentDragItem.itemID).image;
        }

        // Очищаем слот
        SetSlotItem(currentDragSlotID, 0, 0);
    }

    // Завершить перетаскивание
    protected virtual void EndDrag(int targetSlotID)
    {
        ContainerSlot targetSlot = slots[targetSlotID];

        if (currentDragItem.itemID != targetSlot.itemID)
        {
            // Меняем местами
            ContainerSlot tempSlot = CopySlot(targetSlot);
            SetSlotItem(targetSlotID, currentDragItem.itemID, currentDragItem.count);
            SetSlotItem(currentDragSlotID, tempSlot.itemID, tempSlot.count);
        }
        else
        {
            // Объединяем стеки
            ItemData itemData = GetItemData(currentDragItem.itemID);
            if (itemData != null && targetSlot.count + currentDragItem.count <= itemData.stack)
            {
                targetSlot.count += currentDragItem.count;
                SetSlotItem(targetSlotID, targetSlot.itemID, targetSlot.count);
                SetSlotItem(currentDragSlotID, 0, 0);
            }
            else if (itemData != null)
            {
                int canAdd = itemData.stack - targetSlot.count;
                targetSlot.count = itemData.stack;
                currentDragItem.count -= canAdd;
                SetSlotItem(targetSlotID, targetSlot.itemID, targetSlot.count);
                SetSlotItem(currentDragSlotID, currentDragItem.itemID, currentDragItem.count);
            }
        }

        currentDragSlotID = -1;
        currentDragItem = null;

        if (draggingObject != null)
            draggingObject.gameObject.SetActive(false);

        MarkDirty();
    }

    // Обновление UI
    protected virtual void UpdateContainerUI()
    {
        if (data == null || data.item == null)
        {
            Debug.LogError("База данных не инициализирована!");
            return;
        }

        for (int i = 0; i < maxCount && i < slots.Count; i++)
        {
            if (slots[i] == null) continue;

            // Обновляем текст
            Text countText = slots[i].slotGameObject?.GetComponentInChildren<Text>();
            if (countText != null)
            {
                if (slots[i].itemID != 0 && slots[i].count > 1)
                    countText.text = slots[i].count.ToString();
                else
                    countText.text = "";
            }

            // Обновляем иконку
            Image icon = slots[i].slotGameObject?.GetComponent<Image>();
            if (icon != null)
            {
                ItemData itemData = GetItemData(slots[i].itemID);
                icon.sprite = itemData?.image;
            }
        }

        OnContainerUpdated?.Invoke();
    }

    // Работа с предметами
    public virtual bool AddItem(int itemID, int count)
    {
        ItemData itemToAdd = GetItemData(itemID);
        if (itemToAdd == null) return false;

        int remainingCount = count;

        // Сначала пытаемся добавить в существующие стеки
        for (int i = 0; i < maxCount; i++)
        {
            if (slots[i].itemID == itemID && slots[i].count < itemToAdd.stack)
            {
                int spaceLeft = itemToAdd.stack - slots[i].count;
                int toAdd = Mathf.Min(remainingCount, spaceLeft);
                slots[i].count += toAdd;
                remainingCount -= toAdd;
                MarkDirty();

                if (remainingCount <= 0)
                    return true;
            }
        }

        // Затем в пустые слоты
        for (int i = 0; i < maxCount; i++)
        {
            if (slots[i].itemID == 0)
            {
                int toAdd = Mathf.Min(remainingCount, itemToAdd.stack);
                SetSlotItem(i, itemID, toAdd);
                remainingCount -= toAdd;
                MarkDirty();

                if (remainingCount <= 0)
                    return true;
            }
        }

        return false;
    }

    public virtual bool RemoveItem(int slotID, int count)
    {
        if (slotID < 0 || slotID >= maxCount) return false;
        if (slots[slotID].count < count) return false;

        slots[slotID].count -= count;
        if (slots[slotID].count <= 0)
            SetSlotItem(slotID, 0, 0);

        MarkDirty();
        return true;
    }

    public virtual bool TransferItem(BaseContainer targetContainer, int sourceSlotID)
    {
        if (sourceSlotID < 0 || sourceSlotID >= maxCount) return false;
        if (slots[sourceSlotID].itemID == 0) return false;

        ContainerSlot sourceSlot = slots[sourceSlotID];
        bool success = targetContainer.AddItem(sourceSlot.itemID, sourceSlot.count);

        if (success)
            SetSlotItem(sourceSlotID, 0, 0);

        return success;
    }

    // Вспомогательные методы
    protected virtual void SetSlotItem(int slotID, int itemID, int count)
    {
        if (slotID < 0 || slotID >= maxCount) return;

        slots[slotID].itemID = itemID;
        slots[slotID].count = count;

        // Обновляем UI
        if (slots[slotID].slotGameObject != null)
        {
            Image icon = slots[slotID].slotGameObject.GetComponent<Image>();
            if (icon != null)
                icon.sprite = GetItemData(itemID)?.image;

            Text text = slots[slotID].slotGameObject.GetComponentInChildren<Text>();
            if (text != null)
                text.text = (itemID != 0 && count > 1) ? count.ToString() : "";
        }

        OnSlotChanged?.Invoke(slotID, slots[slotID]);
        MarkDirty();
    }

    protected virtual ContainerSlot CopySlot(ContainerSlot slot)
    {
        return new ContainerSlot
        {
            itemID = slot.itemID,
            count = slot.count,
            slotGameObject = slot.slotGameObject
        };
    }

    protected virtual ItemData GetItemData(int id)
    {
        if (data == null || id <= 0 || id >= data.item.Count)
            return null;

        return data.item[id];
    }

    protected virtual void MarkDirty()
    {
        isDirty = true;
    }

    protected abstract string GetContainerName();
    protected abstract void SaveContainer();
    protected abstract void LoadContainer();

    // Очистка контейнера
    public virtual void ClearContainer()
    {
        for (int i = 0; i < maxCount; i++)
        {
            SetSlotItem(i, 0, 0);
        }
        MarkDirty();
    }

    // Получение количества предметов
    public virtual int GetItemCount(int itemID)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (slot.itemID == itemID)
                total += slot.count;
        }
        return total;
    }

    // Проверка наличия места
    public virtual bool HasFreeSlot()
    {
        foreach (var slot in slots)
        {
            if (slot.itemID == 0)
                return true;
        }
        return false;
    }
}

// ============= СТРУКТУРА СЛОТА =============
[Serializable]
public class ContainerSlot
{
    public int itemID;
    public int count;
    public GameObject slotGameObject;

    public bool IsEmpty() => itemID == 0 || count == 0;
    public bool IsFull(ItemData itemData) => itemData != null && count >= itemData.stack;
}

// ============= ДАННЫЕ ДЛЯ СОХРАНЕНИЯ =============
[Serializable]
public class ContainerSaveData
{
    public List<int> itemIDs = new List<int>();
    public List<int> counts = new List<int>();

    public ContainerSaveData(int capacity)
    {
        for (int i = 0; i < capacity; i++)
        {
            itemIDs.Add(0);
            counts.Add(0);
        }
    }
}