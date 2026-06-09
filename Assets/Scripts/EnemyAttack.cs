using UnityEngine;

public class EnemyAttack : AbstractAttack
{
    [SerializeField] private EnemyAttackData attackData;

    public void PerformEnemyAttack()
    {
        if (attackData == null) return;

        // Собираем структуру из ScriptableObject врага
        AttackParameters paramsForAttack = new AttackParameters
        {
            damage = attackData.damage,
            range = attackData.attackRange,
            startup = attackData.startupTime,
            active = attackData.activeTime,
            recovery = attackData.recoveryTime,
            name = "Monster Attack"
        };

        ExecuteMeleeAttack(paramsForAttack);
    }
}