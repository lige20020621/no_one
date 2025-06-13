using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMover : MonoBehaviour
{

    public float moveSpeed = 5f;
    public Sprite[] walkSprites; // 3 walking images
    public float animationSpeed = 0.1f; // time between sprite changes

    private bool canMove = true; // To disable movement during dialogue
    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;
    private float animationTimer = 0f;

    [Header("Collision Settings")]
    public LayerMask wallLayerMask = 1 << 3; // Wall layer only (Layer 3)
    public float collisionCheckDistance = 0.6f; // How far ahead to check for walls
    public float collisionRadius = 0.3f; // Radius for collision detection


    private Rigidbody2D rb2D;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("No SpriteRenderer found on " + gameObject.name + ". Adding one automatically.");
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Get or add Rigidbody2D
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            Debug.LogWarning("No Rigidbody2D found on " + gameObject.name + ". Adding one automatically.");
            rb2D = gameObject.AddComponent<Rigidbody2D>();
        }

        // Make sure the player has a collider for interactions
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogWarning("No Collider2D found on " + gameObject.name + ". Adding BoxCollider2D automatically.");
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();

            // Try to size the collider appropriately if we have a sprite
            if (spriteRenderer.sprite != null)
            {
                boxCollider.size = spriteRenderer.sprite.bounds.size;
                boxCollider.offset = new Vector2(0, 0);
            }
        }

        // Make sure player has the "Player" tag
        if (gameObject.tag != "Player")
        {
            Debug.LogWarning("Player GameObject doesn't have the 'Player' tag. Setting it automatically.");
            gameObject.tag = "Player";
        }

        // Check if we have valid sprites
        if (walkSprites == null || walkSprites.Length < 3)
        {
            Debug.LogWarning("Not enough walk sprites assigned to PlayerMover on " + gameObject.name + ". Need at least 3 sprites.");
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.sprite = walkSprites[0]; // Set initial sprite
        }

        // Make debug message to confirm initialization
        Debug.Log("PlayerMover initialized on " + gameObject.name);
    }

    private void Update()
    {

        if (!canMove)
            return;
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 inputMovement = new Vector2(moveX, moveY).normalized;

        // Check for wall collisions and get allowed movement
        Vector2 allowedMovement = CheckWallCollision(inputMovement);
        //transform.position += move * moveSpeed * Time.deltaTime;
        // Apply movement using Rigidbody2D
        if (allowedMovement.magnitude > 0.01f)
        {
            Vector2 newPosition = rb2D.position + allowedMovement * moveSpeed * Time.deltaTime;
            rb2D.MovePosition(newPosition);

            // Animate walking
            AnimateWalk();
        }
        else
        {
            // Stop at idle sprite when not moving
            if (walkSprites.Length > 0)
            {
                spriteRenderer.sprite = walkSprites[0];
                currentSpriteIndex = 0;
            }
        }
    }

    Vector2 CheckWallCollision(Vector2 intendedMovement)
    {
        Vector2 currentPos = rb2D.position;
        Vector2 allowedMovement = Vector2.zero;

        // Check horizontal movement
        if (Mathf.Abs(intendedMovement.x) > 0.01f)
        {
            Vector2 horizontalTarget = currentPos + Vector2.right * Mathf.Sign(intendedMovement.x) * collisionCheckDistance;

            // Use CircleCast for more accurate collision detection
            RaycastHit2D horizontalHit = Physics2D.CircleCast(
                currentPos,
                collisionRadius,
                Vector2.right * Mathf.Sign(intendedMovement.x),
                collisionCheckDistance,
                wallLayerMask
            );

            if (horizontalHit.collider == null)
            {
                allowedMovement.x = intendedMovement.x;
            }
        }

        // Check vertical movement
        if (Mathf.Abs(intendedMovement.y) > 0.01f)
        {
            Vector2 verticalTarget = currentPos + Vector2.up * Mathf.Sign(intendedMovement.y) * collisionCheckDistance;

            // Use CircleCast for more accurate collision detection
            RaycastHit2D verticalHit = Physics2D.CircleCast(
                currentPos,
                collisionRadius,
                Vector2.up * Mathf.Sign(intendedMovement.y),
                collisionCheckDistance,
                wallLayerMask
            );

            if (verticalHit.collider == null)
            {
                allowedMovement.y = intendedMovement.y;
            }
        }

        return allowedMovement;
    }


    void AnimateWalk()
    {
        // Only animate if we have enough sprites and a SpriteRenderer
        if (spriteRenderer == null || walkSprites == null || walkSprites.Length < 3) return;

        animationTimer += Time.deltaTime;
        if (animationTimer >= animationSpeed)
        {
            animationTimer = 0f;

            // Cycle only between 1 and 2
            currentSpriteIndex++;
            if (currentSpriteIndex < 1 || currentSpriteIndex > 2)
            {
                currentSpriteIndex = 1; // Force to 1 or 2 only
            }

            spriteRenderer.sprite = walkSprites[currentSpriteIndex];
        }
    }

    // Method to disable movement (called when dialogue starts)
    public void DisableMovement()
    {
        canMove = false;

        // Reset to idle sprite when movement is disabled
        if (walkSprites != null && walkSprites.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = walkSprites[0];
        }

        Debug.Log("Player movement disabled");
    }

    // Method to enable movement (called when dialogue ends)
    public void EnableMovement()
    {
        canMove = true;
        Debug.Log("Player movement enabled");
    }

    // Utility method to check if player is currently allowed to move
    public bool CanPlayerMove()
    {
        return canMove;
    }
}