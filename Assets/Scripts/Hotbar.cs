using UnityEngine;
using UnityEngine.UI;
public class Hotbar : BaseCase
{
    [Header("Hotbar Settings")]
    [SerializeField] private int activeSlotIndex = 0;
    [SerializeField] private Transform selectionHighlight;

    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerStamina playerStamina;

    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private Container container;

    public void Start()
    {
        if (item.Count == 0)
        {
            AddGraphics();

        }
        CaseUpdate();
        caseUpdate = false;

        UpdateSelectionUI();
    }

    public void Update()
    {
        HandleDragUpdate(); 
        HandleSlotSelectionInput(); 

        // Проверяем: нажата ЛКМ И инвентарь игрока сейчас ЗАКРЫТ
        // (Замени PlayerInventory.Instance на свою ссылку, если у тебя нет синглтона)
        if (Input.GetMouseButtonDown(0) && !playerInventory.backGround.activeSelf && !container.backGround.activeSelf)
        {
            TryUseActiveItem();
        }
    }

    private void TryUseActiveItem()
    {
        if (playerAttack != null && playerAttack.IsAttacking) return;

        CaseItem activeItem = GetActiveItem();
        if (activeItem == null || activeItem.id == 0) return;

        ItemData itemData = data.item[activeItem.id];

        switch (itemData.type)
        {
            case ItemType.Consumable:
                UseConsumable(activeItem, itemData);
                break;

            case ItemType.Weapon:
                if (playerAttack != null && playerStamina != null)
                {
                    if (playerStamina.TryAttackWithStamina(itemData.staminaCost))
                    {
                        playerAttack.PerformPlayerAttack(itemData);
                    }
                }
                break;
        }
    }
    private void UseConsumable(CaseItem slotItem, ItemData itemData)
    {
        if (playerHealth.GetCurrentHealth() >= playerHealth.GetMaxHealth())
        {
            Debug.Log("Здоровье уже заполнено!");
            return;
        }

        Debug.Log($"Использован расходник: {itemData.name}. Восстановлено {itemData.value} ОЗ.");

        playerHealth.Heal(itemData.value);

        slotItem.count--;

        if (slotItem.count <= 0)    
        {
            AddItem(activeSlotIndex, data.item[0], 0);
        }
        else
        {
            caseUpdate = true;
            CaseUpdate();
        }
    }
    private void HandleSlotSelectionInput()
    {
        int previousSlot = activeSlotIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1)) activeSlotIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) activeSlotIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) activeSlotIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) activeSlotIndex = 3;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            activeSlotIndex--;
            if (activeSlotIndex < 0) activeSlotIndex = maxCount - 1;
        }
        else if (scroll < 0f)
        {
            activeSlotIndex++;
            if (activeSlotIndex >= maxCount) activeSlotIndex = 0;
        }

        // Если слот изменился, обновляем визуальное выделение
        if (previousSlot != activeSlotIndex)
        {
            UpdateSelectionUI();
        }
    }

    private void UpdateSelectionUI()
    {
        if (item.Count == 0 || activeSlotIndex >= item.Count) return;

        if (selectionHighlight != null)
        {
            selectionHighlight.SetParent(item[activeSlotIndex].itemGameObj.transform);
            selectionHighlight.localPosition = Vector3.zero;
            selectionHighlight.gameObject.SetActive(true);
        }
    }

    public CaseItem GetActiveItem()
    {
        if (activeSlotIndex < item.Count)
        {
            return item[activeSlotIndex];
        }
        return null;
    }
}