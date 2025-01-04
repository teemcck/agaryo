using UnityEngine;
using UnityEngine.UI;

// This script attatches listeners to menu scene buttons during runtime
// so a single MenuNavigation instance can persist across all scenes.
public class MenuSceneUIInitializer : MonoBehaviour
{
    // Reference to MenuNavigation Singleton
    private MenuNavigation menuNavigation;

    // Reference to the buttons
    public Button startButton;
    public Button quitButton;

    void Start()
    {
        // Get the instance of MenuNavigation
        menuNavigation = MenuNavigation.instance;

        // Check if the instance is available and assign button listeners
        if (menuNavigation != null)
        {
            // Dynamically assign the button functions from the MenuNavigation Singleton
            if (startButton != null)
            {
                startButton.onClick.AddListener(menuNavigation.StartGame);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(menuNavigation.ExitToDesktop);
            }
        }
        else
        {
            Debug.LogError("MenuNavigation instance not found!");
        }
    }
}
