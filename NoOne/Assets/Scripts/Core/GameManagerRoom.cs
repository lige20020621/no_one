using UnityEngine;
using UnityEngine.UI;

public class GameManagerRoom : MonoBehaviour
{

    [Header("Audio")]

    public AudioClip backgroundMusic00;
    public AudioSource audioSource00;
    public float backgroundMusicVolume = 0.3f;

    void Start()
    {
        // Setup audio
        if (audioSource00 == null)
            audioSource00 = gameObject.AddComponent<AudioSource>();

        // Play background music
        if (backgroundMusic00 != null)
        {
            audioSource00.clip = backgroundMusic00;
            audioSource00.loop = true;
            audioSource00.volume = backgroundMusicVolume;
            audioSource00.Play();
        }
    }
}