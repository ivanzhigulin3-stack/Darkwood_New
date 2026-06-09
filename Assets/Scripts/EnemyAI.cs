using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Mover))]
public class EnemyAI : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool aiEnabled = true;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float stoppingDistance = 1.0f;

    [Header("Stun / Stagger Settings")]
    [Tooltip("Время ступора при обычном ранении")]

    [SerializeField] private float baseStaggerDuration = 0.4f;

    [Header("Optimization Settings")]
    [SerializeField] private float sleepRadius = 25f;

    private Transform playerTransform;
    private Mover mover;
    private EnemyAttack enemyAttack;
    private bool isChasing = false;
    private bool isStaggered = false;

    private Coroutine staggerCoroutine;

    public void Initialize(Transform targetPlayer)
    {
        mover = GetComponent<Mover>();
        enemyAttack = GetComponent<EnemyAttack>();
        playerTransform = targetPlayer;
    }

    void FixedUpdate()
    {
        if (isStaggered) return;

        if (!aiEnabled || playerTransform == null)
        {
            StopChasing();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > sleepRadius)
        {
            StopChasing();
            return;
        }

        if (enemyAttack != null && enemyAttack.IsAttacking)
        {
            StopChasing();
            return;
        }

        mover.RotateTowards(playerTransform.position);

        if (distanceToPlayer > stoppingDistance)
        {
            if (!isChasing)
            {
                isChasing = true;
                mover.StartFollowingTarget(playerTransform, speed);
            }

            mover.MoveAlongPath(speed);
        }
        else
        {
            StopChasing();

            if (enemyAttack != null && !enemyAttack.IsAttacking)
            {
                enemyAttack.PerformEnemyAttack();
            }
        }
    }

    //Вызывается при обычном уроне 
    public void ApplyStagger()
    {
        ApplyStagger(baseStaggerDuration);
    }

    //Вызывается извне (гранаты, супер-удары, кастомное время)
    public void ApplyStagger(float duration)
    {
        // аналог иммунитета
        if (duration <= 0.001f) return;

        // Если враг УЖЕ находится в стане (например, от прошлой гранаты), 
        // сбрасывается старый таймер, чтобы запустить новый (более длинный)
        if (staggerCoroutine != null)
        {
            StopCoroutine(staggerCoroutine);
        }

        staggerCoroutine = StartCoroutine(StaggerRoutine(duration));
    }

    private IEnumerator StaggerRoutine(float duration)
    {
        isStaggered = true;
        StopChasing();

        if (enemyAttack != null)
        {
            enemyAttack.CancelAttack();
        }

        yield return new WaitForSeconds(duration);

        isStaggered = false;
        staggerCoroutine = null; 
    }

    private void StopChasing()
    {
        if (isChasing)
        {
            isChasing = false;
            if (mover != null) mover.StopFollowingTarget();
        }
        else
        {
            if (mover != null) mover.Stop();
        }
    }
}