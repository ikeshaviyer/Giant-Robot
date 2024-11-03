using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Implement the singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    private void Start()
    {
        // Listen for scene changes to reassign button listeners after each scene load
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Clean up listener when destroyed
    }

    // Method to load a scene by name
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Method to quit the application
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting..."); // Log for confirmation in the editor
    }

    // Called automatically after a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignButtonListeners(); // Reassign button listeners for the new scene
    }

    // Method to find and assign button listeners after each scene load
    private void AssignButtonListeners()
    {
        Button backButton = GameObject.Find("BackButton")?.GetComponent<Button>();
        Button settingsButton = GameObject.Find("SettingsButton")?.GetComponent<Button>();

        // Set up listeners for buttons if they exist in the scene
        if (backButton != null)
            backButton.onClick.AddListener(() => LoadScene("Start"));

        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => LoadScene("Settings"));
    }
}
