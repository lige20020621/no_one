// DialogueManager.cs - 對話管理器
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameManagerLevel2 : MonoBehaviour
{
    public static GameManagerLevel2 instance;

    [Header("UI References")]
    public GameObject actionPanel;
    public Button yesButton;
    public Button noButton;
    public GameObject dialoguePanel;

    private int currentDialogueIndex = 0;
    private PlayerControllerLevel02 playerController;

    [Header("對話系統引用")]
    public DialogueManagerLevel02 dialogueManager; // 引用您的DialogueManagerLevel02

    [Header("收集完成後的行為")]
    public bool startDialogueAfterCollection = true; // 收集完成後是否開始對話


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Simplified null checks for IDE0031
        dialogueManager ??= FindObjectOfType<DialogueManagerLevel02>();

        playerController = FindObjectOfType<PlayerControllerLevel02>();

        // Setup UI
        if (actionPanel != null)
            actionPanel.SetActive(false);

        // Setup button events
        if (yesButton != null)
            yesButton.onClick.AddListener(OnYesClicked);

        if (noButton != null)
            noButton.onClick.AddListener(OnNoClicked);
    }

    public void ShowActionPanel()
    {
        if (actionPanel != null)
        {
            actionPanel.SetActive(true);

            // Disable player movement while panel is shown
            if (playerController != null)
                playerController.DisableMovement();
        }
    }

    public void OnYesClicked()
    {
        // Hide action panel  
        if (actionPanel != null)
            actionPanel.SetActive(false);

        // Start dialogue  
        if (dialogueManager != null)
        {
            dialogueManager.StartSecondaryDialogue();
        }
    }

    public void OnNoClicked()
    {
        // Hide action panel and resume game
        if (actionPanel != null)
            actionPanel.SetActive(false);

        if (playerController != null)
            playerController.EnableMovement();
    }
}