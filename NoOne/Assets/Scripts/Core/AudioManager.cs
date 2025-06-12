using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClickSound;
    public AudioClip footstepSound;

    private AudioSource audioSource;

    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Play background music on start
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;  // Loop the music
        audioSource.volume = 0.5f;
        audioSource.Play();
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayButtonClick()
    {
        PlaySoundEffect(buttonClickSound);
    }
}