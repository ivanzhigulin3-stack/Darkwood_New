using UnityEngine;
using UnityEngine.UI;
using System.Collections; 
public class HealthBarUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider healthSlider;

    [Header("Visual Settings")]
    [SerializeField] private float updateSpeed = 100f;

    private Health cachedPlayerHealth;
    private float targetValue;

    public void Initialize(Health playerHealth)
    {
        if (healthSlider == null) healthSlider = GetComponent<Slider>();

        if (playerHealth != null)
        {
            if (cachedPlayerHealth != null)
            {
                cachedPlayerHealth.OnHealthChanged -= HandleHealthChanged;
            }

            cachedPlayerHealth = playerHealth;

            cachedPlayerHealth.OnHealthChanged += HandleHealthChanged;

            StartCoroutine(DeferredInitRoutine());

            Debug.Log($"[HealthBarUI] Успешно инициализирован для {playerHealth.gameObject.name}");
        }
        else
        {
            Debug.LogError($"[HealthBarUI] Передана пустая ссылка на Health игрока!");
        }
    }

    private IEnumerator DeferredInitRoutine()
    {
        yield return new WaitForEndOfFrame();

        if (cachedPlayerHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = (float)cachedPlayerHealth.GetMaxHealth();
            healthSlider.value = (float)cachedPlayerHealth.GetCurrentHealth();
            targetValue = (float)cachedPlayerHealth.GetCurrentHealth();
        }
    }

    private void OnDestroy()
    {
        if (cachedPlayerHealth != null)
        {
            cachedPlayerHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        if (healthSlider == null) return;

        healthSlider.maxValue = (float)maxHealth;
        targetValue = (float)currentHealth;
    }

    private void Update()
    {
        if (healthSlider == null) return;

        if (!Mathf.Approximately(healthSlider.value, targetValue))
        {
            healthSlider.value = Mathf.MoveTowards(healthSlider.value, targetValue, updateSpeed * Time.deltaTime);
        }
    }
}