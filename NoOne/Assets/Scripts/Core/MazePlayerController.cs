using System.Collections;
using UnityEngine;

public class MazePlayerController : MonoBehaviour
{
    [Header("Player Images")]
    public Sprite idleSprite;
    public Sprite moveSprite1;
    public Sprite moveSprite2;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float animationSpeed = 0.5f;

    [Header("Collision Settings")]
    public LayerMask wallLayerMask = -1; // Which layers count as walls
    public float collisionCheckDistance = 0.6f; // How far ahead to check for walls
    public float playerColliderRadius = 0.6f; // Player collider size

    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private Vector2 lastMovement;
    private bool isMoving = false;
    private bool canMove = true;

    // Animation timing
    private float animationTimer = 0f;
    private bool useFirstMoveSprite = true;

    // Reset functionality
    private Vector3 initialPosition;

    void Awake()
    {
        // Store initial position
        initialPosition = transform.position;

        // Add Rigidbody2D
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // No gravity
            rb.freezeRotation = true; // No spinning
            rb.drag = 5f; // Optional: smooth stopping
        }

        // Setup sprite renderer first
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Setup player collider only if it doesn't exist
        //CircleCollider2D playerCollider = GetComponent<CircleCollider2D>();
        //if (playerCollider == null)
        //{
        //    playerCollider = gameObject.AddComponent<CircleCollider2D>();
        //    playerCollider.radius = playerColliderRadius;
        //    playerCollider.isTrigger = false; // Player collider should NOT be trigger
        //    Debug.Log("MazePlayerController: Added CircleCollider2D to player");
        //}
        //else
        //{
        //    // Update existing collider settings
        //    playerCollider.radius = playerColliderRadius;
        //    playerCollider.isTrigger = false;
        //}

        //spriteRenderer.sprite = idleSprite;
    }

    void Start()
    {
        // Awake already handled initialization
        Debug.Log("MazePlayerController: Player initialized successfully");
    }

    void Update()
    {
        if (!canMove) return;

        HandleInput();
        HandleMovement();
        HandleAnimation();
        HandleSpriteFlipping();
    }

    void HandleInput()
    {
        // Get input
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        // Store the intended movement
        Vector2 intendedMovement = new Vector2(inputX, inputY);

        // Check for wall collision before allowing movement
        if (intendedMovement.magnitude > 0)
        {
            Vector2 checkedMovement = CheckWallCollision(intendedMovement);
            movement = checkedMovement;
        }
        else
        {
            movement = Vector2.zero;
        }

        // Check if actually moving
        isMoving = movement.magnitude > 0;

        // Store last movement for sprite flipping
        if (isMoving)
        {
            lastMovement = movement;
        }
    }

    Vector2 CheckWallCollision(Vector2 intendedMovement)
    {
        Vector2 currentPos = transform.position;
        Vector2 allowedMovement = Vector2.zero;

        // Check horizontal movement
        if (intendedMovement.x != 0)
        {
            Vector2 horizontalTarget = currentPos + Vector2.right * Mathf.Sign(intendedMovement.x) * collisionCheckDistance;

            if (!Physics2D.OverlapCircle(horizontalTarget, 0.2f, wallLayerMask))
            {
                allowedMovement.x = intendedMovement.x;
            }
        }

        // Check vertical movement
        if (intendedMovement.y != 0)
        {
            Vector2 verticalTarget = currentPos + Vector2.up * Mathf.Sign(intendedMovement.y) * collisionCheckDistance;

            if (!Physics2D.OverlapCircle(verticalTarget, 0.2f, wallLayerMask))
            {
                allowedMovement.y = intendedMovement.y;
            }
        }

        return allowedMovement;
    }

    void HandleMovement()
    {
        if (isMoving)
        {
            // Normalize diagonal movement
            movement = movement.normalized;

            // Move the player
            transform.Translate(movement * moveSpeed * Time.deltaTime);
        }
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

    void HandleSpriteFlipping()
    {
        // Flip based on horizontal movement
        if (lastMovement.x > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else if (lastMovement.x < 0)
        {
            spriteRenderer.flipX = true; // Face left
        }
    }

    public void DisableMovement()
    {
        canMove = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    // Collision detection for wall touching
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with a wall
        if (IsWall(collision.gameObject))
        {
            Debug.Log("MazePlayerController: Hit wall! Resetting to initial position");
            StartCoroutine(ResetToInitialPosition());
        }
    }

    bool IsWall(GameObject obj)
    {
        // Check if the object is on a wall layer
        return ((1 << obj.layer) & wallLayerMask) != 0;
    }

    IEnumerator ResetToInitialPosition()
    {
        canMove = false;

        // Wait for reset delay
        yield return new WaitForSeconds(1);

        // Reset position
        transform.position = initialPosition;

        // Reset movement state
        movement = Vector2.zero;
        lastMovement = Vector2.zero;
        isMoving = false;

        // Reset sprite to idle
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = idleSprite;
        }

        // Re-enable movement
        canMove = true;

        Debug.Log("MazePlayerController: Reset complete");
    }


    // Visualize collision check in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionCheckDistance);

        // Show collision check points
        Gizmos.color = Color.yellow;
        Vector2 pos = transform.position;
        Gizmos.DrawWireSphere(pos + Vector2.right * collisionCheckDistance, 0.2f);
        Gizmos.DrawWireSphere(pos + Vector2.left * collisionCheckDistance, 0.2f);
        Gizmos.DrawWireSphere(pos + Vector2.up * collisionCheckDistance, 0.2f);
        Gizmos.DrawWireSphere(pos + Vector2.down * collisionCheckDistance, 0.2f);

        // Show player collider
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, playerColliderRadius);
    }
}