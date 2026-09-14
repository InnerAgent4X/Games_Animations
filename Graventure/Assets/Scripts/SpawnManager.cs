// Using adds libraries so I can use game objects and coroutines in Unity
using UnityEngine;
using System.Collections;

// public class as opposed to private class lets the code inside the class be accessed by other scripts. If it were private class then nothing else could see or access it.
public class SpawnManager : MonoBehaviour
{
    // Defining public variables lets me set them in the Unity. Private variables are only accessible inside this class or script
    [Tooltip("Enemy prefab must have an EnemyMovement component")]
    public GameObject PhysicalEnemy;
    public GameObject MagicEnemy;

    [Tooltip("Spawn points for each track (index matches endPoints)")]
    public Transform[] spawnPoints;

    [Tooltip("End points for each track (index matches spawnPoints)")]
    public Transform[] endPoints;

    public int spawnIntervalMin = 1;
    public int spawnIntervalMax = 2;
    public float initialDelay = 1.0f;

    // Start is one of Unity's built-in methods that runs before the first frame, and it only runs once. 
    void Start()
    {
        // Prints a warning if any of the variables are not set in the unity editor.
        if (PhysicalEnemy == null) Debug.LogWarning("SpawnManager: enemyPrefab not assigned");
        if (spawnPoints == null || spawnPoints.Length == 0) Debug.LogWarning("SpawnManager: spawnPoints empty");
        if (endPoints == null || endPoints.Length == 0) Debug.LogWarning("SpawnManager: endPoints empty");

        // calls a subroutine.
        StartCoroutine(SpawnLoop());
    }

    // Coroutines are methods that can pause execution and resume later. They run once per time they get called. (though in this case, the while loop makes it run forever). 
    IEnumerator SpawnLoop()
    {
        //Yield lets the coroutine pause and the waitforseconds lets me add a real time delay.
        yield return new WaitForSeconds(initialDelay);
        while (true && GameManager.Instance.isGameOver == false)
        {
            //this block waits a random time between the min and max, then spawns an enemy from 
            int wait = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(wait);

            // Randomly selects a spawn and end point.
            int index = Random.Range(0, spawnPoints.Length);
            int enemyType = Random.Range(0, 2); // 0 for PhysicalEnemy, 1 for MagicEnemy

            //after selection, it checks sets the spawn and end points to the selected index, and if either is null, it skips this iteration of the loop
            Transform spawn = spawnPoints[index];
            Transform end = endPoints[index];
            if (spawn == null || end == null) continue;

            // Spawns the enemy prefab at the specified location and rotation, then gives start and end points to the EnemyMovement script. 
            GameObject go = Instantiate(enemyType == 0 ? PhysicalEnemy : MagicEnemy, spawn.position, Quaternion.identity);
            Enemy movement = go.GetComponent<Enemy>();
            if (movement != null)
            {
                movement.InitializePath(spawn.position, end.position);
            }
        }
    }
}