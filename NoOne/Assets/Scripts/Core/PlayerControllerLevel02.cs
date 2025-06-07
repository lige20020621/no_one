using UnityEngine;

public class PlayerControllerLevel02 : MonoBehaviour
{
    [Header("Player Images")]
    public Sprite idleSprite;
    public Sprite moveSprite1;
    public Sprite moveSprite2;
    public Sprite hitSprite;

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
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Check if moving
        isMoving = movement.magnitude > 0;

        // Check for hit input (Space key)
        if (Input.GetKeyDown(KeyCode.Space) && !isHitting)
        {
            StartCoroutine(PerformHit());
        }
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