using UnityEngine;
using UnityEngine.UI;

public class GameManagerLanding : MonoBehaviour
{

    [Header("Audio")]
   
    public AudioClip backgroundMusic;
    public AudioSource audioSource;
    public float backgroundMusicVolume = 0.3f;

    void Start()
    {
        // Setup audio
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Play background music
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.volume = backgroundMusicVolume;
            audioSource.Play();
        }
    }
}