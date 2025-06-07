using UnityEngine;

public class OctopusController : MonoBehaviour
{
    [Header("Octopus Images")]
    public Sprite normalSprite;
    public Sprite hitSprite;

    [Header("Hit Settings")]
    public int maxHits = 1; // Each octopus only needs 1 hit
    private int currentHits = 0;

    [Header("Octopus ID")]
    public int octopusID = 0; // 0, 1, or 2 for the three octopuses

    private SpriteRenderer spriteRenderer;
    private bool isHit = false;
    private bool isDead = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Add collider if not present
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<CircleCollider2D>();
        }

        spriteRenderer.sprite = normalSprite;
    }

    public void TakeHit()
    {
        if (currentHits >= maxHits || isDead) return;

        currentHits++;
        Debug.Log($"Octopus {octopusID} hit! {currentHits}/{maxHits} - One hit kill!");

        // Calculate knockback direction based on player position
        Vector3 knockbackDirection = CalculateKnockbackDirection();

        // Start hit animation with directional knockback
        StartCoroutine(HitAnimation(knockbackDirection));

        // Check if octopus is defeated
        if (currentHits >= maxHits)
        {
            OnDefeated();
        }
    }
    Vector3 CalculateKnockbackDirection()
    {
        // Find the player
        PlayerControllerLevel02 player = FindObjectOfType<PlayerControllerLevel02>();
        if (player != null)
        {
            // Calculate direction from player to octopus
            Vector3 playerToOctopus = (transform.position - player.transform.position).normalized;
            return playerToOctopus;
        }

        // Default to left if no player found
        return Vector3.left;
    }

    System.Collections.IEnumerator HitAnimation(Vector3 knockbackDirection)
    {
        isHit = true;
        spriteRenderer.sprite = hitSprite;

        // Store original position
        Vector3 originalPosition = transform.position;

        // Move octopus in the opposite direction from the hit
        float knockbackDistance = 0.5f;
        Vector3 knockbackPosition = originalPosition + knockbackDirection * knockbackDistance;

        // Quick movement back
        float moveTime = 0.1f;
        float elapsedTime = 0f;

        // Move to knockback position
        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / moveTime;
            transform.position = Vector3.Lerp(originalPosition, knockbackPosition, progress);
            yield return null;
        }

        // Stay at knockback position briefly
        yield return new WaitForSeconds(0.2f);

        // Move back to original position
        elapsedTime = 0f;
        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / moveTime;
            transform.position = Vector3.Lerp(knockbackPosition, originalPosition, progress);
            yield return null;
        }

        // Ensure exact original position
        transform.position = originalPosition;

        // Flash effect before dying (if this was the final hit)
        if (currentHits >= maxHits)
        {
            // Flash white several times
            for (int i = 0; i < 5; i++)
            {
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(0.1f);
            }

            // Set inactive after death animation
            gameObject.SetActive(false);
        }
        else
        {
            // Normal hit - return to normal sprite
            isHit = false;
            spriteRenderer.sprite = normalSprite;
        }
    }
    void OnDefeated()
    {
        isDead = true;
        Debug.Log($"Octopus {octopusID} defeated!");

        // Notify the OctopusManager
        OctopusManager octopusManager = FindObjectOfType<OctopusManager>();
        if (octopusManager != null)
        {
            octopusManager.OnOctopusDefeated(octopusID);
        }
    }
}
