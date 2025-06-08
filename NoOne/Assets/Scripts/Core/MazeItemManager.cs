// MazeItemManager.cs - Manages all 4 items and their dialogues
using UnityEngine;

public class MazeItemManager : MonoBehaviour
{
    [Header("Item Collection")]
    public ItemCollectable[] items = new ItemCollectable[4]; // References to the 4 items

    [Header("Dialogue System")]
    public MazeDialogueManager dialogueManager;

    private bool[] itemsCollected = new bool[4]; // Track which items are collected
    private int totalItemsCollected = 0;

    // Define dialogues for each item
    private Level3DialogueBlock[][] itemDialogues;

    void Start()
    {
        // Initialize dialogue system
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<MazeDialogueManager>();
        }

        // Initialize dialogues for each item
        InitializeItemDialogues();

        // Assign item IDs if not set
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                items[i].itemID = i;
            }
        }
    }

    void InitializeItemDialogues()
    {
        itemDialogues = new Level3DialogueBlock[4][];

        // Item 0 - ????? (NPC1)
        itemDialogues[0] = new Level3DialogueBlock[]
        {
            new Level3DialogueBlock
            {
                text = "????...??????...",
                speaker = SpeakerType.Player,
                speakerName = "??"
            },
            new Level3DialogueBlock
            {
                text = "??...???????...\n?????...????...",
                speaker = SpeakerType.NPC1,
                speakerName = "?????"
            }
        };

        // Item 1 - ?? (NPC2) 
        itemDialogues[1] = new Level3DialogueBlock[]
        {
            new Level3DialogueBlock
            {
                text = "????????????...",
                speaker = SpeakerType.Player,
                speakerName = "??"
            },
            new Level3DialogueBlock
            {
                text = "?...??????????...\n??????????...",
                speaker = SpeakerType.NPC2,
                speakerName = "??"
            }
        };

        // Item 2 - ?? (NPC3)
        itemDialogues[2] = new Level3DialogueBlock[]
        {
            new Level3DialogueBlock
            {
                text = "??...?????",
                speaker = SpeakerType.Player,
                speakerName = "??"
            },
            new Level3DialogueBlock
            {
                text = "????????...\n????????????...",
                speaker = SpeakerType.NPC3,
                speakerName = "??"
            }
        };

        // Item 3 - ?? (NPC4)
        itemDialogues[3] = new Level3DialogueBlock[]
        {
            new Level3DialogueBlock
            {
                text = "???????????...",
                speaker = SpeakerType.Player,
                speakerName = "??"
            },
            new Level3DialogueBlock
            {
                text = "??...???????????...\n?????????????...",
                speaker = SpeakerType.NPC4,
                speakerName = "??"
            }
        };
    }

    public void OnItemCollected(ItemCollectable item, GameObject player)
    {
        int itemID = item.itemID;

        // Check if item ID is valid
        if (itemID < 0 || itemID >= 4 || itemsCollected[itemID])
        {
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
        }

        // Start dialogue for this item
        if (dialogueManager != null && itemDialogues[itemID] != null)
        {
            dialogueManager.StartDialogue(itemDialogues[itemID], () => {
                // Re-enable player movement after dialogue
                if (playerController != null)
                {
                    playerController.EnableMovement();
                }

                // Check if all items are collected
                CheckAllItemsCollected();
            });
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