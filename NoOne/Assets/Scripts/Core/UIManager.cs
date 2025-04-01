// UIManager.cs
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Singleton pattern
    public static UIManager Instance { get; private set; }

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

    public void OnPageChanged(PageController.PageType pageType)
    {
        // Inform specific page controllers or handle UI state changes
        switch (pageType)
        {
            case PageController.PageType.Landing:
                // Do any specific setup for landing page
                Debug.Log("Navigated to Landing Page");
                break;
            case PageController.PageType.Room:
                // Do any specific setup for game page
                Debug.Log("Navigated to Room Page");
                break;
            // Add more cases as needed
        }
    }
}