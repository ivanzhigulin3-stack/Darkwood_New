using UnityEngine;
using System;

public class PlayerStamina : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina;
    [SerializeField] private float regenRate = 15f;
    [SerializeField] private float regenDelay = 0.75f;

    [Header("Exhaustion Settings (Stamina <= 0)")]
    [SerializeField] private float exhaustedRegenDelay = 2.0f;
    [SerializeField] private float exhaustionDuration = 3.0f; 
    [Range(0f, 1f)]
    [SerializeField] private float exhaustedSpeedMultiplier = 0.4f; 

    [Header("Attack Buffer Settings")]
    [Range(0f, 100f)]
    [SerializeField] private float minPercentOfCostToAttack = 80f; 

    private float regenTimer;
    private float exhaustionTimer;
    private bool isExhausted;

    public event Action<float, float> OnStaminaChanged;

    private void Awake()
    {
        currentStamina = maxStamina;
    }

    private void Start()
    {
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    private void Update()
    {
        if (exhaustionTimer > 0)
        {
            exhaustionTimer -= Time.deltaTime;
            if (exhaustionTimer <= 0)
            {
                isExhausted = false;
                Debug.Log("[Stamina] Усталость прошла, скорость восстановлена.");
            }
        }

        if (regenTimer > 0)
        {
            regenTimer -= Time.deltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, -50f, maxStamina); 
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
    }

    // Универсальный метод для траты стамины (например, для спринта)
    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            CheckExhaustion();
            return true;
        }
        return false;
    }

    // Специфичный метод для АТАКИ: проверяет порог в 80% от стоимости удара
    public bool TryAttackWithStamina(float attackCost)
    {
        // Вычисляем минимально необходимую стамину (например, 50 * 0.8 = 40)
        float minimumRequired = attackCost * (minPercentOfCostToAttack / 100f);

        if (currentStamina >= minimumRequired)
        {
            currentStamina -= attackCost; 
            CheckExhaustion();
            return true;
        }

        Debug.Log($"[Stamina] Слишком устал! Нужно минимум {minimumRequired} выносливости для этого оружия.");
        return false;
    }

    private void CheckExhaustion()
    {
        if (currentStamina <= 0f)
        {
            isExhausted = true;
            exhaustionTimer = exhaustionDuration;
            regenTimer = exhaustedRegenDelay; 
            Debug.LogWarning($"[Stamina] Полное истощение! Замедление на {exhaustionDuration} сек. ОЗ стамины: {currentStamina}");
        }
        else
        {
            regenTimer = regenDelay;
        }

        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    public float GetSpeedMultiplier() => isExhausted ? exhaustedSpeedMultiplier : 1f;
    public float GetCurrentStamina() => currentStamina;
}