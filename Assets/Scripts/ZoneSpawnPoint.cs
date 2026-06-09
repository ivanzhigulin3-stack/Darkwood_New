using UnityEngine;

public class ZoneSpawnPoint : MonoBehaviour
{
    // Скрипт пустой, он нужен только как метка-компонент, 
    // чтобы менеджер мог найти все точки на сцене и узнать их Vector3 позицию.

    private void OnDrawGizmos()
    {
        // Рисуем иконку в редакторе Unity, чтобы тебе было удобно видеть точки спавна
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}