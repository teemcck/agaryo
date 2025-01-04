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

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneIndex);
    }

    public void SendToEndScreen()
    {
        SceneManager.LoadScene(endSceneIndex);
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }
}
