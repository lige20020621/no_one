using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMover : MonoBehaviour
{

    public float moveSpeed = 5f;
    public Sprite[] walkSprites; // 3 walking images
    public float animationSpeed = 0.5f; // time between sprite changes

    private bool canMove = true; // To disable movement during dialogue
    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;
    private float animationTimer = 0f;

    [Header("Collision Settings")]
    public LayerMask wallLayerMask = 1 << 3; // Wall layer only (Layer 3)
    public float collisionCheckDistance = 0.6f; // How far ahead to check for walls
    public float collisionRadius = 0.3f; // Radius for collision detection

    [Header("Player Images")]
    public Sprite idleSprite;
    public Sprite moveSprite1;
    public Sprite moveSprite2;

    private Rigidbody2D rb2D;
    private Vector2 movement;
    private bool isMoving = false;
    private bool useFirstMoveSprite = true;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Get or add Rigidbody2D
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            rb2D = gameObject.AddComponent<Rigidbody2D>();
        }

        // Make sure the player has a collider for interactions
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
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
            gameObject.tag = "Player";
        }

        // Check if we have valid sprites
        if (walkSprites == null || walkSprites.Length < 3)
        {
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.sprite = walkSprites[0]; // Set initial sprite
        }
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleAnimation();
    }

    void HandleInput()
    {
        // Get raw input from player
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        // Create intended movement vector
        Vector2 intendedMovement = new Vector2(inputX, inputY);

        // Apply wall collision check to prevent going outside screen
        if (intendedMovement.magnitude > 0 && canMove)
        {
            Vector2 checkedMovement = CheckWallCollision(intendedMovement);
            movement = checkedMovement;
        }
        else
        {
            movement = Vector2.zero;
        }
        isMoving = movement.magnitude > 0;
    }
    void HandleMovement()
    {
        // Normalize diagonal movement
        movement = movement.normalized;

        // Move the player
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
    void HandleAnimation()
    {
        if (isMoving)
        {
            // Animate between two move sprites
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationSpeed)
            {
                animationTimer = 0f;
                useFirstMoveSprite = !useFirstMoveSprite;
                spriteRenderer.sprite = useFirstMoveSprite ? moveSprite1 : moveSprite2;
            }
        }
        else
        {
            // Idle state
            spriteRenderer.sprite = idleSprite;
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
    }

    // Method to enable movement (called when dialogue ends)
    public void EnableMovement()
    {
        canMove = true;
    }

    // Utility method to check if player is currently allowed to move
    public bool CanPlayerMove()
    {
        return canMove;
    }
}