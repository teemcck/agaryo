using UnityEngine;
using System.Collections;

public class EntitySpawner : MonoBehaviour
{
    [Header("Player Properties")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 playerSpawnLocation;

    [Header("Enemy Properties")]
    [SerializeField] private GameObject wanderingEnemyPrefab;
    [SerializeField] private GameObject stationaryEnemyPrefab;
    [SerializeField] private int maxWanderingEnemies;
    [SerializeField] private int maxStationaryEnemies;
    [SerializeField] private float wanderingSpawnInterval;
    [SerializeField] private float initialWanderingSpawnDelay;

    private Vector2 boundaryMin;
    private Vector2 boundaryMax;

    private GameObject enemiesParent;
    private Coroutine wanderingEnemyCoroutine;

    // Called when the script instance is being loaded. Sets up initial GameObjects and starts spawning enemies.
    void Awake()
    {
        InitializeGameObjects();
        CalculateBoundaryLimits();
        SpawnStationaryEnemies();
        wanderingEnemyCoroutine = StartCoroutine(SpawnWanderingEnemies());
    }

    // Called when the object is destroyed. Stops coroutines and cleans up resources.
    void OnDestroy()
    {
        if (wanderingEnemyCoroutine != null)
        {
            StopCoroutine(wanderingEnemyCoroutine);
        }
    }

    // Initializes parent GameObjects for organizing entities and spawns the player at a specified location.
    private void InitializeGameObjects()
    {
        // Create gameObjects object if not existing.
        GameObject gameObjects = GameObject.Find("GameObjects") ?? new GameObject("GameObjects");
        Instantiate(playerPrefab, playerSpawnLocation, Quaternion.identity, gameObjects.transform);

        // Set gameObjects to parent of Enemies.
        enemiesParent = new GameObject("Enemies");
        enemiesParent.transform.SetParent(gameObjects.transform);
    }

    // Calculates the boundaries of the map based on objects tagged as "Border."
    private void CalculateBoundaryLimits()
    {
        GameObject[] borders = GameObject.FindGameObjectsWithTag("Border");

        if (borders.Length > 0)
        {
            boundaryMin = new Vector2(float.MaxValue, float.MaxValue);
            boundaryMax = new Vector2(float.MinValue, float.MinValue);

            foreach (GameObject border in borders)
            {
                Collider2D borderCollider = border.GetComponent<Collider2D>();
                if (borderCollider != null)
                {
                    boundaryMin = Vector2.Min(boundaryMin, borderCollider.bounds.min);
                    boundaryMax = Vector2.Max(boundaryMax, borderCollider.bounds.max);
                }
            }
        }
        else
        {
            Debug.LogWarning("No objects tagged as 'Border' were found in the scene.");
        }
    }

    // Coroutine that spawns wandering enemies at intervals until the maximum limit is reached.
    private IEnumerator SpawnWanderingEnemies()
    {
        yield return new WaitForSeconds(initialWanderingSpawnDelay);

        // Spawn wandering enemies at intervals until the max limit is reached.
        for (int i = 0; i < maxWanderingEnemies; i++)
        {
            SpawnWanderingEnemy();
            yield return new WaitForSeconds(wanderingSpawnInterval);
        }
    }

    // Spawns a single wandering enemy and registers its destruction callback.
    private void SpawnWanderingEnemy()
    {
        Vector3 spawnPosition = Vector3.zero; // Center of the map.
        Instantiate(wanderingEnemyPrefab, spawnPosition, Quaternion.identity, enemiesParent.transform);
    }

    // Spawns stationary enemies randomly within the map boundaries.
    private void SpawnStationaryEnemies()
    {
        for (int i = 0; i < maxStationaryEnemies; i++)
        {
            Vector3 spawnPosition = GetRandomPositionWithinBounds();
            Instantiate(stationaryEnemyPrefab, spawnPosition, Quaternion.identity, enemiesParent.transform);
        }
    }

    // Returns a random position within the calculated map boundaries.
    private Vector3 GetRandomPositionWithinBounds()
    {
        float randomX = Random.Range(boundaryMin.x, boundaryMax.x);
        float randomY = Random.Range(boundaryMin.y, boundaryMax.y);
        return new Vector3(randomX, randomY, 0);
    }
}
