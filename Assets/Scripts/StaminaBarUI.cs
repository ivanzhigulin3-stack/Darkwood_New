using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider staminaSlider;

    [Header("Visual Settings")]
    [SerializeField] private float updateSpeed = 100f;

    private PlayerStamina cachedPlayerStamina;
    private float targetValue;

    public void Initialize(PlayerStamina playerStamina)
    {
        if (staminaSlider == null) staminaSlider = GetComponent<Slider>();

        if (playerStamina != null)
        {
            cachedPlayerStamina = playerStamina;

            // Подписываемся на событие
            cachedPlayerStamina.OnStaminaChanged += HandleStaminaChanged;

            // Сразу выставляем актуальные значения
            staminaSlider.maxValue = 100f; 
            staminaSlider.value = cachedPlayerStamina.GetCurrentStamina();
            targetValue = cachedPlayerStamina.GetCurrentStamina();

            Debug.Log($"[StaminaBarUI] Успешно инициализирован для {playerStamina.gameObject.name}");
        }
        else
        {
            Debug.LogError($"[StaminaBarUI] Передана пустая ссылка на PlayerStamina!");
        }
    }

    private void OnDestroy()
    {
        // Безопасная отписка
        if (cachedPlayerStamina != null)
        {
            cachedPlayerStamina.OnStaminaChanged -= HandleStaminaChanged;
        }
    }

    private void HandleStaminaChanged(float currentStamina, float maxStamina)
    {
        staminaSlider.maxValue = maxStamina;
        targetValue = currentStamina;
    }

    private void Update()
    {
        if (!Mathf.Approximately(staminaSlider.value, targetValue))
        {
            staminaSlider.value = Mathf.MoveTowards(staminaSlider.value, targetValue, updateSpeed * Time.deltaTime);
        }
    }
}