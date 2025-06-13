using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManagerBadEnd : MonoBehaviour
{
    [Header("Dialogue System")]
    public GameObject dialoguePanel;
    public Text dialogueText;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.1f;

    private bool isDisplayingText = false;
    private Coroutine typingCoroutine;
    private string receivedContent = "";

    [Header("Quit Settings")]
    public bool autoQuitAfterDialogue = true;
    public float quitDelay = 2f; // Delay before quitting


    [Header("Audio")]
    public AudioClip backgroundMusic;
    public AudioSource audioSource;
    public float backgroundMusicVolume = 0.3f;

    [Header("Typing Audio")]
    public AudioClip typingSound;
    public AudioSource typingAudioSource;
    public float typingVolume = 0.5f;
    public bool playTypingSoundOnEveryChar = false; // If false, plays continuously while typing

    [Header("Next Block Audio")]
    public AudioClip nextBlockSound;
    public AudioSource nextBlockAudioSource;
    public float nextBlockVolume = 0.5f;

    [Header("UI")]
    public GameObject quitButton; // Reference to the quit button

    void Start()
    {

        // Setup audio
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Receive and process parameters from previous scene
        ProcessSceneParameters();
        SetupAudio();
    }

    void SetupAudio()
    {
        // Play background music
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.volume = backgroundMusicVolume;
            audioSource.Play();
        }

        // Setup typing audio source
        if (typingAudioSource == null)
        {
            GameObject typingGO = new GameObject("TypingAudioSource");
            typingGO.transform.SetParent(transform);
            typingAudioSource = typingGO.AddComponent<AudioSource>();
        }
        typingAudioSource.loop = true; // For continuous typing sound
        typingAudioSource.volume = typingVolume;

        // Setup next block audio source
        if (nextBlockAudioSource == null)
        {
            GameObject nextBlockGO = new GameObject("NextBlockAudioSource");
            nextBlockGO.transform.SetParent(transform);
            nextBlockAudioSource = nextBlockGO.AddComponent<AudioSource>();
        }
        nextBlockAudioSource.loop = false;
        nextBlockAudioSource.volume = nextBlockVolume;

    }

    void PlayNextBlockSound()
    {
        if (nextBlockSound != null && nextBlockAudioSource != null)
        {
            nextBlockAudioSource.PlayOneShot(nextBlockSound);
        }
    }

    void StartTypingSound()
    {
        if (typingSound != null && typingAudioSource != null)
        {
            if (!playTypingSoundOnEveryChar)
            {
                // Play continuous typing sound
                typingAudioSource.clip = typingSound;
                typingAudioSource.Play();
            }
        }
    }

    void StopTyping()
    {
        if (typingAudioSource != null && typingAudioSource.isPlaying)
        {
            typingAudioSource.Stop();
        }
    }

    void ProcessSceneParameters()
    {
        // Get content from previous scene
        receivedContent = ChangeSceneManager.GetSceneParameter<string>("content", "");
        Debug.Log($"Bad End Scene received: Content={receivedContent}");

        // Show the content as dialogue if it exists
        if (!string.IsNullOrEmpty(receivedContent))
        {
            // Start dialogue after a short delay
            Invoke("StartBadEndDialogue", 0.5f);
        }
        else
        {
            Debug.LogWarning("No content received from previous scene");
        }
    }

    void StartBadEndDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            // Start typing the received content
            StartTypingText(receivedContent);
        }
        else
        {
            Debug.LogError("GameManagerBadEnd: Dialogue panel not assigned!");
        }
    }

    void StartTypingText(string text)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        isDisplayingText = true;

        if (dialogueText != null)
        {
            dialogueText.text = "";
            // Start typing sound
            StartTypingSound();

            foreach (char letter in text.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            // Stop typing sound
            StopTyping();
        }

        isDisplayingText = false;
        typingCoroutine = null;
    }

    void Update()
    {
        // Handle input to advance or skip dialogue
        if (dialoguePanel != null && dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            PlayNextBlockSound();
            if (isDisplayingText)
            {
                // Skip typing and show full text
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }

                if (dialogueText != null)
                {
                    dialogueText.text = receivedContent;
                }

                isDisplayingText = false;
            }
            else
            {
                // End dialogue and show ending content
                EndDialogue();
            }
        }
    }

    void EndDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        quitButton.SetActive(true);
        // Auto quit after showing ending
        //if (autoQuitAfterDialogue)
        //{
        //    StartCoroutine(QuitGameAfterDelay());
        //}
    }
    void QuitGame()
    {
        Debug.Log("GameManagerBadEnd: Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void QuitImmediately()
    {
        QuitGame();
    }

}