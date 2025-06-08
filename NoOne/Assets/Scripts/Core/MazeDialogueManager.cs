using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Level3DialogueBlock
{
    public string text;                  // ????
    public SpeakerType speaker;          // ????
    public string speakerName;           // ???????????????
    public float displaySpeed = 0.05f;   // ??????????
}

public class MazeDialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Text speakerNameText;

    private Level3DialogueBlock[] currentDialogue;
    private int currentBlockIndex = 0;
    private bool isDisplayingText = false;
    private System.Action onDialogueComplete;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public void StartDialogue(Level3DialogueBlock[] dialogue, System.Action onComplete = null)
    {
        currentDialogue = dialogue;
        currentBlockIndex = 0;
        onDialogueComplete = onComplete;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        DisplayCurrentDialogueBlock();
    }

    void DisplayCurrentDialogueBlock()
    {
        if (currentBlockIndex < currentDialogue.Length)
        {
            Level3DialogueBlock block = currentDialogue[currentBlockIndex];

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
            EndDialogue();
        }
    }

    IEnumerator TypeText(string text, float typingSpeed)
    {
        isDisplayingText = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isDisplayingText = false;
        typingCoroutine = null;
    }

    void Update()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (isDisplayingText)
            {
                // Skip typing and show full text
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }
                dialogueText.text = currentDialogue[currentBlockIndex].text;
                isDisplayingText = false;
            }
            else
            {
                // Move to next dialogue block
                currentBlockIndex++;
                DisplayCurrentDialogueBlock();
            }
        }
    }

    void EndDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Invoke completion callback
        onDialogueComplete?.Invoke();
    }
}