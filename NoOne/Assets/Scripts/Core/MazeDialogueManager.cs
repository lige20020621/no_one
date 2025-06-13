using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Level3DialogueBlock
{
    public string text;                  // 對話內容
    public SpeakerType speaker;          // 誰在說話
    public string speakerName;           // 說話者的名字（可選，用於顯示）
    public float displaySpeed = 0.1f;   // 文字顯示速度（可選）
}

public class MazeDialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Text speakerNameText;

    [Header("Debug Settings")]
    public bool enableDebugLogs = true;

    private Level3DialogueBlock[] currentDialogue;
    private int currentBlockIndex = 0;
    private bool isDisplayingText = false;
    private System.Action onDialogueComplete;
    private Coroutine typingCoroutine;


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
            dialoguePanel.SetActive(false);
            if (enableDebugLogs) Debug.Log("MazeDialogueManager: Dialogue panel initialized and hidden");
        }
        else
        {
            Debug.LogError("MazeDialogueManager: Dialogue panel is not assigned!");
        }

        if (dialogueText == null)
        {
            Debug.LogError("MazeDialogueManager: Dialogue text is not assigned!");
        }

        if (speakerNameText == null)
        {
            Debug.LogError("MazeDialogueManager: Speaker name text is not assigned!");
        }

        SetupAudio();
    }

    void SetupAudio()
    {
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

    public void StartDialogue(Level3DialogueBlock[] dialogue, System.Action onComplete = null)
    {
        if (enableDebugLogs) Debug.Log($"MazeDialogueManager: StartDialogue called with {dialogue?.Length ?? 0} dialogue blocks");

        if (dialogue == null || dialogue.Length == 0)
        {
            Debug.LogError("MazeDialogueManager: Dialogue array is null or empty!");
            return;
        }

        currentDialogue = dialogue;
        currentBlockIndex = 0;
        onDialogueComplete = onComplete;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            if (enableDebugLogs) Debug.Log("MazeDialogueManager: Dialogue panel activated");
        }
        else
        {
            Debug.LogError("MazeDialogueManager: Cannot show dialogue - panel is null!");
            return;
        }

        DisplayCurrentDialogueBlock();
    }

    void PlayNextBlockSound()
    {
        if (nextBlockSound != null && nextBlockAudioSource != null)
        {
            // Stop typing sound
            StopTyping();
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

    void DisplayCurrentDialogueBlock()
    {
        if (currentBlockIndex < currentDialogue.Length)
        {
            Level3DialogueBlock block = currentDialogue[currentBlockIndex];

            if (enableDebugLogs) Debug.Log($"MazeDialogueManager: Displaying block {currentBlockIndex}: '{block.text}' by {block.speakerName}");

            // Update speaker name
            if (speakerNameText != null)
            {
                speakerNameText.text = block.speakerName;
            }

            // Start typing text
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText(block.text, block.displaySpeed));
        }
        else
        {
            if (enableDebugLogs) Debug.Log("MazeDialogueManager: All dialogue blocks completed, ending dialogue");
            EndDialogue();
        }
    }

    IEnumerator TypeText(string text, float typingSpeed)
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
        else
        {
            Debug.LogError("MazeDialogueManager: Cannot display text - dialogueText is null!");
        }

        isDisplayingText = false;
        typingCoroutine = null;
    }

    void Update()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (enableDebugLogs) Debug.Log("MazeDialogueManager: Space key pressed during dialogue");
            PlayNextBlockSound();
            if (isDisplayingText)
            {
                // Skip typing and show full text
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }

                if (dialogueText != null && currentDialogue != null && currentBlockIndex < currentDialogue.Length)
                {
                    dialogueText.text = currentDialogue[currentBlockIndex].text;
                }

                isDisplayingText = false;
                if (enableDebugLogs) Debug.Log("MazeDialogueManager: Skipped typing animation");
            }
            else
            {
                // Move to next dialogue block
                currentBlockIndex++;
                if (enableDebugLogs) Debug.Log($"MazeDialogueManager: Moving to next dialogue block: {currentBlockIndex}");
                DisplayCurrentDialogueBlock();
            }
        }
    }

    void EndDialogue()
    {
        if (enableDebugLogs) Debug.Log("MazeDialogueManager: EndDialogue called");

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Invoke completion callback
        if (onDialogueComplete != null)
        {
            if (enableDebugLogs) Debug.Log("MazeDialogueManager: Invoking dialogue completion callback");
            onDialogueComplete.Invoke();
        }
        else
        {
            if (enableDebugLogs) Debug.Log("MazeDialogueManager: No completion callback to invoke");
        }
    }
}