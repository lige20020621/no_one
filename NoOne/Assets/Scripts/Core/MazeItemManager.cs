// MazeItemManager.cs - Manages all 4 items and their dialogues
using UnityEngine;

public class MazeItemManager : MonoBehaviour
{
    [Header("Item Collection")]
    public ItemCollectable[] items = new ItemCollectable[4]; // References to the 4 items

    [Header("Dialogue System")]
    public MazeDialogueManager dialogueManager;


    [Header("Question Panel")]
    public GameObject questionPanel; // Panel that shows after brain dialogue
    public UnityEngine.UI.Button yesButton;
    public UnityEngine.UI.Button noButton; 

    [Header("Debug Settings")]
    public bool enableDebugLogs = true;

    private bool[] itemsCollected = new bool[4]; // Track which items are collected
    private int totalItemsCollected = 0;

    // Define dialogues for each item
    private Level3DialogueBlock[][] itemDialogues;

    void Start()
    {
        // Initialize dialogue system
        if (enableDebugLogs) Debug.Log("MazeItemManager: Starting initialization");

        // Initialize dialogue system
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<MazeDialogueManager>();
            if (enableDebugLogs) Debug.Log($"MazeItemManager: Found dialogue manager: {dialogueManager != null}");
        }

        if (dialogueManager == null)
        {
            Debug.LogError("MazeItemManager: No MazeDialogueManager found! Please assign one or add to scene.");
        }

