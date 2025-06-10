using UnityEngine;

public class PlayerControllerLevel02 : MonoBehaviour
{
    [Header("Player Images")]
    public Sprite idleSprite;
    public Sprite moveSprite1;
    public Sprite moveSprite2;
    public Sprite hitSprite;


    [Header("Collision Settings")]
    public LayerMask wallLayerMask = 1 << 3; // Wall layer only (Layer 3)
    public float collisionCheckDistance = 0.6f; // How far ahead to check for walls

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float animationSpeed = 0.5f;

    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private bool isMoving = false;
    private bool isHitting = false;
    private bool canMove = false; // Start with movement disabled for initial dialogue

    // Animation timing
    private float animationTimer = 0f;
    private bool useFirstMoveSprite = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Set initial sprite
        spriteRenderer.sprite = idleSprite;
    }

    void Update()
    {
        if (!canMove) return;

        HandleInput();
        HandleMovement();
        HandleAnimation();
        HandleSpriteFlipping(); // Add sprite flipping
    }

    void HandleInput()
    {
        // Get raw input from player
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        // Create intended movement vector
        Vector2 intendedMovement = new Vector2(inputX, inputY);

        // Apply wall collision check to prevent going outside screen
        if (intendedMovement.magnitude > 0)
        {
            Vector2 checkedMovement = CheckWallCollision(intendedMovement);
            movement = checkedMovement;
        }
        else
        {
            movement = Vector2.zero;
        }

        // Check if we're actually moving (after wall collision check)
        isMoving = movement.magnitude > 0;

        // Check for hit input (Space key)
        if (Input.GetKeyDown(KeyCode.Space) && !isHitting)
        {
            StartCoroutine(PerformHit());
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
        if (isMoving && !isHitting)
        {
            // Normalize diagonal movement
            movement = movement.normalized;

            // Move the player
            transform.Translate(movement * moveSpeed * Time.deltaTime);
        }
    }

    void HandleAnimation()
    {
        if (isHitting)
        {
            spriteRenderer.sprite = hitSprite;
        }
        else if (isMoving)
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
        // Only flip when moving horizontally
        if (isMoving && !isHitting)
        {
            if (movement.x > 0)
            {
                // Moving right - face right (normal sprite)
                spriteRenderer.flipX = false;
            }
            else if (movement.x < 0)
            {
                // Moving left - face left (flip sprite)
                spriteRenderer.flipX = true;
            }
            // Don't change flip when only moving vertically (movement.x == 0)
        }
    }

    System.Collections.IEnumerator PerformHit()
    {
        isHitting = true;

        // Check for octopus in range
        CheckForOctopusHit();

        // Show hit animation for a short time
        yield return new WaitForSeconds(0.3f);

        isHitting = false;
    }

    void CheckForOctopusHit()
    {
        // Find the octopus in range (there's only one octopus in the game)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        foreach (Collider2D collider in colliders)
        {
            OctopusController octopus = collider.GetComponent<OctopusController>();
            if (octopus != null)
            {
                octopus.TakeHit();
                break; // Hit the octopus
            }
        }
    }

    public void DisableMovement()
    {
        canMove = false;

        // Extra safety check to prevent null reference
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    public void EnableMovement()
    {
        canMove = true;
    }
}