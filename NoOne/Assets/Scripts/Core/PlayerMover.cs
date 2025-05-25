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

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("No SpriteRenderer found on " + gameObject.name + ". Adding one automatically.");
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
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

        Vector3 move = new Vector3(moveX, moveY, 0f).normalized;
        //transform.position += move * moveSpeed * Time.deltaTime;
        GetComponent<Rigidbody2D>().MovePosition(transform.position + move * moveSpeed * Time.deltaTime);

        // 動畫控制
        if (move.magnitude > 0.1f)
        {
            AnimateWalk();
        }
        else
        {
            if (walkSprites.Length > 0)
            {
                spriteRenderer.sprite = walkSprites[0];
                currentSpriteIndex = 0;
            }
        }
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