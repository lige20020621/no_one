using UnityEngine;

/// <summary>
/// Attach this script to trigger areas that should activate dialogues when the player enters them
/// and presses the interaction key.
/// </summary>
public class DialogueTriggerArea : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [Tooltip("ID of the NPC/dialogue to trigger (1=NPC1, 2=NPC2, 3=NPC Conversation)")]
    public int npcId = 1;

    [Header("Interaction")]
    [Tooltip("Key the player needs to press to start the dialogue")]
    public KeyCode interactionKey = KeyCode.E;

    [Tooltip("Should dialogue start automatically when player enters the trigger area?")]
    public bool autoActivate = false;

    [Header("Visual Indicators")]
    [Tooltip("Optional visual cue that player can interact (like an '!' or 'E' icon)")]
    public GameObject interactionIndicator;

    [Tooltip("Optional tooltip text UI element")]
    public GameObject tooltipText;

    [Header("Debug")]
    [Tooltip("Enable to show debug messages in the console")]
    public bool showDebugMessages = true;

    // Private variables
    private bool isPlayerInRange = false;
    private NewDialogueManager dialogueManager;
    private bool hasInteracted = false; // Prevent multiple activations

    private void Start()
    {
        // Find the dialogue manager in the scene
        dialogueManager = FindObjectOfType<NewDialogueManager>();

        // Check for required components
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError("DialogueTriggerArea requires a Collider2D component! Add a BoxCollider2D and set it as a trigger.");
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector2(2f, 2f); // Default size
        }
        else if (!GetComponent<Collider2D>().isTrigger)
        {
            Debug.LogWarning("DialogueTriggerArea's Collider2D should be set as a trigger! Setting it automatically.");
            GetComponent<Collider2D>().isTrigger = true;
        }

        // ADDED: Ensure we have a Rigidbody2D for trigger detection
        if (GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogWarning("DialogueTriggerArea requires a Rigidbody2D for trigger detection. Adding one automatically.");
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            rb.useFullKinematicContacts = true;
        }

        // Hide indicators at start
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(false);
        }

        if (tooltipText != null)
        {
            tooltipText.SetActive(false);
        }

        // Log warning if dialogueManager is missing
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager not found! Make sure you have a GameObject with NewDialogueManager component in your scene.");
        }

        if (showDebugMessages)
        {
            Debug.Log($"DialogueTriggerArea for NPC ID {npcId} initialized");
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Trigger detected collision with: " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            hasInteracted = false; // Reset interaction flag when player re-enters

            // Show interaction indicators
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(true);
            }

            if (tooltipText != null)
            {
                tooltipText.SetActive(true);
            }

            if (showDebugMessages)
            {
                Debug.Log($"Player entered trigger area for NPC {npcId}");
            }

            // Auto-activate dialogue if enabled
            if (autoActivate && !hasInteracted)
            {
                TriggerDialogue();
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;

            // Hide interaction indicators
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(false);
            }

            if (tooltipText != null)
            {
                tooltipText.SetActive(false);
            }

            if (showDebugMessages)
            {
                Debug.Log($"Player exited trigger area for NPC {npcId}");
            }
        }
    }

    private void Update()
    {
        // If player is in range, hasn't already interacted, and presses the interaction key
        if (isPlayerInRange && !hasInteracted && Input.GetKeyDown(interactionKey))
        {
            TriggerDialogue();
        }
    }

    /// <summary>
    /// Triggers the dialogue sequence associated with this area's npcId
    /// </summary>
    private void TriggerDialogue()
    {
        if (dialogueManager != null)
        {
            // Hide interaction indicators
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(false);
            }

            if (tooltipText != null)
            {
                tooltipText.SetActive(false);
            }

            // Start dialogue
            dialogueManager.TriggerNPCDialogue(npcId);
            hasInteracted = true;

            if (showDebugMessages)
            {
                Debug.Log($"Triggered dialogue for NPC {npcId}");
            }
        }
        else
        {
            Debug.LogError("DialogueManager not found in the scene! Cannot trigger dialogue.");
        }
    }

    /// <summary>
    /// Manually trigger this dialogue area from another script
    /// </summary>
    public void TriggerDialogueExternal()
    {
        if (!hasInteracted)
        {
            TriggerDialogue();
        }
    }

    /// <summary>
    /// Reset the interaction state, allowing the dialogue to be triggered again
    /// </summary>
    public void ResetInteraction()
    {
        hasInteracted = false;

        // Show interaction indicators if player is still in range
        if (isPlayerInRange)
        {
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(true);
            }

            if (tooltipText != null)
            {
                tooltipText.SetActive(true);
            }
        }

        if (showDebugMessages)
        {
            Debug.Log($"Reset interaction state for NPC {npcId}");
        }
    }

    /// <summary>
    /// Change the NPC ID associated with this trigger area
    /// </summary>
    public void ChangeNpcId(int newNpcId)
    {
        npcId = newNpcId;
        hasInteracted = false; // Reset interaction state

        if (showDebugMessages)
        {
            Debug.Log($"Changed trigger area to NPC {npcId}");
        }
    }

    // Visual debugging to see the trigger area in the editor
    private void OnDrawGizmos()
    {
        // Set the color based on NPC ID
        switch (npcId)
        {
            case 1:
                Gizmos.color = new Color(1, 0, 0, 0.3f); // Red for NPC1
                break;
            case 2:
                Gizmos.color = new Color(0, 0, 1, 0.3f); // Blue for NPC2
                break;
            case 3:
                Gizmos.color = new Color(1, 1, 0, 0.3f); // Yellow for NPC Conversation
                break;
            default:
                Gizmos.color = new Color(0, 1, 0, 0.3f); // Green for other
                break;
        }

        // Use the collider bounds if possible
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            // Different display based on collider type
            if (collider is BoxCollider2D)
            {
                BoxCollider2D boxCollider = collider as BoxCollider2D;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCollider.offset, boxCollider.size);
            }
            else if (collider is CircleCollider2D)
            {
                CircleCollider2D circleCollider = collider as CircleCollider2D;
                Gizmos.DrawSphere(transform.position + new Vector3(circleCollider.offset.x, circleCollider.offset.y, 0),
                                  circleCollider.radius);
            }
            else
            {
                // Fallback for other collider types
                Gizmos.DrawWireSphere(transform.position, 1f);
            }
        }
        else
        {
            // No collider, show default sphere
            Gizmos.DrawWireSphere(transform.position, 1f);
        }

        // Draw line to indicate trigger point
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 1.5f);

        // Add NPC ID label
        Gizmos.color = Color.black;
        // Note: Cannot draw text in OnDrawGizmos, would need OnDrawGizmosSelected and UnityEditor
    }
}