using UnityEngine;
using System.Collections.Generic;

public class Container : MonoBehaviour, IInteractable
{
    [Header("Настройки контейнера")]
    public string containerName = "Сундук";
    public int containerSize = 20;
    public KeyCode openKey = KeyCode.E;
    public float interactionRange = 2f;

    [Header("Визуальные эффекты")]
    public GameObject openEffect;
    public GameObject closeEffect;
    public Sprite openSprite;
    public Sprite closeSprite;

    [Header("Начальные предметы")]
    public List<ContainerStartItem> startItems;

    private bool isOpen = false;
    private SpriteRenderer spriteRenderer;
    private Inventory playerInventory;
    private ContainerUI containerUI;
    private GameObject currentPlayer; // Оставляем для ссылки на игрока, но убираем триггерную логику

    public List<ContainerSlot> containerItem = new List<ContainerSlot>();

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        if (closeSprite != null) spriteRenderer.sprite = closeSprite;

        InitializeSlots();
        AddStartItems();

        // Добавляем BoxCollider2D для физики (не триггер)
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        containerUI = FindFirstObjectByType<ContainerUI>(FindObjectsInactive.Include);
        if (containerUI != null) containerUI.Initialize(this);
    }

    private void Update()
    {
        // Убираем проверку нажатия клавиши отсюда - теперь это делает PlayerInteraction
        // Код взаимодействия перенесен в метод Interact()
    }

    private void InitializeSlots()
    {
        containerItem.Clear();
        for (int i = 0; i < containerSize; i++)
        {
            containerItem.Add(new ContainerSlot());
        }
    }

    private void AddStartItems()
    {
        if (startItems != null)
        {
            foreach (var startItem in startItems)
            {
                if (startItem.itemID > 0 && startItem.amount > 0)
                {
                    AddItem(startItem.itemID, startItem.amount);
                }
            }
        }
    }

    public void Interact(GameObject player)
    {
        currentPlayer = player;
        playerInventory = player.GetComponent<Inventory>();

        if (!isOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    public string GetInteractionText()
    {
        string action = isOpen ? "Закрыть" : "Открыть";
        return $"{action} {containerName} [E]";
    }

    private void Open()
    {
        isOpen = true;

        if (openSprite != null) spriteRenderer.sprite = openSprite;

        if (openEffect != null) Instantiate(openEffect, transform.position, Quaternion.identity);

        if (containerUI != null)
        {
            containerUI.ShowContainer(this, playerInventory);
        }

        if (currentPlayer != null)
        {
            PlayerMovement playerMovement = currentPlayer.GetComponent<PlayerMovement>();
            if (playerMovement != null) playerMovement.enabled = false;
        }

        Debug.Log($"Вы открыли {containerName}");
    }

    public void Close()
    {
        isOpen = false;

        if (closeSprite != null)
            spriteRenderer.sprite = closeSprite;

        if (closeEffect != null)
            Instantiate(closeEffect, transform.position, Quaternion.identity);

        if (containerUI != null)
        {
            containerUI.CloseContainer();
        }

        if (currentPlayer != null)
        {
            PlayerMovement playerMovement = currentPlayer.GetComponent<PlayerMovement>();
            if (playerMovement != null)
                playerMovement.enabled = true;
        }

        Debug.Log($"Вы закрыли {containerName}");
    }

    public bool AddItem(int itemID, int amount)
    {
        DataBase dataBase = FindFirstObjectByType<DataBase>();
        if (dataBase == null || itemID >= dataBase.item.Count) return false;

        ItemData itemData = dataBase.item[itemID];
        int remainingAmount = amount;

        // Сначала добавляем в существующие стеки
        for (int i = 0; i < containerItem.Count; i++)
        {
            if (containerItem[i].id == itemID && containerItem[i].id != 0)
            {
                int spaceLeft = itemData.stack - containerItem[i].amount;
                if (spaceLeft > 0)
                {
                    int toAdd = Mathf.Min(remainingAmount, spaceLeft);
                    containerItem[i].amount += toAdd;
                    remainingAmount -= toAdd;

                    if (remainingAmount <= 0)
                    {
                        UpdateUI();
                        return true;
                    }
                }
            }
        }

        // Затем в пустые слоты
        for (int i = 0; i < containerItem.Count; i++)
        {
            if (containerItem[i].id == 0)
            {
                int toAdd = Mathf.Min(remainingAmount, itemData.stack);
                containerItem[i].id = itemID;
                containerItem[i].amount = toAdd;
                remainingAmount -= toAdd;

                if (remainingAmount <= 0)
                {
                    UpdateUI();
                    return true;
                }
            }
        }

        Debug.Log($"Не хватило места в {containerName} для {itemData.name} x{amount}");
        UpdateUI();
        return false;
    }

    public bool RemoveItem(int slotIndex, int amount)
    {
        if (slotIndex >= containerItem.Count) return false;

        if (containerItem[slotIndex].amount >= amount)
        {
            containerItem[slotIndex].amount -= amount;

            if (containerItem[slotIndex].amount <= 0)
            {
                containerItem[slotIndex].id = 0;
                containerItem[slotIndex].amount = 0;
            }

            UpdateUI();
            return true;
        }
        return false;
    }

    public ContainerSlot GetItem(int slotIndex)
    {
        if (slotIndex < containerItem.Count)
            return containerItem[slotIndex];
        return null;
    }

    private void UpdateUI()
    {
        if (containerUI != null && isOpen)
        {
            containerUI.UpdateUI();
        }
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}

[System.Serializable]
public class ContainerStartItem
{
    public int itemID;
    public int amount;
}

[System.Serializable]
public class ContainerSlot
{
    public int id;
    public int amount;
    public ContainerSlot()
    {
        id = 0;
        amount = 0;
    }
}