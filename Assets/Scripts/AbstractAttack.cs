using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackParameters
{
    public int damage;
    public float range;
    public float startup;
    public float active;
    public float recovery;
    public string name;
}

public abstract class AbstractAttack : MonoBehaviour
{
    [Header("Base Attack Setup")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask targetLayer;

    private Coroutine activeAttackCoroutine;
    public bool IsAttacking { get; private set; }

    public void ExecuteMeleeAttack(AttackParameters parameters)
    {
        if (IsAttacking) return;

        activeAttackCoroutine = StartCoroutine(AttackRoutine(parameters));
    }

    private IEnumerator AttackRoutine(AttackParameters p)
    {
        IsAttacking = true;

        // 1. ФАЗА: STARTUP
        yield return new WaitForSeconds(p.startup);

        // 2. ФАЗА: ACTIVE
        float timer = 0f;

        HashSet<Health> targetsAlreadyHit = new HashSet<Health>();

        while (timer < p.active)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, p.range, targetLayer);

            foreach (Collider2D col in hitColliders)
            {
                if (col.TryGetComponent<Health>(out var targetHealth))
                {
                    if (!targetsAlreadyHit.Contains(targetHealth))
                    {
                        targetsAlreadyHit.Add(targetHealth); 
                        targetHealth.TakeDamage(p.damage);   

                        Debug.Log($"[{gameObject.name}] Успешно ранил {col.gameObject.name}. Число жертв за взмах: {targetsAlreadyHit.Count}");
                    }
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 3. ФАЗА: RECOVERY
        yield return new WaitForSeconds(p.recovery);

        IsAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, 0.5f);
    }

    public void CancelAttack()
    {
        if (!IsAttacking) return;

        if (activeAttackCoroutine != null)
        {
            StopCoroutine(activeAttackCoroutine);
        }

        IsAttacking = false;
        Debug.Log($"[{gameObject.name}] Атака была ПРЕРВАНА из-за стана/пошатывания!");
    }
}