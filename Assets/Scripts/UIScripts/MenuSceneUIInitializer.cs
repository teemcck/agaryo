using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// This script attaches listeners to menu scene buttons during runtime
// so a single MenuNavigation instance can persist across all scenes.
public class MenuSceneUIInitializer : MonoBehaviour
{
    // Reference to MenuNavigation Singleton.
    private MenuNavigation menuNavigation;

    [Tooltip("Time (in seconds) to display credits pop up.")]
    [SerializeField] private float creditsDuration;

    // Reference to the buttons.
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    // Reference to credits panel.
    [SerializeField] private GameObject creditsPanel;

    void Start()
    {
        // Get the instance of MenuNavigation.
        menuNavigation = MenuNavigation.instance;

        // Check if the instance is available and assign button listeners.
        if (menuNavigation != null)
        {
            // Dynamically assign the button functions from the MenuNavigation Singleton.
            if (startButton != null)
            {
                startButton.onClick.AddListener(menuNavigation.StartGame);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(menuNavigation.ExitToDesktop);
            }

            if (creditsButton != null)
            {
                creditsButton.onClick.AddListener(DisplayCredits);
            }
        }
        else
        {
            Debug.LogError("MenuNavigation instance not found!");
        }
    }

    // Activates credits a starts active countdown.
    private void DisplayCredits()
    {
        // Show the credits panel.
        creditsPanel.SetActive(true);

        // Start the coroutine to hide credits after a delay.
        StartCoroutine(HideCreditsAfterDelay());
    }

    // Hides the credits popup after creditsDuration seconds.
    private IEnumerator HideCreditsAfterDelay()
    {
        // Wait for 4 seconds.
        yield return new WaitForSeconds(creditsDuration);

        // Hide the credits panel after the delay.
        creditsPanel.SetActive(false);
    }
}
