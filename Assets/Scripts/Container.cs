using Unity.Multiplayer.PlayMode;
using UnityEngine;

public class Container : BaseCase
{
    [SerializeField] private PlayerInventory playerInventory;
    private bool isPlayerNearby = false;
    public void Start()
    {
        
        if (item.Count == 0)
        {
            AddGraphics();
            //AddTestItems();
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

        if (Input.GetKeyDown(KeyCode.E) && isPlayerNearby)
        {
            if (!backGround.activeSelf || !playerInventory.backGround.activeSelf) Open();
            else Close();

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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerInteraction"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerInteraction"))
        {
            isPlayerNearby = false;
            Close();
        }
    }

    private void Open()
    {
        backGround.SetActive(true);
        playerInventory.backGround.SetActive(true);
    }
    private void Close()
    {
        backGround.SetActive(false);
        playerInventory.backGround.SetActive(false);
    }
    /*
    public void AddTestItems()
    {
        for (int i = 0; i < maxCount; i++)
        {
            AddItem(i, data.item[0], 0);
        }
        AddItemByID(1, 23);
        AddItemByID(2, Random.Range(3,35));
        AddItemByID(3, 4);
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
    */
}
