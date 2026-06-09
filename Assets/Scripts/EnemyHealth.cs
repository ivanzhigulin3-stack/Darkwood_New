using UnityEngine;

public class EnemyHealth : Health
{
    protected override void Die()
    {
        base.Die(); 

        if (TryGetComponent<EnemyAI>(out var ai))
        {
            ai.enabled = false;
        }

        if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;
        if (TryGetComponent<Rigidbody2D>(out var rb)) rb.bodyType = RigidbodyType2D.Kinematic;

        Destroy(gameObject, 1.5f);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (TryGetComponent<EnemyAI>(out var enemyAI))
        {
            enemyAI.ApplyStagger();
        }
    }
}