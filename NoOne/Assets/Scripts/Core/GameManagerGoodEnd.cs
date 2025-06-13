using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManagerGoodEnd : MonoBehaviour
{
    [Header("Audio")]

    public AudioClip backgroundMusic;
    public AudioSource audioSource;
    public float backgroundMusicVolume = 0.3f;

    [Header("UI References")]
    public GameObject quitButton; // Reference to the quit button
    public GameObject dialoguePanel;
    public Text dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.1f;
    public float timeBetweenTexts = 1f; // Pause between text segments

    // The good ending text - editable in inspector
    [TextArea(3, 8)]
    public string[] endingTexts = new string[]
    {
        "當糯米睜開眼睛的時候,看到的是陌生的天花板...\n然後身邊傳來的是爸爸媽媽説話的聲音,急切地關心在糯米耳邊傳來\n\"好像做了一場怪夢...\"糯米想著",
        "糯米撲向爸爸媽媽的懷抱嘟囔著想要多陪陪自己\n不想要一個人, 爸爸媽媽們似乎也意識到了錯誤：\n\"對不起寶貝...以後再也不會沒有人陪著你啦...\""
    };

    private bool isTyping = false;
    private SimpleEyeOpeningEffect eyeEffect;
    private int currentTextIndex = 0;
    private bool dialogueComplete = false;

    [Header("Typing Audio")]
    public AudioClip typingSound;
    public AudioSource typingAudioSource;
    public float typingVolume = 0.5f;
    public bool playTypingSoundOnEveryChar = false; // If false, plays continuously while typing

    [Header("Next Block Audio")]
    public AudioClip nextBlockSound;
    public AudioSource nextBlockAudioSource;
    public float nextBlockVolume = 0.5f;

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // Find the eye effect component
        eyeEffect = FindObjectOfType<SimpleEyeOpeningEffect>();
        if (eyeEffect == null)
        {
            // Create one if it doesn't exist
            GameObject effectGO = new GameObject("EyeOpeningEffect");
            eyeEffect = effectGO.AddComponent<SimpleEyeOpeningEffect>();
        }


        SetupAudio(); 
        // Start dialogue automatically
        StartCoroutine(PlaySequence());
    }

    void SetupAudio()
    {
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

    IEnumerator PlaySequence()
    {
        // Start eye opening and wait for it to complete
        yield return StartCoroutine(PlayEyeOpeningEffect());

        // Now start dialogue
        yield return StartCoroutine(ShowAllDialogues());
    }

    IEnumerator PlayEyeOpeningEffect()
    {
        // Calculate total duration of eye opening effect
        float totalDuration = eyeEffect.startDelay + eyeEffect.fadeDuration;

        // Start the effect
        eyeEffect.StartEyeOpening();

        // Wait for it to complete
        yield return new WaitForSeconds(totalDuration);
    }

    IEnumerator ShowAllDialogues()
    {
        // Show each text in the array
        for (int i = 0; i < endingTexts.Length; i++)
        {
            currentTextIndex = i;
            yield return StartCoroutine(ShowSingleDialogue(endingTexts[i]));

            // Wait between texts (except for the last one)
            if (i < endingTexts.Length - 1)
            {
                yield return new WaitForSeconds(timeBetweenTexts);
            }
        }

        dialogueComplete = true;
    }

    IEnumerator ShowSingleDialogue(string textToShow)
    {
        isTyping = true;

        if (dialogueText != null)
        {
            dialogueText.text = "";
            // Start typing sound
            StartTypingSound();

            foreach (char c in textToShow)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }

            // Stop typing sound
            StopTyping();
        }

        isTyping = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayNextBlockSound();

            if (isTyping)
            {
                // Skip current typing
                SkipCurrentTyping();
            }
            else if (!dialogueComplete)
            {
                // If not all dialogues are shown, skip to next
                SkipToNextDialogue();
            }
            else
            {
                // All dialogues complete, exit game

                dialoguePanel.SetActive(false);
                quitButton.SetActive(true);
            }
        }
    }

    void SkipCurrentTyping()
    {
        StopAllCoroutines();
        StopTyping();

        if (dialogueText != null && currentTextIndex < endingTexts.Length)
        {
            dialogueText.text = endingTexts[currentTextIndex];
        }

        isTyping = false;
    }

    void SkipToNextDialogue()
    {
        StopAllCoroutines();
        StopTyping();

        // Show all text immediately and mark as complete
        if (dialogueText != null)
        {
            // Combine all remaining texts
            string allRemainingText = "";
            for (int i = currentTextIndex; i < endingTexts.Length; i++)
            {
                allRemainingText += endingTexts[i];
                if (i < endingTexts.Length - 1)
                {
                    allRemainingText += "\n\n"; // Add spacing between texts
                }
            }
            dialogueText.text = allRemainingText;
        }

        dialogueComplete = true;
        isTyping = false;
    }

    void ExitGame()
    {
        Debug.Log("GameManagerGoodEnd: Ending game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Public methods for external control
    public void SetCurrentTextIndex(int index)
    {
        currentTextIndex = Mathf.Clamp(index, 0, endingTexts.Length - 1);
    }

    public void SkipAllDialogue()
    {
        SkipToNextDialogue();
    }

    public bool IsDialogueComplete()
    {
        return dialogueComplete;
    }

    public int GetCurrentTextIndex()
    {
        return currentTextIndex;
    }

    public int GetTotalTextCount()
    {
        return endingTexts.Length;
    }

    // Method to add text dynamically (optional)
    public void AddEndingText(string newText)
    {
        System.Array.Resize(ref endingTexts, endingTexts.Length + 1);
        endingTexts[endingTexts.Length - 1] = newText;
    }

    void OnDestroy()
    {
        // Clean up audio
        StopAllCoroutines();
        StopTyping();
    }

    public void QuitImmediately()
    {
        QuitGame();
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
}