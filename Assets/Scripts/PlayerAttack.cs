public class PlayerAttack : AbstractAttack
{
    public void PerformPlayerAttack(ItemData weaponData)
    {
        // Прямо на лету собираем структуру параметров из инвентарного ItemData
        AttackParameters paramsForAttack = new AttackParameters
        {
            damage = weaponData.value,
            range = weaponData.attackRange,
            startup = weaponData.startupTime,
            active = weaponData.activeTime,
            recovery = weaponData.recoveryTime,
            name = weaponData.name
        };

        ExecuteMeleeAttack(paramsForAttack);
    }
}