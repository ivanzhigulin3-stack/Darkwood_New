using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public int weaponDamage = 25;
    public float attackRange = 2f;
    public LayerMask enemyLayers; // В инспекторе выбери слой "Enemy"

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши
        {
            Attack();
        }
    }

    void Attack()
    {
        // Эмуляция удара (Raycast или OverlapSphere)
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, attackRange, enemyLayers))
        {
            Health enemyHealth = hit.collider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(weaponDamage);
                Debug.Log("Попал по врагу!");
                // Добавь звук, эффект крови, отбрасывание
            }
        }
    }
}