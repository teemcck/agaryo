using UnityEngine;
using UnityEngine.UI;

// This script attatches listeners to menu scene buttons during runtime
// so a single MenuNavigation instance can persist across all scenes.
public class EndSceneUIInitializer : MonoBehaviour
{
    // Reference to MenuNavigation Singleton
    private MenuNavigation menuNavigation;

    // Reference to the buttons
    public Button returnButton;
    public Button quitButton;

    void Start()
    {
        // Get the instance of MenuNavigation
        menuNavigation = MenuNavigation.instance;

        // Check if the instance is available and assign button listeners
        if (menuNavigation != null)
        {
            if (returnButton != null)
            {
                returnButton.onClick.AddListener(menuNavigation.ReturnToMenu);
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
