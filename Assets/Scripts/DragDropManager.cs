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

        inventory.AddItem(slotID, inventory.data.item[0], 0);
    }

    public void DropOn(BaseCase targetInventory, int targetSlotID)
    {
        if (draggedItem == null || sourceInventory == null)
            return;

        CaseItem targetItem = targetInventory.item[targetSlotID];

        if (targetItem.id == 0)
        {
            targetInventory.AddCaseItem(targetSlotID, draggedItem);
            EndDrag();
        }
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
        else
        {
            CaseItem tempItem = targetInventory.CopyCaseItem(targetItem);
            targetInventory.AddItem(targetSlotID, targetInventory.data.item[0], 0);
            targetInventory.AddCaseItem(targetSlotID, draggedItem);
            sourceInventory.AddCaseItem(sourceSlotID, tempItem);
            EndDrag();
        }

        if (targetInventory is TradeContainer tradeTarget)
        {
            tradeTarget.RecalculateBasketValue();
        }
        if (sourceInventory is TradeContainer tradeSource)
        {
            tradeSource.RecalculateBasketValue();
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