 // LandingPage.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LandingPage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int sceneID;
    public GameObject whiteShadow;

    public void onStartClick()
    {
        ChangeSceneManager.Instance.onChangeScene(sceneID);
        Debug.Log("Start button clicked!");
    }

    // Called when the pointer enters the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show white shadow on hover
        if (UIFeedbackManager.Instance != null)
        {
            UIFeedbackManager.Instance.ShowWhiteShadow(whiteShadow);
        }
        else
        {
            // Fallback if manager isn't available
            whiteShadow.SetActive(true);
        }

        Debug.Log("Pointer entered!");
    }

    // Called when the pointer exits the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide white shadow when no longer hovering
        if (UIFeedbackManager.Instance != null)
        {
            UIFeedbackManager.Instance.HideWhiteShadow();
        }
        else
        {
            // Fallback if manager isn't available
            whiteShadow.SetActive(false);
        }

        Debug.Log("Pointer exited!");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        #if UNITY_EDITOR
        // If we're running in the editor
        { 
            UnityEditor.EditorApplication.isPlaying = false; 
        }
        #else
        // If we're running in a standalone build
        { 
            Application.Quit(); 
        }
        #endif

    }
}