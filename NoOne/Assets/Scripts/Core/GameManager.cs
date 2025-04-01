// GameManager.cs
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    // Game state
    private bool isGameRunning = false;

    private void Awake()
    {
        // Ensure singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        isGameRunning = true;
        Debug.Log("Game started!");
        
        // Initialize game-specific systems and variables here
    }

    public void EndGame()
    {
        isGameRunning = false;
        Debug.Log("Game ended!");
        
        // Handle end game logic
        
        // Return to landing page
        PageController.Instance.NavigateToPage(PageController.PageType.Landing);
    }

    // Check if the game is currently running
    public bool IsGameRunning()
    {
        return isGameRunning;
    }
}