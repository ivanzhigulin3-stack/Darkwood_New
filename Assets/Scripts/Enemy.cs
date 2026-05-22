using UnityEngine;
using UnityEngine.AI; // Обязательно добавь Component -> Navigation -> NavMeshAgent

[RequireComponent(typeof(NavMeshAgent), typeof(Health))]
public class Enemy : MonoBehaviour
{
    [Header("Атака")]
    public int damage = 20;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    [Header("Зрение")]
    public Transform player;   // Перетащи сюда игрока в Inspector
    public float visionRange = 10f;
    public float attackDistance = 1.8f;

    private NavMeshAgent agent;
    private Health health;
    private Animator anim; // если есть анимации

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        anim = GetComponent<Animator>();

        // Автоматически найти игрока, если не назначен
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // Подписываемся на событие смерти
        health.OnDie.AddListener(() => enabled = false);
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Враг видит игрока?
        if (distanceToPlayer <= visionRange)
        {
            // Догоняем
            agent.SetDestination(player.position);

            // Проверяем, можем ли атаковать
            if (distanceToPlayer <= attackDistance && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            // Если игрок далеко — стоим на месте (можно добавить патрулирование)
            agent.ResetPath();
        }

        // Анимация скорости (опционально)
        if (anim != null)
            anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        // Наносим урон игроку
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("Враг атакует!");
        }

        // Запустить анимацию атаки
        if (anim != null)
            anim.SetTrigger("Attack");
    }
}