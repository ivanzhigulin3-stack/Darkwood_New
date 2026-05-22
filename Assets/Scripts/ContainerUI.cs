using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ContainerUI : MonoBehaviour
{
    [Header("UI элементы")]
    public GameObject slotPrefab;
    public Transform containerSlotsParent;
    public Transform inventorySlotsParent;
    public Button closeButton;
    public Text containerTitleText;

    private Container currentContainer;
    private Inventory playerInventory;
    private List<GameObject> containerSlots = new List<GameObject>();
    private List<GameObject> inventorySlots = new List<GameObject>();

    private int selectedContainerSlot = -1;
    private int selectedInventorySlot = -1;

    private void Start()
    {
        if (closeButton != null) closeButton.onClick.AddListener(OnCloseButtonClick);

        gameObject.SetActive(false);
    }

    public void Initialize(Container container)
    {
        currentContainer = container;
    }

    public void ShowContainer(Container container, Inventory inventory)
    {
        currentContainer = container;
        playerInventory = inventory;

        if (containerTitleText != null) containerTitleText.text = container.containerName;

        ClearSlots();
        CreateSlots();
        UpdateUI();

        gameObject.SetActive(true);
    }

    private void CreateSlots()
    {
        for (int i = 0; i < currentContainer.containerSize; i++)
        {
            GameObject slot = Instantiate(slotPrefab, containerSlotsParent);
            containerSlots.Add(slot);
            int slotIndex = i;

            Button button = slot.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnContainerSlotClick(slotIndex));
            }
            AddRightClickHandler(slot, () => TransferFromContainerToInventory(slotIndex));
        }

        for (int i = 0; i < playerInventory.maxCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventorySlotsParent);
            inventorySlots.Add(slot);
            int slotIndex = i;

            Button button = slot.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnInventorySlotClick(slotIndex));
            }
            AddRightClickHandler(slot, () => TransferFromInventoryToContainer(slotIndex));
        }
    }
    private void AddRightClickHandler(GameObject slot, System.Action action)
    {
        var trigger = slot.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
            trigger = slot.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => {
            var pointerData = (UnityEngine.EventSystems.PointerEventData)data;
            if (pointerData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Right)
            {
                action();
            }
        });
        trigger.triggers.Add(entry);
    }

    private void OnContainerSlotClick(int slotIndex)
    {
        if (selectedInventorySlot != -1)
        {
            TransferFromInventoryToContainer(selectedInventorySlot);
            selectedInventorySlot = -1;
        }
        else
        {
            ClearSelection();
            selectedContainerSlot = slotIndex;
            HighlightSlot(containerSlots[slotIndex], true);
        }
    }

    private void OnInventorySlotClick(int slotIndex)
    {
        if (selectedContainerSlot != -1)
        {
            TransferFromContainerToInventory(selectedContainerSlot);
            selectedContainerSlot = -1;
        }
        else
        {
            ClearSelection();
            selectedInventorySlot = slotIndex;
            HighlightSlot(inventorySlots[slotIndex], true);
        }
    }

    private void TransferFromContainerToInventory(int containerSlot)
    {
        if (currentContainer == null || playerInventory == null) return;

        ContainerSlot item = currentContainer.GetItem(containerSlot);
        if (item == null || item.id == 0) return;

        // Перемещаем весь стек
        bool success = playerInventory.AddItemByID(item.id, item.amount);

        if (success)
        {
            currentContainer.RemoveItem(containerSlot, item.amount);
            UpdateUI();
        }
        else
        {
            Debug.Log("Недостаточно места в инвентаре!");
        }

        ClearSelection();
    }

    private void TransferFromInventoryToContainer(int inventorySlot)
    {
        if (currentContainer == null || playerInventory == null) return;

        InventoryItem item = playerInventory.item[inventorySlot];
        if (item.id == 0) return;

        // Перемещаем весь стек
        bool success = currentContainer.AddItem(item.id, item.count);

        if (success)
        {
            playerInventory.AddItem(inventorySlot, playerInventory.data.item[0], 0);
            playerInventory.UpdateInventory();
            UpdateUI();
        }
        else
        {
            Debug.Log("Недостаточно места в контейнере!");
        }

        ClearSelection();
    }

    public void UpdateUI()
    {
        // Обновляем слоты контейнера
        DataBase dataBase = FindObjectOfType<DataBase>();
        if (dataBase == null) return;

        for (int i = 0; i < containerSlots.Count && i < currentContainer.containerItem.Count; i++)
        {
            UpdateSlot(containerSlots[i], currentContainer.containerItem[i].id, currentContainer.containerItem[i].amount, dataBase);
        }

        // Обновляем слоты инвентаря
        for (int i = 0; i < inventorySlots.Count && i < playerInventory.item.Count; i++)
        {
            UpdateSlot(inventorySlots[i], playerInventory.item[i].id, playerInventory.item[i].count, dataBase);
        }
    }

    private void UpdateSlot(GameObject slot, int itemID, int amount, DataBase dataBase)
    {
        Image icon = slot.transform.Find("Icon")?.GetComponent<Image>();
        Text countText = slot.transform.Find("Count")?.GetComponent<Text>();

        if (icon != null)
        {
            if (itemID != 0 && itemID < dataBase.item.Count)
            {
                icon.sprite = dataBase.item[itemID].image;
                icon.color = Color.white;
                icon.enabled = true;

                if (countText != null)
                {
                    if (amount > 1)
                    {
                        countText.text = amount.ToString();
                        countText.enabled = true;
                    }
                    else
                    {
                        countText.enabled = false;
                    }
                }
            }
            else
            {
                icon.sprite = null;
                icon.enabled = false;
                if (countText != null) countText.enabled = false;
            }
        }
    }

    private void HighlightSlot(GameObject slot, bool highlight)
    {
        Image border = slot.GetComponent<Image>();
        if (border != null)
        {
            border.color = highlight ? Color.yellow : Color.white;
        }
    }

    private void ClearSelection()
    {
        selectedContainerSlot = -1;
        selectedInventorySlot = -1;

        foreach (var slot in containerSlots)
        {
            HighlightSlot(slot, false);
        }
        foreach (var slot in inventorySlots)
        {
            HighlightSlot(slot, false);
        }
    }
    private void ClearSlots()
    {
        foreach (var slot in containerSlots) Destroy(slot);
        foreach (var slot in inventorySlots) Destroy(slot);

        containerSlots.Clear();
        inventorySlots.Clear();
    }
    private void OnCloseButtonClick()
    {
        if (currentContainer != null && currentContainer.IsOpen())
        {
            currentContainer.Close(); 
        }
        CloseContainer();
    }
    public void CloseContainer()
    {
        ClearSlots();
        gameObject.SetActive(false);
    }
}
