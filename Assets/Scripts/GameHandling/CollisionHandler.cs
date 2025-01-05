using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private PlayerController player;

    void OnEnable()
    {
        // Ensure the player reference is set.
        if (player == null)
        {
            // Locate the player object.
            player = FindObjectOfType<PlayerController>();
        }

        if (player != null)
        {
            // Subscribe to the player's collision event.
            player.OnPlayerEnemyCollision += HandlePlayerCollision;
        }
        else
        {
            Debug.LogError("PlayerController not found!");
        }
    }

    void OnDisable()
    {
        if (player != null)
        {
            // Unsubscribe to avoid memory leaks.
            player.OnPlayerEnemyCollision -= HandlePlayerCollision;
        }
    }

    // Compares player & enemy sizes and decides how game should proceed.
    private int HandlePlayerCollision(Collider2D playerCollider, Collider2D enemyCollider)
    {
        // Retrieve the player controller.
        PlayerController player = playerCollider.GetComponent<PlayerController>();

        // Define a variable to store enemy size and the enemy type.
        int enemySize = 0;
        string enemyType = "";

        // Check the tag of the enemy and get its instance accordingly.
        if (enemyCollider.CompareTag("WanderingEnemy"))
        {
            WanderingEnemy wanderingEnemy = enemyCollider.GetComponent<WanderingEnemy>();
            if (wanderingEnemy != null)
            {
                enemySize = wanderingEnemy.enemySize;
                enemyType = "WanderingEnemy";
            }
        }
        else if (enemyCollider.CompareTag("StationaryEnemy"))
        {
            StationaryEnemy stationaryEnemy = enemyCollider.GetComponent<StationaryEnemy>();
            if (stationaryEnemy != null)
            {
                enemySize = stationaryEnemy.enemySize;
                enemyType = "StationaryEnemy";
            }
        }

        int playerSize = player.playerSize;

        // If the enemy was successfully found, handle collision logic.
        if (!string.IsNullOrEmpty(enemyType))
        {
            // Compare player size with enemy size.
            if (playerSize >= enemySize)
            {
                // Player is bigger, consume the enemy.
                Destroy(enemyCollider.gameObject);
                playerSize += enemySize;
                return playerSize; // Return new size.
            }
            else
            {
                // Enemy is bigger.
                return -1; // Game has ended, return -1.
            }
        }
        else
        {
            // If it's neither a WanderingEnemy nor a StationaryEnemy.
            Debug.Log("Debug: Collided with an unknown enemy type.");
            return playerSize; // Invalid collision, return no size change.
        }
    }
}
