using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Vector3 GetClosestSpawnPosition(Vector3 deathPosition)
    {
        ZoneSpawnPoint[] allSpawns = FindObjectsByType<ZoneSpawnPoint>(FindObjectsSortMode.None);

        if (allSpawns.Length == 0)
        {
            Debug.LogWarning("[СПАВН] На сцене нет ни одной ZoneSpawnPoint! Спавним в нуле координат.");
            return Vector3.zero;
        }

        ZoneSpawnPoint closestSpawn = allSpawns[0];
        float closestDistance = Vector3.Distance(deathPosition, closestSpawn.transform.position);

        // Ищем самую близкую геометрически точку
        for (int i = 1; i < allSpawns.Length; i++)
        {
            float distance = Vector3.Distance(deathPosition, allSpawns[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSpawn = allSpawns[i];
            }
        }

        Debug.Log($"[СПАВН] Ближайшая точка возрождения: {closestSpawn.gameObject.name} на расстоянии {closestDistance} метров.");
        return closestSpawn.transform.position;
    }

    public void RespawnPlayer(GameObject player, Vector3 spawnTargetPos)
    {
        player.transform.position = spawnTargetPos;
        player.SetActive(true);
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ResetHealthAfterRespawn();
        }

        Debug.Log("[СПАВН] Игрок возвращен на локацию. ХП полностью обновлено в UI.");
    }
}