using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneManager : MonoBehaviour
{
    public static ChangeSceneManager Instance;

    // Static variables to pass data between scenes
    public static Dictionary<string, object> sceneParameters = new Dictionary<string, object>();

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void onChangeScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
        Debug.Log("Change Scene!");
    }

    public void onChangeScene(int sceneID, string parameterName, object parameterValue)
    {
        // Store parameter
        sceneParameters[parameterName] = parameterValue;

        Debug.Log($"Changing to scene {sceneID} with parameter: {parameterName} = {parameterValue}");
        SceneManager.LoadScene(sceneID);
    }

    // Get parameter in the new scene
    public static T GetSceneParameter<T>(string parameterName, T defaultValue = default(T))
    {
        if (sceneParameters.ContainsKey(parameterName))
        {
            try
            {
                return (T)sceneParameters[parameterName];
            }
            catch (System.InvalidCastException)
            {
                Debug.LogError($"Parameter '{parameterName}' could not be cast to type {typeof(T)}");
                return defaultValue;
            }
        }

        Debug.LogWarning($"Parameter '{parameterName}' not found, using default value: {defaultValue}");
        return defaultValue;
    }

    // Check if parameter exists
    public static bool HasSceneParameter(string parameterName)
    {
        return sceneParameters.ContainsKey(parameterName);
    }
}
