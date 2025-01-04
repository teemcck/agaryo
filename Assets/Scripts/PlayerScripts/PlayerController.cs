using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Properties")]
    [Tooltip("Movement speed of the player.")]
    [SerializeField] private float moveSpeed;

    [Tooltip("Scale increment per size unit.")]
    [SerializeField] private float playerScaleIncrement;

    [Tooltip("Starting size of player.")]
    [SerializeField] public int playerSize;

    [Tooltip("Stores index of end game scene (available in build settings).")]
    [SerializeField] public int endSceneIndex;

    private Rigidbody2D rigidBody;
    private Collider2D playerCollider;
    private Vector2 moveDirection;

    private Vector2 boundaryMin; // Minimum boundary limits
    private Vector2 boundaryMax; // Maximum boundary limits

    // Define delegate function for collision event.
    public delegate int EnemyCollisionHandler(Collider2D playerCollider, Collider2D enemyCollider);
    public event EnemyCollisionHandler OnPlayerEnemyCollision;

    void Awake()
    {
        // Get the Rigidbody2D and Collider2D components from the player.
        rigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        // Set size of player according to starting player size and the scale increment.
        transform.localScale = Vector3.one * (playerSize * playerScaleIncrement);

        // Calculate the overall boundaries from all "Border" tagged objects
        CalculateBoundaryLimits();
    }

    void Update()
    {
        // Get player input for movement (WASD or arrow keys)
        float moveX = Input.GetAxisRaw("Horizontal");  // Left/Right (A/D or Arrow keys)
        float moveY = Input.GetAxisRaw("Vertical");    // Up/Down (W/S or Arrow keys)

        moveDirection = new Vector2(moveX, moveY).normalized; // Normalize to prevent faster diagonal movement
    }

    void FixedUpdate()
    {
        // Move the player
        Vector2 newPosition = rigidBody.position + moveDirection * moveSpeed * Time.fixedDeltaTime;

        // Clamp the new position within the boundary limits
        newPosition.x = Mathf.Clamp(newPosition.x, boundaryMin.x, boundaryMax.x);
        newPosition.y = Mathf.Clamp(newPosition.y, boundaryMin.y, boundaryMax.y);

        rigidBody.MovePosition(newPosition);
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
        }
        else
        {
            Debug.LogWarning("No objects tagged as 'Border' were found in the scene.");
        }
    }

    private void OnTriggerEnter2D(Collider2D enemyCollider)
    {
        // If player collides with enemy, trigger the OnPlayerEnemyCollision event.
        // The invoked function returns the new size of the player, or -1 if the player has died.
        playerSize = (int)OnPlayerEnemyCollision?.Invoke(playerCollider, enemyCollider);

        // Update new player size.
        if (playerSize == -1)
        {
            // The player has died, switch to game over scene.
            MenuNavigation menuNavigation = MenuNavigation.instance; // Access the MenuNavigation instance.
            if (menuNavigation != null)
            {
                menuNavigation.SendToEndScreen(); // Call SendToEndScreen on the instance
            }
            else
            {
                Debug.LogError("MenuNavigation instance not found!");
            }
        }
        else
        {
            // Set size of player according to new playerSize.
            transform.localScale = Vector3.one * (playerSize * playerScaleIncrement);
        }
    }
}
