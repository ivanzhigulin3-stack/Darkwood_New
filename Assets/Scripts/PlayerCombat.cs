using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public int weaponDamage = 25;
    public float attackRange = 2f;
    public LayerMask enemyLayers; 

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Attack();
        }
    }

    void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, attackRange, enemyLayers))
        {
            Health enemyHealth = hit.collider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(weaponDamage);
                Debug.Log("Попал по врагу!");
               
            }
        }
    }
}