using UnityEngine;

public class ItemCollectable : MonoBehaviour
{
    [Header("Item Settings")]
    public int itemID; // 0-3 for the 4 different items
    public SpeakerType associatedSpeaker; // Which NPC this item represents
    public string itemName = "Item";

    [Header("Visual Settings")]
    public bool hideAfterCollection = true;
    public bool hasCollectionAnimation = true;

    private bool isCollected = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D itemCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemCollider = GetComponent<Collider2D>();

        // Add trigger collider if not present
        if (itemCollider == null)
        {
            CircleCollider2D trigger = gameObject.AddComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            trigger.radius = 0.5f;
        }
        else
        {
            itemCollider.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if player touched the item
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectItem(other.gameObject);
        }
    }

    void CollectItem(GameObject player)
    {
        if (isCollected) return;

        isCollected = true;
        Debug.Log($"Collected {itemName} (ID: {itemID})");

        // Find the maze manager and notify collection
        MazeItemManager itemManager = FindObjectOfType<MazeItemManager>();
        if (itemManager != null)
        {
            itemManager.OnItemCollected(this, player);
        }

        // Play collection animation if enabled
        if (hasCollectionAnimation)
        {
            StartCoroutine(PlayCollectionAnimation());
        }
        else if (hideAfterCollection)
        {
            gameObject.SetActive(false);
        }
    }

    System.Collections.IEnumerator PlayCollectionAnimation()
    {
        // Simple scale-up and fade-out animation
        Vector3 originalScale = transform.localScale;
        Color originalColor = spriteRenderer.color;

        float animationTime = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationTime;

            // Scale up slightly
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.2f, progress);

            // Fade out
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, progress);
            spriteRenderer.color = newColor;

            yield return null;
        }

        if (hideAfterCollection)
        {
            gameObject.SetActive(false);
        }
    }

    // Reset item for reuse
    public void ResetItem()
    {
        isCollected = false;
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }
}