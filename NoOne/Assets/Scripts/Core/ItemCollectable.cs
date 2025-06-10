using UnityEngine;

public class ItemCollectable : MonoBehaviour
{
    [Header("Item Settings")]
    public int itemID; // 0-3 for the 4 different items
    public SpeakerType associatedSpeaker; // Which NPC this item represents
    public string itemName = "Item";

    [Header("Collision Settings")]
    public ColliderType colliderType = ColliderType.Circle;
    [Header("Circle Collider")]
    public float colliderRadius = 0.5f; // For circle collider
    [Header("Box Collider")]
    public Vector2 boxSize = new Vector2(1f, 1f); // For box collider

    [Header("Visual Settings")]
    public bool hideAfterCollection = false;
    public bool hasCollectionAnimation = false;

    [Header("Debug")]
    public bool enableDebugLogs = false;

    public enum ColliderType
    {
        Circle,
        Box,
        Polygon
    }

    private bool isCollected = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D itemCollider;
    private bool colliderSetupComplete = false;

    // This runs when the script is added to an object in the editor
    void Reset()
    {
        SetupCollider();
        if (enableDebugLogs) Debug.Log($"ItemCollectable: Added {colliderType} collider via Reset()");
    }

    void Awake()
    {
        // Setup collider in Awake to ensure it's ready before Start
        if (!colliderSetupComplete)
        {
            //SetupColliderSafe();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.isKinematic = true; // Items don't move
        }
    }

    void SetupColliderSafe()
    {
        // Check if we already have the correct collider type
        itemCollider = GetComponent<Collider2D>();

        bool needsNewCollider = false;

        if (itemCollider == null)
        {
            needsNewCollider = true;
            if (enableDebugLogs) Debug.Log("ItemCollectable: No collider found, creating new one");
        }
        else
        {
            // Check if collider type matches what we want
            switch (colliderType)
            {
                case ColliderType.Circle:
                    if (!(itemCollider is CircleCollider2D))
                        needsNewCollider = true;
                    break;
                case ColliderType.Box:
                    if (!(itemCollider is BoxCollider2D))
                        needsNewCollider = true;
                    break;
                case ColliderType.Polygon:
                    if (!(itemCollider is PolygonCollider2D))
                        needsNewCollider = true;
                    break;
            }

            if (needsNewCollider && enableDebugLogs)
            {
                Debug.Log($"ItemCollectable: Wrong collider type found, need to replace with {colliderType}");
            }
        }

        if (needsNewCollider)
        {
            // Remove existing collider if it's the wrong type
            if (itemCollider != null)
            {
                if (Application.isPlaying)
                    Destroy(itemCollider);
                else
                    DestroyImmediate(itemCollider);
            }

            // Add the correct collider type
            CreateCollider();
        }
        else
        {
            // Configure existing collider
            ConfigureExistingCollider();
        }

        colliderSetupComplete = true;
    }

    void CreateCollider()
    {
        //switch (colliderType)
        //{
        //    case ColliderType.Circle:
        //        CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        //        circleCollider.isTrigger = true;
        //        circleCollider.radius = colliderRadius;
        //        itemCollider = circleCollider;
        //        break;

        //    case ColliderType.Box:
        //        BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
        //        boxCollider.isTrigger = true;
        //        boxCollider.size = boxSize;
        //        itemCollider = boxCollider;
        //        break;

        //    case ColliderType.Polygon:
        //        PolygonCollider2D polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        //        polygonCollider.isTrigger = true;
        //        itemCollider = polygonCollider;
        //        break;
        //}

        if (enableDebugLogs) Debug.Log($"ItemCollectable: Created {colliderType} collider");
    }

