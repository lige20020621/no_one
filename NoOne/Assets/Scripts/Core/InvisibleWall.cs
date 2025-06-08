// InvisibleWall.cs - Simple script for invisible wall objects
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InvisibleWall : MonoBehaviour
{
    void Awake()
    {
        // Ensure this object has a collider (RequireComponent ensures this)
        BoxCollider2D wallCollider = GetComponent<BoxCollider2D>();
        if (wallCollider == null)
        {
            wallCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        // Set to Wall layer if it exists
        int wallLayer = LayerMask.NameToLayer("Wall");
        if (wallLayer != -1)
        {
            gameObject.layer = wallLayer;
        }

        // Make sure the wall is invisible (no renderer or disabled renderer)
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.enabled = false; // Hide the sprite but keep collider
        }
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
            collider.size = Vector2.one; // Default 1x1 size
        }

        // Set to Wall layer if it exists
        int wallLayer = LayerMask.NameToLayer("Wall");
        if (wallLayer != -1)
        {
            gameObject.layer = wallLayer;
        }
    }

    // Show wall bounds in editor for easy placement
    void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f); // Semi-transparent red
            Gizmos.DrawCube(transform.position, col.bounds.size);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}