using UnityEngine;
using UnityEngine.UI;

public class TradeContainer : BaseCase
{
    [Header("Trade UI Elements")]
    [SerializeField] private Text costText;
    [SerializeField] private Button confirmButton;

    private int requiredAmount = 0;
    private int currentBasketValue = 0;
    private bool isLifeBuyout = false;

    private BaseCase merchantInventory; 
    private int dealBalance = 0;       

    [Header("Respawn Reference (Hidden)")]
    private Vector3 calculatedSpawnPos;
    private GameObject playerInstance;

    public void Start()
    {
        if (item.Count == 0) AddGraphics();

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(TryConfirmTrade);
            confirmButton.interactable = false;
        }

        UpdateTradeUI();
    }

    public void OpenLifeBuyout(int deathTax, Vector3 spawnPos, GameObject player)
    {
        requiredAmount = deathTax;
        currentBasketValue = 0;
        isLifeBuyout = true;
        merchantInventory = null;

        calculatedSpawnPos = spawnPos;
        playerInstance = player;

        ClearTradeContainer();
        backGround.SetActive(true);
        UpdateTradeUI();
    }

    public void OpenMerchantTrade(BaseCase merchantStorage)
    {
        isLifeBuyout = false;
        merchantInventory = merchantStorage;
        dealBalance = 0;

        ClearTradeContainer();
        backGround.SetActive(true);

        if (merchantInventory != null)
        {
            merchantInventory.backGround.SetActive(true);
            merchantInventory.CaseUpdate();
        }

        UpdateTradeUI();
    }

    public void RecalculateBasketValue()
    {
        currentBasketValue = 0;

        for (int i = 0; i < maxCount; i++)
        {
            if (item[i].id != 0)
            {
                int itemPrice = data.item[item[i].id].price;
                currentBasketValue += itemPrice * item[i].count;
            }
        }
        dealBalance = currentBasketValue;

        UpdateTradeUI();
    }

    private void UpdateTradeUI()
    {
        if (costText != null)
        {
            if (isLifeBuyout)
            {
                costText.text = $"Пожертвуйте: {currentBasketValue} / {requiredAmount}";
            }
            else
            {
                costText.text = $"Ценность предложения: {dealBalance}";
            }
        }

        if (confirmButton != null)
        {
            if (isLifeBuyout)
            {
                confirmButton.interactable = (currentBasketValue >= requiredAmount);
            }
            else
            {
                confirmButton.interactable = (dealBalance > 0);
            }
        }
    }

    private void TryConfirmTrade()
    {
        if (isLifeBuyout && currentBasketValue < requiredAmount) return;
        if (!isLifeBuyout && dealBalance <= 0) return;

        Debug.Log("[ТОРГОВЛЯ] Сделка совершена успешно!");

        ClearTradeContainer();

        if (isLifeBuyout)
        {
            if (playerInstance != null && SpawnManager.Instance != null)
            {
                SpawnManager.Instance.RespawnPlayer(playerInstance, calculatedSpawnPos);
            }
        }
        else
        {
            if (merchantInventory != null)
            {
                merchantInventory.backGround.SetActive(false);
            }
        }

        backGround.SetActive(false);
    }

    public void CloseTradeWindow()
    {
        if (isLifeBuyout) return; 

        ReturnItemsToPlayer();

        if (merchantInventory != null)
        {
            merchantInventory.backGround.SetActive(false);
        }

        backGround.SetActive(false);
    }

    private void ClearTradeContainer()
    {
        for (int i = 0; i < maxCount; i++)
        {
            AddItem(i, data.item[0], 0);
        }
        CaseUpdate();
    }

    private void ReturnItemsToPlayer()
    {
        PlayerInventory playerInv = FindFirstObjectByType<PlayerInventory>();
        if (playerInv == null) return;

        for (int i = 0; i < maxCount; i++)
        {
            if (item[i].id != 0)
            {
                playerInv.AddItemByID(item[i].id, item[i].count);
            }
        }
        ClearTradeContainer();
        playerInv.CaseUpdate();
    }
}