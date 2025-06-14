// InvisibleWall.cs - Simple script for invisible wall objects
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InvisibleWall : MonoBehaviour
{
    [Header("Wall Settings")]
    public bool maintainFixedColliderSize = true;
    public Vector2 fixedColliderSize = Vector2.one;
    public bool ignoreTransformScale = true;

    [Header("Debug")]
    public bool showDebugInfo = false;
    public Color gizmoColor = Color.red;
    [Range(0f, 1f)]
    public float gizmoAlpha = 0.3f;

    private BoxCollider2D wallCollider;
    private Vector2 originalSize;
    private Vector3 originalScale;

    void Awake()
    {
        SetupWallCollider();
        SetupWallLayer();
        HideVisualComponents();
        StoreOriginalValues();
    }

    void SetupWallCollider()
    {
        // Ensure this object has a collider (RequireComponent ensures this)
        wallCollider = GetComponent<BoxCollider2D>();
        if (wallCollider == null)
        {
            wallCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        // Set initial size if maintainFixedColliderSize is enabled
        if (maintainFixedColliderSize)
        {
            wallCollider.size = fixedColliderSize;
        }
    }

    void SetupWallLayer()
    {
        // Set to Wall layer if it exists
        int wallLayer = LayerMask.NameToLayer("Wall");
        if (wallLayer != -1)
        {
            gameObject.layer = wallLayer;
        }
        else
        {
            Debug.LogWarning($"InvisibleWall: 'Wall' layer not found. Please create a 'Wall' layer for {gameObject.name}");
        }
    }

    void HideVisualComponents()
    {
        // Make sure the wall is invisible (no renderer or disabled renderer)
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.enabled = false; // Hide the sprite but keep collider
        }

        // Hide any other visual components
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    }

    void StoreOriginalValues()
    {
        if (wallCollider != null)
        {
            originalSize = wallCollider.size;
        }
        originalScale = transform.localScale;
    }

    void Start()
    {
        // Apply fixed size after all initialization
        if (maintainFixedColliderSize)
        {
            ApplyFixedColliderSize();
        }

        if (showDebugInfo)
        {
            LogWallInfo();
        }
    }

    void Update()
    {
        // Continuously maintain fixed collider size if enabled
        if (maintainFixedColliderSize)
        {
            ApplyFixedColliderSize();
        }

        // Reset scale if ignoreTransformScale is enabled
        if (ignoreTransformScale && transform.localScale != Vector3.one)
        {
            // Keep transform scale at 1,1,1 to avoid scaling issues
            transform.localScale = Vector3.one;
        }
    }

    void ApplyFixedColliderSize()
    {
        if (wallCollider != null && wallCollider.size != fixedColliderSize)
        {
            wallCollider.size = fixedColliderSize;
        }
    }

    void LogWallInfo()
    {
        Debug.Log($"InvisibleWall '{gameObject.name}' Info:");
        Debug.Log($"- Position: {transform.position}");
        Debug.Log($"- Scale: {transform.localScale}");
        Debug.Log($"- Collider Size: {wallCollider.size}");
        Debug.Log($"- Bounds Size: {wallCollider.bounds.size}");
        Debug.Log($"- Screen Size: {Screen.width}x{Screen.height}");
    }

    // This runs when the script is added in the editor
    void Reset()
    {
        // Automatically add BoxCollider2D when script is added
        if (GetComponent<BoxCollider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        // Set default size
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = fixedColliderSize; // Use fixed size instead of Vector2.one
        }

        // Set to Wall layer if it exists
        int wallLayer = LayerMask.NameToLayer("Wall");
        if (wallLayer != -1)
        {
            gameObject.layer = wallLayer;
        }

        // Set default settings
        maintainFixedColliderSize = true;
        ignoreTransformScale = true;
    }

    // Show wall bounds in editor for easy placement
    void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            // Semi-transparent filled cube
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, gizmoAlpha);
            Gizmos.DrawCube(transform.position, col.bounds.size);

            // Wire frame outline
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);

            // Show size information in Scene view
            if (showDebugInfo)
            {
#if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    transform.position + Vector3.up * (col.bounds.size.y * 0.5f + 0.2f),
                    $"Size: {col.bounds.size.x:F1} x {col.bounds.size.y:F1}"
                );
#endif
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Show more detailed info when selected
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            // Highlight selected wall
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);

            // Show corner points
            Vector3 size = col.bounds.size;
            Vector3 pos = transform.position;

            Gizmos.color = Color.green;
            float pointSize = 0.1f;

            // Draw corner spheres
            Gizmos.DrawSphere(pos + new Vector3(-size.x / 2, -size.y / 2, 0), pointSize);
            Gizmos.DrawSphere(pos + new Vector3(size.x / 2, -size.y / 2, 0), pointSize);
            Gizmos.DrawSphere(pos + new Vector3(-size.x / 2, size.y / 2, 0), pointSize);
            Gizmos.DrawSphere(pos + new Vector3(size.x / 2, size.y / 2, 0), pointSize);
        }
    }

    // Public methods for external control
    public void SetFixedSize(Vector2 newSize)
    {
        fixedColliderSize = newSize;
        if (maintainFixedColliderSize)
        {
            ApplyFixedColliderSize();
        }
    }

    public void SetFixedSize(float width, float height)
    {
        SetFixedSize(new Vector2(width, height));
    }

    public Vector2 GetColliderSize()
    {
        return wallCollider != null ? wallCollider.size : Vector2.zero;
    }

    public Vector2 GetBoundsSize()
    {
        if (wallCollider != null)
        {
            Bounds bounds = wallCollider.bounds;
            return new Vector2(bounds.size.x, bounds.size.y);
        }
        return Vector2.zero;
    }

    public void EnableFixedSize(bool enable)
    {
        maintainFixedColliderSize = enable;
        if (enable)
        {
            ApplyFixedColliderSize();
        }
    }

    // Method to reset to original size
    public void ResetToOriginalSize()
    {
        if (wallCollider != null)
        {
            wallCollider.size = originalSize;
            fixedColliderSize = originalSize;
        }
    }

    // Validation method
    public bool ValidateWallSetup()
    {
        bool isValid = true;

        if (wallCollider == null)
        {
            Debug.LogError($"InvisibleWall: No BoxCollider2D found on {gameObject.name}");
            isValid = false;
        }

        if (gameObject.layer != LayerMask.NameToLayer("Wall"))
        {
            Debug.LogWarning($"InvisibleWall: {gameObject.name} is not on 'Wall' layer");
        }

        return isValid;
    }
}