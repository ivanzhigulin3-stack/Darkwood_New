using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Inventory/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Combat Stats")]
    public int damage = 20;
    public float attackRange = 1.2f;

    [Header("Mortal Kombat Frame Data (Seconds)")]
    public float startupTime = 0.2f;
    public float activeTime = 0.1f;
    public float recoveryTime = 0.3f;

    [Header("Stamina Costs")]
    public float staminaCost = 30f;
}