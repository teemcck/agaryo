using UnityEngine;

public class WanderingEnemy : MonoBehaviour
{
    [Header("Spawn Properties")]
    [Tooltip("Minimum size for the enemy.")]
    [SerializeField] private int minSize;

    [Tooltip("Maximum size for the enemy.")]
    [SerializeField] private int maxSize;

    [Tooltip("Scale increment per size unit.")]
    [SerializeField] private float enemyScaleIncrement;

    [Header("Movement Properties")]
    [Tooltip("Minimum duration for movement.")]
    [SerializeField] private float minMoveDuration;

    [Tooltip("Maximum duration for movement.")]
    [SerializeField] private float maxMoveDuration;

    [Tooltip("Minimum duration for idle time.")]
    [SerializeField] private float minIdleDuration;

    [Tooltip("Maximum duration for idle time.")]
    [SerializeField] private float maxIdleDuration;

    [Tooltip("Speed of the enemy's movement.")]
    [SerializeField] private float enemySpeed;

    [Header("Boundary Padding")]
    [Tooltip("Padding to keep enemies away from borders.")]
    [SerializeField] private float boundaryPadding = 0.5f;

    public int enemySize;
    private Vector2 moveDirection;
    private Rigidbody2D enemyRigidBody;
    private float moveTimer;
    private float idleTimer;
    private bool isMoving;

    private Vector2 boundaryMin; // Minimum boundary limits
    private Vector2 boundaryMax; // Maximum boundary limits

    public delegate void EnemyDestroyedHandler();
    public event EnemyDestroyedHandler OnEnemyDestroyed;

    void Awake()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();

        // Set the enemy's size randomly between minSize and maxSize.
        enemySize = Random.Range(minSize, maxSize);
        transform.localScale = Vector3.one * (enemySize * enemyScaleIncrement);

        // Set random color
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(Random.value, Random.value, Random.value);
        }

        // Calculate the overall boundaries from all "Border" tagged objects.
        CalculateBoundaryLimits();

        StartIdle();
    }

    void Update()
    {
        if (isMoving)
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0)
            {
                StartIdle();
            }
        }
        else
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                StartMoving();
            }
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 potentialPosition = (Vector2)transform.position + moveDirection * enemySpeed * Time.fixedDeltaTime;

            // Clamp the new position within the boundary limits
            potentialPosition.x = Mathf.Clamp(potentialPosition.x, boundaryMin.x, boundaryMax.x);
            potentialPosition.y = Mathf.Clamp(potentialPosition.y, boundaryMin.y, boundaryMax.y);

            // Move the enemy to the clamped position
            enemyRigidBody.MovePosition(potentialPosition);
        }
        else
        {
            enemyRigidBody.velocity = Vector2.zero;
        }
    }

    private void StartMoving()
    {
        isMoving = true;
        moveTimer = Random.Range(minMoveDuration, maxMoveDuration);
        moveDirection = Random.insideUnitCircle.normalized;
    }

    private void StartIdle()
    {
        isMoving = false;
        idleTimer = Random.Range(minIdleDuration, maxIdleDuration);
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
            boundaryMin += Vector2.one * boundaryPadding;
            boundaryMax -= Vector2.one * boundaryPadding;
        }
        else
        {
            Debug.LogWarning("No objects tagged as 'Border' were found in the scene.");
        }
    }

    private void OnDestroy()
    {
        // Trigger the event when the enemy is destroyed
        OnEnemyDestroyed?.Invoke();
    }
}