    void ConfigureExistingCollider()
    {
        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;

            // Update size/radius if needed
            switch (colliderType)
            {
                case ColliderType.Circle:
                    CircleCollider2D circle = itemCollider as CircleCollider2D;
                    if (circle != null)
                        circle.radius = colliderRadius;
                    break;

                case ColliderType.Box:
                    BoxCollider2D box = itemCollider as BoxCollider2D;
                    if (box != null)
                        box.size = boxSize;
                    break;
            }

            if (enableDebugLogs) Debug.Log($"ItemCollectable: Configured existing {colliderType} collider");
        }
    }

    void SetupCollider()
    {
        // This is the old method, kept for Reset() functionality
        // Remove all existing colliders
        Collider2D[] existingColliders = GetComponents<Collider2D>();
        for (int i = 0; i < existingColliders.Length; i++)
        {
            if (Application.isPlaying)
            {
                Destroy(existingColliders[i]);
            }
            else
            {
                if (Application.isEditor && !Application.isPlaying)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.delayCall += () => {
                        if (existingColliders[i] != null)
                            DestroyImmediate(existingColliders[i]);
                    };
#endif
                }
            }
        }

        // Create new collider
        CreateCollider();
    }

    void Start()
    {
        // Ensure collider is properly configured
        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;
        }

        if (enableDebugLogs) Debug.Log($"ItemCollectable: {itemName} (ID: {itemID}) initialized with {colliderType} collider");
    }

    // Update collider when values change in inspector
    void OnValidate()
    {
        if (itemCollider == null)
            itemCollider = GetComponent<Collider2D>();

        if (itemCollider != null)
        {
            switch (colliderType)
            {
                case ColliderType.Circle:
                    CircleCollider2D circle = itemCollider as CircleCollider2D;
                    if (circle != null)
                        circle.radius = colliderRadius;
                    break;

                case ColliderType.Box:
                    BoxCollider2D box = itemCollider as BoxCollider2D;
                    if (box != null)
                        box.size = boxSize;
                    break;
            }
        }

        // If collider type changed, schedule collider setup for next frame
        if (itemCollider != null)
        {
            bool needsNewCollider = false;

            if (colliderType == ColliderType.Circle && !(itemCollider is CircleCollider2D))
                needsNewCollider = true;
            else if (colliderType == ColliderType.Box && !(itemCollider is BoxCollider2D))
                needsNewCollider = true;
            else if (colliderType == ColliderType.Polygon && !(itemCollider is PolygonCollider2D))
                needsNewCollider = true;

            if (needsNewCollider)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () => {
                    if (this != null)
                    {
                        SetupCollider();
                        colliderSetupComplete = true;
                    }
                };
#endif
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D");
        if (enableDebugLogs) Debug.Log($"ItemCollectable: Trigger entered by {other.name} with tag {other.tag}");

        // Check if player touched the item
        if (other.CompareTag("Player") && !isCollected)
        {
            if (enableDebugLogs) Debug.Log($"ItemCollectable: Player touched {itemName}!");
            CollectItem(other.gameObject);
        }
    }

    void CollectItem(GameObject player)
    {
        Debug.Log("CollectItem");
        if (isCollected) return;

        isCollected = true;
        Debug.Log($"Collected {itemName} (ID: {itemID})");

        // Disable the collider immediately to prevent multiple triggers
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
            if (enableDebugLogs) Debug.Log("ItemCollectable: Collider disabled");
        }

        // Find the maze manager and notify collection
        MazeItemManager itemManager = FindObjectOfType<MazeItemManager>();
        if (itemManager != null)
        {
            if (enableDebugLogs) Debug.Log("ItemCollectable: Notifying MazeItemManager");
            itemManager.OnItemCollected(this, player);
        }
        else
        {
            Debug.LogError("ItemCollectable: No MazeItemManager found in scene!");
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
        Color originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        float animationTime = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationTime;

            // Scale up slightly
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.2f, progress);

            // Fade out
            if (spriteRenderer != null)
            {
                Color newColor = originalColor;
                newColor.a = Mathf.Lerp(1f, 0f, progress);
                spriteRenderer.color = newColor;
            }

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

        // Re-enable the collider
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        if (enableDebugLogs) Debug.Log($"ItemCollectable: {itemName} reset");
    }

    // Show collider bounds in editor
    void OnDrawGizmosSelected()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        Gizmos.color = new Color(0, 1, 0, 0.3f); // Semi-transparent green

        if (col is CircleCollider2D circle)
        {
            Gizmos.DrawSphere(transform.position + (Vector3)circle.offset, circle.radius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
        }
        else if (col is BoxCollider2D box)
        {
            Vector3 center = transform.position + (Vector3)box.offset;
            Vector3 size = new Vector3(box.size.x, box.size.y, 0.1f);
            Gizmos.DrawCube(center, size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(center, size);
        }
        else if (col is PolygonCollider2D polygon)
        {
            // Draw polygon outline
            Gizmos.color = Color.green;
            for (int i = 0; i < polygon.points.Length; i++)
            {
                Vector3 current = transform.TransformPoint(polygon.points[i]);
                Vector3 next = transform.TransformPoint(polygon.points[(i + 1) % polygon.points.Length]);
                Gizmos.DrawLine(current, next);
            }
        }
    }
}