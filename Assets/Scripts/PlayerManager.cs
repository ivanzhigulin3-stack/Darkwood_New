using Unity.VectorGraphics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Статическая ссылка (Синглтон) — точка доступа, видимая из любого скрипта проекта
    public static PlayerManager Instance { get; private set; }

    [Header("Player References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Health playerHealth;

    // Геттеры для безопасного получения данных
    public Transform PlayerTransform => playerTransform;
    public Health PlayerHealth => playerHealth;

    private void Awake()
    {
        // Настройка Синглтона: менеджер должен быть только один
        if (Instance == null)
        {
            Instance = this;
            // Если менеджер должен жить между сценами (как в видео), раскомментируй строку ниже:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Если ссылки не перетащили вручную в инспекторе, ищем их на текущем объекте
        if (playerTransform == null) playerTransform = transform;
        if (playerHealth == null) playerHealth = GetComponent<Health>();
    }
}