using UnityEngine;
using UnityEngine.UI;

public class GameManagerLevel3 : MonoBehaviour
{

    [Header("Audio")]
   
    public AudioClip backgroundMusic;
    public AudioSource audioSource;
    public float backgroundMusicVolume = 0.3f;

    [Header("Hint")]
    public GameObject hintPanel;

    [Header("Player Control")]
    public MazePlayerController  playerMover;


    void Start()
    {

        SetupAudio();
        SetupPlayer();
    }

    void SetupAudio()
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

    void SetupPlayer()
    {
        // Find player automatically if not assigned
        if (playerMover == null)
        {
            playerMover = FindObjectOfType<MazePlayerController>();

            if (playerMover == null)
            {
                Debug.LogWarning("GameManagerLevel3: No PlayerMover found in scene!");
                return;
            }
        }

        // Disable player movement at start if hint is shown
        if (playerMover != null)
        {
            playerMover.DisableMovement();
            Debug.Log("GameManagerLevel3: Player movement disabled for hint display");
        }
    }

    void Update()
    {
        // Handle spacebar input for hint panel
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HideHint();
        }
    }

    public void HideHint()
    {
        if (hintPanel != null )
        {
            hintPanel.SetActive(false);
            // Enable player movement when hiding hint
            if (playerMover != null)
            {
                playerMover.EnableMovement();
            }

            Debug.Log("GameManagerLevel3: Hint panel hidden, player movement enabled");
        }
    }
}