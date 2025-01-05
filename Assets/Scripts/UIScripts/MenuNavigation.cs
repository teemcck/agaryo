using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField, Tooltip("Stores index of menu scene (available in build settings).")]
    private int menuSceneIndex;

    [SerializeField, Tooltip("Stores index of game scene (available in build settings).")]
    private int gameSceneIndex;

    [SerializeField, Tooltip("Stores index of end game scene (available in build settings).")]
    private int endSceneIndex;

    [SerializeField, Tooltip("Stores index of win scene (available in build settings).")]
    private int winSceneIndex;

    // Singleton pattern to make sure there is only one instance of MenuNavigation.
    public static MenuNavigation instance;

    void Awake()
    {
        // Ensure that the object persists across scenes.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed on scene load.
        }
        else
        {
            Destroy(gameObject); // If instance already exists, destroy this object.
        }
    }

    // Sends player to the game scene.
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }

    // Sends player to the main menu scene.
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneIndex);
    }

    // Sends player to the death scene.
    public void SendToEndScreen()
    {
        SceneManager.LoadScene(endSceneIndex);
    }

    // Sends player to the win scene.
    public void SendToWinScreen()
    {
        SceneManager.LoadScene(winSceneIndex);
    }

    // Exits application.
    public void ExitToDesktop()
    {
        Application.Quit();
    }
}
