using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyAttackData", menuName = "Enemy Attack Data")]
public class EnemyAttackData : ScriptableObject
{
    [Header("Enemy Combat Stats")]
    public int damage = 20;
    public float attackRange = 1.2f;

    [Header("Mortal Kombat Frame Data (Seconds)")]
    public float startupTime = 0.2f;
    public float activeTime = 0.1f;
    public float recoveryTime = 0.3f;
}
