using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Sprite[] walkSprites; // 3 walking images
    public float animationSpeed = 0.1f; // time between sprite changes

    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;
    private float animationTimer = 0f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(moveX, moveY, 0f).normalized;
        transform.position += move * moveSpeed * Time.deltaTime;

        // 限制角色在螢幕內
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        viewPos.x = Mathf.Clamp(viewPos.x, 0.05f, 0.95f);
        viewPos.y = Mathf.Clamp(viewPos.y, 0.05f, 0.95f);
        transform.position = Camera.main.ViewportToWorldPoint(viewPos);

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
}