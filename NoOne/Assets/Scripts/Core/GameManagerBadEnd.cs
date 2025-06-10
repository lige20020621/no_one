using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManagerBadEnd : MonoBehaviour
{
    [Header("Dialogue System")]
    public GameObject dialoguePanel;
    public Text dialogueText;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f;

    private bool isDisplayingText = false;
    private Coroutine typingCoroutine;
    private string receivedContent = "";

    [Header("Quit Settings")]
    public bool autoQuitAfterDialogue = true;
    public float quitDelay = 2f; // Delay before quitting

    void Start()
    {
        // Receive and process parameters from previous scene
        ProcessSceneParameters();
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

            foreach (char letter in text.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        isDisplayingText = false;
        typingCoroutine = null;
    }

    void Update()
    {
        // Handle input to advance or skip dialogue
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

        // Auto quit after showing ending
        if (autoQuitAfterDialogue)
        {
            StartCoroutine(QuitGameAfterDelay());
        }
    }

    IEnumerator QuitGameAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        Application.Quit();
    }

}