using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [Tooltip("Enemy prefab must have an EnemyMovement component")]
    public GameObject enemyPrefab;

    [Tooltip("Spawn points for each track (index matches endPoints)")]
    public Transform[] spawnPoints;

    [Tooltip("End points for each track (index matches spawnPoints)")]
    public Transform[] endPoints;

    public float spawnIntervalMin = 1.0f;
    public float spawnIntervalMax = 2.5f;
    public float initialDelay = 1.0f;

    void Start()
    {
        if (enemyPrefab == null) Debug.LogWarning("SpawnManager: enemyPrefab not assigned");
        if (spawnPoints == null || spawnPoints.Length == 0) Debug.LogWarning("SpawnManager: spawnPoints empty");
        if (endPoints == null || endPoints.Length == 0) Debug.LogWarning("SpawnManager: endPoints empty");
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            float wait = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(wait);

            if (enemyPrefab == null) continue;
            if (spawnPoints == null || spawnPoints.Length == 0) continue;

            int index = Random.Range(0, spawnPoints.Length);
            if (endPoints == null || index >= endPoints.Length) continue;

            Transform spawn = spawnPoints[index];
            Transform end = endPoints[index];
            if (spawn == null || end == null) continue;

            GameObject go = Instantiate(enemyPrefab, spawn.position, spawn.rotation);
            var movement = go.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.InitializePath(spawn.position, end.position);
            }
        }
    }
}
