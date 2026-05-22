using UnityEngine;
using UnityEngine.Events; 

public class Health : MonoBehaviour
{
    [Header("Параметры")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("События (UI, анимация, звук)")]
    public UnityEvent OnTakeDamage;   
    public UnityEvent OnDie;           
    public UnityEvent OnHeal;          

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return; 

        currentHealth -= amount;
        OnTakeDamage?.Invoke(); 

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        Debug.Log($"{gameObject.name} Health: {currentHealth}/{maxHealth}");
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHeal?.Invoke();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} умер.");
        OnDie?.Invoke();

        if (gameObject.CompareTag("Player"))
        {
            // Здесь будет логика смерти игрока
        }
        else
        {
            Destroy(gameObject, 1f); 
        }
    }
}