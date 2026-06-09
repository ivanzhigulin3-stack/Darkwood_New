using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayEntryPoint : MonoBehaviour
{
    [Header("Core Managers")]
    [SerializeField] private PlayerManager playerManager;

    [Header("UI Systems")]
    [SerializeField] private StaminaBarUI staminaBarUI;
    [SerializeField] private HealthBarUI healthBarUI;

    private void Awake()
    {
        // Если на сцене забыли вручную привязать PlayerManager, ищем его
        if (playerManager == null)
        {
            playerManager = FindFirstObjectByType<PlayerManager>();
        }

        InitTimeline();
    }

    private void InitTimeline()
    {
        Debug.Log("[EntryPoint] Начало инициализации сцены...");

        // 1. Проверяем наличие игрока
        if (playerManager == null)
        {
            Debug.LogError("[EntryPoint] Критическая ошибка: PlayerManager не найден! Инициализация прервана.");
            return;
        }

        // 2. Инициализируем врагов на сцене и раздаем им игрока вручную
        InitEnemies();

        // 3. Включаем интерфейс (UI подписывается на уже готового игрока)
        InitUI();

        Debug.Log("[EntryPoint] Сцена успешно собрана и готова к игре!");
    }

    private void InitEnemies()
    {
        // Находим ВСЕХ врагов, которые сейчас есть на уровне
        EnemyAI[] allEnemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);

        Debug.Log($"[EntryPoint] Найдено врагов для инициализации: {allEnemies.Length}");

        // Передаем каждому врагу трансформ игрока напрямую!
        foreach (EnemyAI enemy in allEnemies)
        {
            enemy.Initialize(playerManager.PlayerTransform);
        }
    }

    private void InitUI()
    {
        Debug.Log("[EntryPoint] Инициализация систем интерфейса...");

        // Получаем компоненты характеристик прямо из нашего playerManager
        PlayerStamina playerStamina = playerManager.GetComponent<PlayerStamina>();
        Health playerHealth = playerManager.PlayerHealth;

        // 1. Инициализируем полоску выносливости
        if (staminaBarUI != null && playerStamina != null)
        {
            staminaBarUI.Initialize(playerStamina);
        }
        else
        {
            Debug.LogWarning("[EntryPoint] Пропущена инициализация StaminaBarUI.");
        }

        // 2. Инициализируем полоску здоровья
        if (healthBarUI != null && playerHealth != null)
        {
            healthBarUI.Initialize(playerHealth);
        }
        else
        {
            Debug.LogWarning("[EntryPoint] ПропущенаBox инициализация HealthBarUI (отсутствует скрипт или компонент здоровья).");
        }
    }
}