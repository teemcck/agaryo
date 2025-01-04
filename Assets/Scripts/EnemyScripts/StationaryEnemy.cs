using UnityEngine;

public class StationaryEnemy : MonoBehaviour
{
    [Tooltip("Scale increment per size unit.")]
    [SerializeField] private float enemyScaleIncrement;

    // Locked size for stationary enemies.
    public int enemySize = 1;

    void Awake()
    {
        // Set random color for the enemy
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Set size of enemy according to enemyScaleIncrement.
        transform.localScale = new Vector3(enemySize, enemySize, 0) * enemyScaleIncrement;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = randomColor;
        }
    }
}
