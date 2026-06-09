using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : Health
{
    [Header("Player Stagger Settings")]
    [SerializeField] private float damageStunDuration = 0.25f;

    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private bool isStunned = false;
    private Vector3 targetSpawnPosition;

    protected override void Start()
    {
        base.Start();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    public override void TakeDamage(int damage)
    {
        if (!gameObject.activeSelf || currentHealth <= 0) return;

        base.TakeDamage(damage);

        if (CameraShaker.Instance != null)
        {
            CameraShaker.Instance.Shake(0.15f, 0.1f);
        }

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(PlayerStunRoutine());
        }
    }

    private IEnumerator PlayerStunRoutine()
    {
        isStunned = true;

        if (playerAttack != null)
        {
            playerAttack.CancelAttack();
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            if (TryGetComponent<Mover>(out var mover)) mover.Stop();
        }

        yield return new WaitForSeconds(damageStunDuration);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        isStunned = false;
    }
    protected override void Die()
    {
        base.Die();

        Debug.Log("Игрок погиб! Мир материальный исчезает...");

        StopAllCoroutines();
        isStunned = false;

        if (playerMovement != null) playerMovement.enabled = true; 

        Vector3 targetSpawnPosition = transform.position;
        if (SpawnManager.Instance != null)
        {
            targetSpawnPosition = SpawnManager.Instance.GetClosestSpawnPosition(transform.position);
        }

        TradeContainer tradeWindow = FindFirstObjectByType<TradeContainer>();
        PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (tradeWindow != null && playerInventory != null)
        {
            playerInventory.backGround.SetActive(true);
            playerInventory.CaseUpdate();
            tradeWindow.OpenLifeBuyout(350, targetSpawnPosition, gameObject);
        }

        gameObject.SetActive(false);
    }
    public override void ResetHealthAfterRespawn()
    {
        base.ResetHealthAfterRespawn();
        isStunned = false;
    }
    
    
}