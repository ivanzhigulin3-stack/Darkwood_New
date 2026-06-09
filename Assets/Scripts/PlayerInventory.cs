using System;
using UnityEngine.EventSystems;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerInventory : BaseCase
{
    public void Start()
    {
        if (item.Count == 0)
        {
            AddGraphics();

            AddTestItems();
        }
    }

    public void Update()
    {
        /*
        if (currentID != -1)
        {
            MoveObject();
        }
        */
        HandleDragUpdate();

        if (Input.GetKeyDown(KeyCode.I))
        {
            backGround.SetActive(!backGround.activeSelf);
            if (backGround.activeSelf)
            {
                CaseUpdate();
                caseUpdate = false;
            }
        }

        if (backGround.activeSelf && caseUpdate)
        {
            CaseUpdate();
            caseUpdate = false;
        }

        
    }

    public void AddTestItems()
    {
        for (int i = 0; i < maxCount; i++)
        {
            AddItem(i, data.item[0], 0);
        }
        AddItemByID(1, 1);
        AddItemByID(2, 2);
        AddItemByID(3, 30);
    }

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
                caseUpdate = true;

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
}