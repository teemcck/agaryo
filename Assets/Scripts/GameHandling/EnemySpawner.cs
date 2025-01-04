using UnityEngine;
using System.Collections;

public class EntitySpawner : MonoBehaviour
{
    [Header("Player Properties")]
    [Tooltip("Player prefab.")]
    [SerializeField] private GameObject playerPrefab;

    [Tooltip("Player spawn location")]
    [SerializeField] private Vector3 playerSpawnLocation;

    [Header("Enemy Properties")]
    [Tooltip("WanderingEnemy prefab.")]
    [SerializeField] private GameObject wanderingEnemyPrefab;

    [Tooltip("StationaryEnemy prefab.")]
    [SerializeField] private GameObject stationaryEnemyPrefab;

    [Tooltip("Maximum number of wandering enemies to spawn.")]
    [SerializeField] private int maxWanderingEnemies;

    [Tooltip("Maximum number of stationary enemies to spawn.")]
    [SerializeField] private int maxStationaryEnemies;

    [Tooltip("Delay in seconds between wandering enemy spawns.")]
    [SerializeField] private float wanderingSpawnInterval;

    [Tooltip("Initial delay in seconds before wandering enemy spawning starts.")]
    [SerializeField] private float initialWanderingSpawnDelay;

    private int currentWanderingEnemyCount = 0;
    private int currentStationaryEnemyCount = 0;
    private Vector2 boundaryMin;
    private Vector2 boundaryMax;

    // Parent GameObject for organizing enemies in the hierarchy
    private GameObject enemiesParent;

    void Awake()
    {
        // Locate the GameObjects empty in the scene hierarchy.
        GameObject gameObjects = GameObject.Find("GameObjects");

        // Instantiate the player object.
        Instantiate(playerPrefab, playerSpawnLocation, Quaternion.identity, gameObjects.transform);

        // Create a parent GameObject to organize enemies.
        enemiesParent = new GameObject("Enemies");

        // Set Enemies as a child of GameObjects.
        if (gameObjects != null)
        {
            enemiesParent.transform.SetParent(gameObjects.transform);
        }
        else
        {
            Debug.LogWarning("GameObjects parent not found in the scene. Enemies will remain unparented.");
        }

        // Calculate the map boundaries based on Border objects
        CalculateBoundaryLimits();

        // Spawn stationary enemies
        SpawnStationaryEnemies();

        // Gradually start spawning wandering enemies
        StartCoroutine(SpawnWanderingEnemiesWithDelay());
    }

    IEnumerator SpawnWanderingEnemiesWithDelay()
    {
        // Wait for the initial delay
        yield return new WaitForSeconds(initialWanderingSpawnDelay);

        // Spawn wandering and stationary enemies until the maximum is reached
        while (currentWanderingEnemyCount < maxWanderingEnemies || currentStationaryEnemyCount < maxStationaryEnemies)
        {
            if (currentWanderingEnemyCount < maxWanderingEnemies)
            {
                SpawnWanderingEnemy();
                currentWanderingEnemyCount++;
            }

            yield return new WaitForSeconds(wanderingSpawnInterval);
        }
    }

    private void SpawnWanderingEnemy()
    {
        Vector3 spawnPosition = new Vector3(0, 0, 0); // Center of the map.
        GameObject newEnemy = Instantiate(wanderingEnemyPrefab, spawnPosition, Quaternion.identity, enemiesParent.transform);

        // Register a callback to handle wandering enemy destruction
        WanderingEnemy wanderingEnemyScript = newEnemy.GetComponent<WanderingEnemy>();
        if (wanderingEnemyScript != null)
        {
            wanderingEnemyScript.OnEnemyDestroyed += HandleWanderingEnemyDestroyed;
        }
    }

    private void SpawnStationaryEnemy()
    {
        Vector3 spawnPosition = GetRandomPositionWithinBounds();
        Instantiate(stationaryEnemyPrefab, spawnPosition, Quaternion.identity, enemiesParent.transform);
    }

    private void SpawnStationaryEnemies()
    {
        for (int i = 0; i < maxStationaryEnemies; i++)
        {
            SpawnStationaryEnemy();
        }
    }

    private void HandleWanderingEnemyDestroyed()
    {
        StartCoroutine(WaitAndSpawnNewWanderingEnemy());
    }

    IEnumerator WaitAndSpawnNewWanderingEnemy()
    {
        yield return new WaitForSeconds(1f);
        if (currentWanderingEnemyCount < maxWanderingEnemies)
        {
            SpawnWanderingEnemy();
            currentWanderingEnemyCount++;
        }
    }

    private Vector3 GetRandomPositionWithinBounds()
    {
        float randomX = Random.Range(boundaryMin.x, boundaryMax.x);
        float randomY = Random.Range(boundaryMin.y, boundaryMax.y);
        return new Vector3(randomX, randomY, 0);
    }

    private void CalculateBoundaryLimits()
    {
        // Find all objects tagged as "Border"
        GameObject[] borders = GameObject.FindGameObjectsWithTag("Border");

        if (borders.Length > 0)
        {
            // Initialize boundary limits to extremes
            boundaryMin = new Vector2(float.MaxValue, float.MaxValue);
            boundaryMax = new Vector2(float.MinValue, float.MinValue);

            // Iterate through each border to calculate the overall bounds
            foreach (GameObject border in borders)
            {
                Collider2D borderCollider = border.GetComponent<Collider2D>();
                if (borderCollider != null)
                {
                    // Update min and max bounds based on the collider's bounds
                    boundaryMin = Vector2.Min(boundaryMin, borderCollider.bounds.min);
                    boundaryMax = Vector2.Max(boundaryMax, borderCollider.bounds.max);
                }
            }

            // Apply padding to the boundaries
            boundaryMin += Vector2.one;
            boundaryMax -= Vector2.one;
        }
        else
        {
            Debug.LogWarning("No objects tagged as 'Border' were found in the scene.");
        }
    }
}
