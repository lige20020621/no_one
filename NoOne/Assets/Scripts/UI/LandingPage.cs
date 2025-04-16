 // LandingPage.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandingPage : MonoBehaviour
{

    public int sceneID;
    public GameObject whiteShadow;
    public void onStartClick()
    {
        ChangeSceneManager.Instance.onChangeScene(sceneID);
        UIFeedbackManager.Instance.ShowWhiteShadow(whiteShadow);
        Debug.Log("Start button clicked!");
    }
}