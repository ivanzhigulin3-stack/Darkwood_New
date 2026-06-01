using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager Instance;

    private BaseCase sourceInventory;
    private int sourceSlotID;
    private CaseItem draggedItem;
    private GameObject dragVisual;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartDrag(BaseCase inventory, int slotID, CaseItem item, GameObject visual)
    {
        sourceInventory = inventory;
        sourceSlotID = slotID;
        draggedItem = inventory.CopyCaseItem(item);

        dragVisual = visual;
        dragVisual.SetActive(true);
        dragVisual.GetComponent<UnityEngine.UI.Image>().sprite = inventory.data.item[item.id].image;

        // Очищаем исходный слот
        inventory.AddItem(slotID, inventory.data.item[0], 0);
    }

    public void DropOn(BaseCase targetInventory, int targetSlotID)
    {
        if (draggedItem == null || sourceInventory == null)
            return;

        CaseItem targetItem = targetInventory.item[targetSlotID];

        // Если в целевом слоте пусто
        if (targetItem.id == 0)
        {
            targetInventory.AddCaseItem(targetSlotID, draggedItem);
            EndDrag();
        }
        // Если предметы одинаковые и можно стакать
        else if (targetItem.id == draggedItem.id)
        {
            ItemData itemData = sourceInventory.data.item[draggedItem.id];
            int totalCount = targetItem.count + draggedItem.count;

            if (totalCount <= itemData.stack)
            {
                targetItem.count = totalCount;
                targetInventory.caseUpdate = true;
                EndDrag();
            }
            else
            {
                targetItem.count = itemData.stack;
                int remaining = totalCount - itemData.stack;
                draggedItem.count = remaining;
                sourceInventory.AddCaseItem(sourceSlotID, draggedItem);
                targetInventory.caseUpdate = true;
                sourceInventory.caseUpdate = true;
                EndDrag();
            }
        }
        // Разные предметы - меняем местами
        else
        {
            // Сохраняем целевой предмет
            CaseItem tempItem = targetInventory.CopyCaseItem(targetItem);

            // Очищаем целевой слот
            targetInventory.AddItem(targetSlotID, targetInventory.data.item[0], 0);

            // Вставляем перетаскиваемый предмет в целевой слот
            targetInventory.AddCaseItem(targetSlotID, draggedItem);

            // Вставляем сохраненный предмет в исходный слот
            sourceInventory.AddCaseItem(sourceSlotID, tempItem);

            EndDrag();
        }
    }

    public void EndDrag()
    {
        if (dragVisual != null)
            dragVisual.SetActive(false);

        draggedItem = null;
        sourceInventory = null;
        sourceSlotID = -1;
    }

    public bool IsDragging() => draggedItem != null;
}