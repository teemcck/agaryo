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

    [Header("Sound Effects")]
    [Tooltip("Pop sound source played after entity death.")]
    private AudioSource popSource;

    private Rigidbody2D rigidBody;
    private Collider2D playerCollider;
    private Animator playerAnimator;
    private Vector2 moveDirection;

    private Vector2 boundaryMin; // Minimum boundary limits.
    private Vector2 boundaryMax; // Maximum boundary limits.
    private Camera mainCamera;

    public delegate int EnemyCollisionHandler(Collider2D playerCollider, Collider2D enemyCollider);
    public event EnemyCollisionHandler OnPlayerEnemyCollision;

    // Called when the script instance is being loaded.
    void Awake()
    {
        // Initialize player components and calculate boundary limits.
        rigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        playerAnimator = GetComponent<Animator>();

        popSource = GameObject.Find("PopSource")?.GetComponent<AudioSource>();

        if (popSource == null)
        {
            Debug.LogWarning("Pop sound source not found in children.");
        }

        // Set player localScale.
        transform.localScale = Vector3.one * (playerSize * playerScaleIncrement);

        // Set scene camera.
        mainCamera = Camera.main;

        if (mainCamera != null)
        {
            // Adjust camera zoom based on player starting size.
            AdjustCameraZoom();
        }
        else
        {
            Debug.LogError("Main Camera not found!");
        }

        // Find map boundaries to prevent player from exiting map.
        CalculateBoundaryLimits();
    }

    // Called once per frame to handle player input.
    void Update()
    {
        // Capture movement input from the player.
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    // Called at fixed intervals to handle physics-related updates.
    void FixedUpdate()
    {
        // Move the player based on input and apply boundary limits.
        Vector2 newPosition = rigidBody.position + moveDirection * moveSpeed * Time.fixedDeltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, boundaryMin.x, boundaryMax.x);
        newPosition.y = Mathf.Clamp(newPosition.y, boundaryMin.y, boundaryMax.y);

        rigidBody.MovePosition(newPosition);
    }

    // Handles collision with enemy objects.
    private void OnTriggerEnter2D(Collider2D enemyCollider)
    {
        playerSize = (int)OnPlayerEnemyCollision?.Invoke(playerCollider, enemyCollider);

        // playerSize = -1, meaning the player has died.
        if (playerSize == -1)
        {
            MenuNavigation menuNavigation = MenuNavigation.instance;
            if (menuNavigation != null)
            {
                popSource.Play(); // Play a pop sound.
                menuNavigation.SendToEndScreen(); // Send player to death scene.
            }
            else
            {
                Debug.LogError("MenuNavigation instance not found!");
            }
        }
        // playerSize >= 100, meaning the player has won.
        else if (playerSize >= 100)
        {
            MenuNavigation menuNavigation = MenuNavigation.instance;
            if (menuNavigation != null)
            {
                menuNavigation.SendToWinScreen(); // Send player to win scene.
            }
            else
            {
                Debug.LogError("MenuNavigation instance not found!");
            }
        }
        else
        {
            // Update player and camera sizing after gaining mass.
            transform.localScale = Vector3.one * (playerSize * playerScaleIncrement);
            AdjustCameraZoom();

            // Play keyframed fade to red animation.
            PlayFadeAnimation();

            // Play pop sound.
            popSource.Play();
        }
    }

    // Adjusts the camera's zoom level based on the player's size.
    private void AdjustCameraZoom()
    {
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = Mathf.Max(5f, playerSize * playerScaleIncrement);
        }
    }

    // Calculates the boundary limits of the playable area.
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

    // Plays a fade animation when triggered.
    public void PlayFadeAnimation()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("FadeTrigger");
        }
    }
}
