using TMPro;
using UnityEngine;

public class SizeUIUpdater : MonoBehaviour
{
    [Tooltip("UI Text element to display the player's size.")]
    [SerializeField] private TMP_Text sizeText;

    private PlayerController playerController;

    void OnEnable()
    {
        // Attempt to find the PlayerController instance in the scene.
        playerController = FindObjectOfType<PlayerController>();

        if (playerController == null)
        {
            Debug.Log("PlayerController not found in the scene!");
        }
    }

    // Update size UI in corner every frame.
    void Update()
    {
        if (playerController != null && sizeText != null)
        {
            // Update the UI text with the player's current size
            sizeText.text = $"Size: {playerController.playerSize}";
        }
        else
        {
            Debug.Log("PlayerController or SizeText is not assigned!");
        }
    }
}
