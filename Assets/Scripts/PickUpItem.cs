using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public int itemID;
    public int amount;
    public KeyCode pickupKey = KeyCode.E;


    private Transform player;
    private Inventory PlayerInventory;
    private bool playerInRange = false;
    private GameObject currentPlayer;

    public GameObject pickupEffect; 
    public float floatSpeed = 1f;
    public float floatHeight = 0.2f; 
    public float rotationSpeed = 90f;

    private Vector3 startPosition;
    private float floatTimer = 0f;

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
        FloatAndRotate();

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
            PlayerInventory = currentPlayer.GetComponent<Inventory>();

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
            PlayerInventory = null;

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
    

    private void FloatAndRotate()
    {
        floatTimer += Time.deltaTime * floatSpeed;
        float newY = startPosition.y + Mathf.Sin(floatTimer) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private bool HasFreeSpace()
    {
        if (PlayerInventory == null) return false;

        for (int i = 0; i < PlayerInventory.maxCount; i++)
        {
            if (PlayerInventory.item[i].id == itemID &&
                PlayerInventory.item[i].count < PlayerInventory.data.item[itemID].stack)
                return true;
          
            if (PlayerInventory.item[i].id == 0) return true;
        }

        return false;
    }

    private void AddToInventory()
    {
        int remainingAmount = amount;

        for (int i = 0; i < PlayerInventory.maxCount; i++)
        {
            if (PlayerInventory.item[i].id == itemID)
            {
                int maxStack = PlayerInventory.data.item[itemID].stack;
                int currentCount = PlayerInventory.item[i].count;

                if (currentCount < maxStack)
                {
                    int SpaceLeft = maxStack - currentCount;
                    int toAdd = Mathf.Min(remainingAmount, SpaceLeft);

                    PlayerInventory.item[i].count += toAdd;
                    remainingAmount -= toAdd;
                    PlayerInventory.UpdateInventory();

                    if (remainingAmount <= 0)
                    {
                        OnPickupSuccess();
                        return;
                    } 
                }
            }
        }

        for (int i = 0; i < PlayerInventory.maxCount; i++)
        {
            if (PlayerInventory.item[i].id == 0)
            {
                int maxStack = PlayerInventory.data.item[itemID].stack;
                int toAdd = Mathf.Min(remainingAmount, maxStack);

                PlayerInventory.AddItem(i, PlayerInventory.data.item[itemID], toAdd);
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
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity); 
        }
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
        if (PlayerInventory == null)
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
        if (PlayerInventory != null && PlayerInventory.data != null)
        {
            return PlayerInventory.data.item[itemID].name;
        }

        return "Предмет";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