        // Hide question panel initially
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }

        // Setup button events
        if (yesButton != null)
        {
            yesButton.onClick.AddListener(OnYesClicked);
        }

        if (noButton != null)
        {
            noButton.onClick.AddListener(OnNoClicked);
        }

        // Initialize dialogues for each item
        InitializeItemDialogues();

        // Assign item IDs if not set
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                items[i].itemID = i;
                if (enableDebugLogs) Debug.Log($"MazeItemManager: Set item {i} ID to {i}");
            }
            else
            {
                Debug.LogWarning($"MazeItemManager: Item slot {i} is empty!");
            }
        }

        if (enableDebugLogs) Debug.Log("MazeItemManager: Initialization complete");
    }

    void InitializeItemDialogues()
    {
        itemDialogues = new Level3DialogueBlock[4][];

        // Item 0 - 龜裂的土地 (NPC1)
        itemDialogues[0] = new Level3DialogueBlock[]
        {
            new Level3DialogueBlock
            {
                text = "我懷念..以前...滿眼的綠色光影....",
                speaker = SpeakerType.NPC1,
                speakerName = "龜裂的土地"
            },
            new Level3DialogueBlock
            {
                text = "在這個鋼筋水泥的世界...\n人類的貪婪...終將引來最後的毀滅...",
                speaker = SpeakerType.NPC1,
                speakerName = "龜裂的土地"
            }
        };

        // Item 1 - 怪貓 (NPC2) 
        itemDialogues[1] = new Level3DialogueBlock[]
        { 
            new Level3DialogueBlock
            {
                text = "喵！~&*...喵*￥%#...\n好餓...好痛......\n不要抛下我嗚喵嗚喵嗚...",
                speaker = SpeakerType.NPC2,
                speakerName = "怪貓"
            }
        };

        // Item 2 - 大腦 (NPC3)
        itemDialogues[2] = new Level3DialogueBlock[]
        {
            new Level3DialogueBlock
            {
                text = "嘿小家夥...你好哇...這裏看起來真糟糕...你説是吧？",
                speaker = SpeakerType.NPC3,
                speakerName = "大腦"
            },
            new Level3DialogueBlock
            {
                text = "我可以帶你去一個很棒很棒的樂園哦.....那裏有享用不完的美食和金錢~",
                speaker = SpeakerType.NPC3,
                speakerName = "大腦"
            },
            new Level3DialogueBlock
            {
                text = "你要抛下一切跟我來嗎？反正...你應該也回不去了...",
                speaker = SpeakerType.NPC3,
                speakerName = "大腦"
            }
        };

        // Item 3 - 巨蛇 (NPC4)
        itemDialogues[3] = new Level3DialogueBlock[]
        {

            new Level3DialogueBlock
            {
                text = "嘿...小家夥...你爲什麽來到這裏？",
                speaker = SpeakerType.NPC4,
                speakerName = "巨蛇"
            },
            new Level3DialogueBlock
            {
                text = "我...我也不知道...",
                speaker = SpeakerType.Player,
                speakerName = "糯米"
            },
             new Level3DialogueBlock
            {
                text = "那...你喜歡這裏嗎？",
                speaker = SpeakerType.NPC4,
                speakerName = "巨蛇"
            },
            new Level3DialogueBlock
            {
                text = "嗚...這個地方很有趣...\n有很多在書中沒看過的...神奇東西...\n但...我想爸爸媽媽了...",
                speaker = SpeakerType.Player,
                speakerName = "糯米"
            },
             new Level3DialogueBlock
            {
                text = "那...你想回家嗎？",
                speaker = SpeakerType.NPC4,
                speakerName = "巨蛇"
            },
            new Level3DialogueBlock
            {
                text = "想.......\n我想回去和爸爸媽媽一起...\n一起做很多很多事...",
                speaker = SpeakerType.Player,
                speakerName = "糯米"
            },
              new Level3DialogueBlock
            {
                text = "我想回去告訴爸爸媽媽...\n希望他們可以陪陪我...\n我討厭沒有人...",
                speaker = SpeakerType.Player,
                speakerName = "糯米"
            },
                new Level3DialogueBlock
            {
                text = "你長大了呢...會説出自己想法了...\n別哭了，小家夥...我送你回家",
                speaker = SpeakerType.NPC4,
                speakerName = "巨蛇"
            },
        };
    }

    public void OnItemCollected(ItemCollectable item, GameObject player)
    {
        int itemID = item.itemID;

        if (enableDebugLogs) Debug.Log($"MazeItemManager: OnItemCollected called for item ID {itemID}");

        // Check if item ID is valid
        if (itemID < 0 || itemID >= 4)
        {
            Debug.LogError($"MazeItemManager: Invalid item ID: {itemID}");
            return;
        }

        if (itemsCollected[itemID])
        {
            if (enableDebugLogs) Debug.Log($"MazeItemManager: Item {itemID} already collected, ignoring");
            return;
        }

        // Mark item as collected
        itemsCollected[itemID] = true;
        totalItemsCollected++;

        Debug.Log($"Item {itemID} collected! Total: {totalItemsCollected}/4");

        // Disable player movement during dialogue
        MazePlayerController playerController = player.GetComponent<MazePlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
            if (enableDebugLogs) Debug.Log("MazeItemManager: Player movement disabled");
        }
        else
        {
            Debug.LogWarning("MazeItemManager: No MazePlayerController found on player!");
        }

        // Start dialogue for this item
        if (dialogueManager != null && itemDialogues[itemID] != null)
        {
            if (enableDebugLogs) Debug.Log($"MazeItemManager: Starting dialogue for item {itemID}");

            dialogueManager.StartDialogue(itemDialogues[itemID], () => {
                if (enableDebugLogs) Debug.Log("MazeItemManager: Dialogue completed, re-enabling player movement");

                // Special handling for brain (item ID 2)
                if (itemID == 2)
                {
                    ShowQuestionPanel();
                }
                else
                {
                    // Re-enable player movement for other items
                    if (playerController != null)
                    {
                        playerController.EnableMovement();
                    }
                    CheckAllItemsCollected();
                }
            });
        }
        else
        {
            Debug.LogError($"MazeItemManager: Cannot start dialogue - dialogueManager: {dialogueManager != null}, dialogue exists: {itemDialogues[itemID] != null}");
        }
    }

    void ShowQuestionPanel()
    {
        if (enableDebugLogs) Debug.Log("MazeItemManager: Showing question panel after brain dialogue");

        if (questionPanel != null)
        {
            questionPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("MazeItemManager: Question panel not assigned!");
            // Fallback - just re-enable movement
            MazePlayerController playerController = FindObjectOfType<MazePlayerController>();
            if (playerController != null)
            {
                playerController.EnableMovement();
            }
        }
    }

    void OnYesClicked()
    {
        if (enableDebugLogs) Debug.Log("MazeItemManager: Player clicked YES");

        // Hide question panel
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }

        // Re-enable player movement
        MazePlayerController playerController = FindObjectOfType<MazePlayerController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
        }

        ChangeSceneManager.Instance.onChangeScene(5, "content", "沒有人知道這人是否是真的樂園...你感受到這裏有充足的食物\n花不完的金錢，但你知道這裏少了些東西，沒有人能夠傾聽你的聲音\n最後連你的心的沉寂在這片“樂園”......");
    }

    void OnNoClicked()
    {
        if (enableDebugLogs) Debug.Log("MazeItemManager: Player clicked NO");

        // Hide question panel
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }

        // Re-enable player movement
        MazePlayerController playerController = FindObjectOfType<MazePlayerController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
        }
    }

    void CheckAllItemsCollected()
    {
        if (totalItemsCollected >= 4)
        {
            Debug.Log("All 4 items collected! Maze complete!");
            OnAllItemsCollected();
        }
    }

    void OnAllItemsCollected()
    {
        // Handle what happens when all items are collected
        // You can add final dialogue, scene transition, etc.
        Debug.Log("Maze collection complete! Ready for next phase.");

        // Example: Start final dialogue or change scene
        // ChangeSceneManager.Instance.onChangeScene(nextSceneIndex);
    }

    // Optional: Reset all items
    public void ResetAllItems()
    {
        totalItemsCollected = 0;
        for (int i = 0; i < itemsCollected.Length; i++)
        {
            itemsCollected[i] = false;
        }

        foreach (ItemCollectable item in items)
        {
            if (item != null)
            {
                item.ResetItem();
            }
        }
    }
}