// LandingPage.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LandingPage : MonoBehaviour
{
    public void onStartClick(int SceneID)
    {
        SceneManager.LoadScene(SceneID);
        Debug.Log("Start button clicked!");
    }

    private void Start()
    {}

    private void Update()
    {}

    private void OnDestroy()
    {

    }
}