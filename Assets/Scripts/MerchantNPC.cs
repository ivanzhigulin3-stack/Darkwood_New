using UnityEngine;

[RequireComponent(typeof(BaseCase))] 
public class MerchantNPC : MonoBehaviour
{
    private BaseCase merchantStorage;
    private bool isPlayerNearby = false;

    private void Start()
    {
        merchantStorage = GetComponent<BaseCase>();

        if (merchantStorage.item.Count == 0)
        {
            merchantStorage.AddGraphics();
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            ToggleTrade();
        }
    }

    private void ToggleTrade()
    {
        TradeContainer tradeWindow = FindFirstObjectByType<TradeContainer>();
        PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (tradeWindow == null || playerInventory == null) return;

        if (tradeWindow.backGround.activeSelf)
        {
            tradeWindow.CloseTradeWindow();
        }
        else
        {
            playerInventory.backGround.SetActive(true);
            playerInventory.CaseUpdate();

            tradeWindow.OpenMerchantTrade(merchantStorage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("[ЖИТЕЛЬ] Нажми [F], чтобы поторговать.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            TradeContainer tradeWindow = FindFirstObjectByType<TradeContainer>();
            if (tradeWindow != null && tradeWindow.backGround.activeSelf)
            {
                tradeWindow.CloseTradeWindow();
            }
        }
    }
}