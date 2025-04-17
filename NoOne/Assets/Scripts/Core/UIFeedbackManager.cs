using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFeedbackManager : MonoBehaviour
{
    public static UIFeedbackManager Instance { get; private set; }

    private GameObject currentShadow;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Debug.Log("UIFeedbackManager Awake");
    }

    // In UIFeedbackManager class
    public void ShowWhiteShadow(GameObject shadow)
    {
        shadow.SetActive(true);
        currentShadow = shadow;
        Debug.Log("ShowWhiteShadow");
    }

    // Change from private to public
    public void HideWhiteShadow()
    {
        if (currentShadow != null)
        {
            currentShadow.SetActive(false);
            currentShadow = null;
        }
    }
}
