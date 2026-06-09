using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public int itemID;
    public int amount;
    public KeyCode pickupKey = KeyCode.E;


    private Transform player;
    private PlayerInventory playerInventory;
    private bool playerInRange = false;
    private GameObject currentPlayer;

    private Vector3 startPosition;
    //private float floatTimer = 0f;

    private GameObject pickupHint;

    private void Start()
    {
        startPosition = transform.position;

        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 1.5f;

        CreatePickupHint();
    }

    private void Update() 
    {
       

        if (playerInRange && Input.GetKeyDown(pickupKey))
        {
            TryPickup();
        }

        if (pickupHint != null && pickupHint.activeSelf)
        {
            pickupHint.transform.LookAt(Camera.main.transform);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentPlayer = other.gameObject;
            playerInventory = currentPlayer.GetComponent<PlayerInventory>();

            ShowPickupHint(true);

            EnableGlowEffect(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            currentPlayer = null;
            playerInventory = null;

            ShowPickupHint(false);

            EnableGlowEffect(false);
        }
    }

    private void ShowPickupHint(bool show) 
    {
        if (pickupHint != null)
        {
            pickupHint.SetActive(show);
        }

        if (show)
        {
            Debug.Log($"Нажмите {pickupKey} чтобы подобрать {GetItemName()} x{amount}");
            //**
        }
    }

    private void EnableGlowEffect(bool enable) 
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            if (enable)
            {
                renderer.material.SetFloat("_Glow", 0.5f);
                
                renderer.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                renderer.material.SetFloat("_Glow", 0f);
                renderer.color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }
    

    private bool HasFreeSpace()
    {
        if (playerInventory == null) return false;

        for (int i = 0; i < playerInventory.maxCount; i++)
        {
            if (playerInventory.item[i].id == itemID &&
                playerInventory.item[i].count < playerInventory.data.item[itemID].stack)
                return true;
          
            if (playerInventory.item[i].id == 0) return true;
        }

        return false;
    }

    private void AddToInventory()
    {
        int remainingAmount = amount;

        for (int i = 0; i < playerInventory.maxCount; i++)
        {
            if (playerInventory.item[i].id == itemID)
            {
                int maxStack = playerInventory.data.item[itemID].stack;
                int currentCount = playerInventory.item[i].count;

                if (currentCount < maxStack)
                {
                    int SpaceLeft = maxStack - currentCount;
                    int toAdd = Mathf.Min(remainingAmount, SpaceLeft);

                    playerInventory.item[i].count += toAdd;
                    remainingAmount -= toAdd;
                    playerInventory.CaseUpdate();

                    if (remainingAmount <= 0)
                    {
                        OnPickupSuccess();
                        return;
                    } 
                }
            }
        }

        for (int i = 0; i < playerInventory.maxCount; i++)
        {
            if (playerInventory.item[i].id == 0)
            {
                int maxStack = playerInventory.data.item[itemID].stack;
                int toAdd = Mathf.Min(remainingAmount, maxStack);

                playerInventory.AddItem(i, playerInventory.data.item[itemID], toAdd);
                remainingAmount -= toAdd;

                if (remainingAmount <= 0)
                {
                    OnPickupSuccess();
                    return;
                }
            }
        }
        Debug.Log($"Недостаточно места для {GetItemName()} x{remainingAmount}");
        //**
    }

    private void OnPickupSuccess()
    {
        Debug.Log($"Подобран {GetItemName()} x{amount}");
        //**
        
        Destroy(gameObject);
    }
    private void CreatePickupHint()
    {
        pickupHint = new GameObject("PickupHint");
        pickupHint.transform.SetParent(transform);
        pickupHint.transform.localPosition = new Vector3(0, 1f, 0);
    }

    private void TryPickup()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Inventory component not found on player!");
            //**
            return;
        }

        if (HasFreeSpace()) 
        {
            AddToInventory();
        }
        else
        {
            Debug.Log("Инвентарь полон!");
            //**
        }
    }

    private string GetItemName()
    {
        if (playerInventory != null && playerInventory.data != null)
        {
            return playerInventory.data.item[itemID].name;
        }

        return "Предмет";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
