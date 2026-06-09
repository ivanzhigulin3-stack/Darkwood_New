using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    public List<ItemData> item = new List<ItemData>();

    private void Awake()
    {
        foreach (var i in item)
        {
            if (i.isWeapon && i.weaponDetails != null)
            {
                i.value = i.weaponDetails.damage;
                i.attackRange = i.weaponDetails.attackRange;
                i.startupTime = i.weaponDetails.startupTime;
                i.activeTime = i.weaponDetails.activeTime;
                i.recoveryTime = i.weaponDetails.recoveryTime;
                i.staminaCost = i.weaponDetails.staminaCost;
            }
        }
    }
}

[System.Serializable]
public class ItemData
{
    [Header("Base Item Settings")]
    public int id;
    public string name;
    public Sprite image;
    public int stack;
    public ItemType type;
    public int value;

    [Header("Economy Settings")]
    public int price;

    [Header("Weapon Configurations")]
    public bool isWeapon; 

    public WeaponData weaponDetails;

   
    [HideInInspector] public float attackRange;
    [HideInInspector] public float startupTime;
    [HideInInspector] public float activeTime;
    [HideInInspector] public float recoveryTime;
    [HideInInspector] public float staminaCost;
}

public enum ItemType
{
    Consumable,
    Weapon,
    Resource,
    Quest
}