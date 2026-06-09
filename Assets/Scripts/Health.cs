using UnityEngine;
using System;

public abstract class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int currentHealth;

    // События, на которые могут подписываться другие скрипты (например, UI полоски здоровья)
    public event Action<int, int> OnHealthChanged; // Передает (currentHealth, maxHealth)
    public event Action OnDeath;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public virtual void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"{gameObject.name} получил {damage} урона. Осталось ОЗ: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        if (currentHealth <= 0) return; 

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    protected virtual void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} умер!");
    }

    public virtual void ResetHealthAfterRespawn()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}