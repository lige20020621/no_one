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
    public GameObject dialoguePanel;
    public Text dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.05f;

    // The good ending text - editable in inspector
    [TextArea(5, 10)]
    public string endingText = "當糯米睜開眼睛的時候,看到的是陌生的天花板...然後身邊傳來的是爸爸媽媽説話的聲音,急切地關心在糯米耳邊傳來\n好像做了一場怪夢...糯米想著糯米撲向爸爸媽媽的懷抱嘟囔著想要多陪陪自己,不想要一個人\n爸爸媽媽們似乎也意識到了錯誤：\"對不起寶貝...以後再也不會沒有人陪著你啦...\"";
    
    private bool isTyping = false;
    private SimpleEyeOpeningEffect eyeEffect;


    [Header("Typing Audio")]
    public AudioClip typingSound;
    public AudioSource typingAudioSource;
    public float typingVolume = 0.2f;
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
        yield return StartCoroutine(ShowDialogue());
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

    IEnumerator ShowDialogue()
    {
        isTyping = true;

        if (dialogueText != null)
        {
            dialogueText.text = "";
            // Start typing sound
            StartTypingSound();

            foreach (char c in endingText)
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
                // Skip typing
                StopAllCoroutines();
                if (dialogueText != null)
                {
                    dialogueText.text = endingText;
                }
                isTyping = false;
            }
            else
            {
                // Close game or go to menu
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
            }
        }
    }
}