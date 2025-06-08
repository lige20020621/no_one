using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [Header("Wall Settings")]
    public GameObject wallPrefab; // Prefab for wall blocks
    public Texture2D mazeTexture; // Your maze map texture
    public float wallSize = 1f; // Size of each wall block
    public Color wallColor = Color.black; // Color that represents walls in your texture
    public float colorTolerance = 0.1f; // How close colors need to be to count as walls

    [Header("Generation Settings")]
    public Vector2 mazeStartPosition = Vector2.zero; // Where to start generating the maze
    public bool generateOnStart = true;

    void Start()
    {
        if (generateOnStart && mazeTexture != null)
        {
            GenerateWallsFromTexture();
        }
    }

    [ContextMenu("Generate Walls")]
    public void GenerateWallsFromTexture()
    {
        if (mazeTexture == null)
        {
            Debug.LogError("Maze texture is not assigned!");
            return;
        }

        if (wallPrefab == null)
        {
            Debug.LogError("Wall prefab is not assigned!");
            return;
        }

        // Clear existing walls
        ClearExistingWalls();

        // Generate walls based on texture
        for (int x = 0; x < mazeTexture.width; x++)
        {
            for (int y = 0; y < mazeTexture.height; y++)
            {
                Color pixelColor = mazeTexture.GetPixel(x, y);

                // Check if this pixel represents a wall
                if (IsWallColor(pixelColor))
                {
                    CreateWallAt(x, y);
                }
            }
        }

        Debug.Log($"Generated walls for maze of size {mazeTexture.width}x{mazeTexture.height}");
    }

    bool IsWallColor(Color pixelColor)
    {
        // Check if the pixel color is close enough to the wall color
        float colorDistance = Vector3.Distance(
            new Vector3(pixelColor.r, pixelColor.g, pixelColor.b),
            new Vector3(wallColor.r, wallColor.g, wallColor.b)
        );

        return colorDistance <= colorTolerance;
    }

    void CreateWallAt(int textureX, int textureY)
    {
        // Convert texture coordinates to world coordinates
        Vector2 worldPosition = new Vector2(
            mazeStartPosition.x + textureX * wallSize,
            mazeStartPosition.y + (mazeTexture.height - textureY - 1) * wallSize // Flip Y axis
        );

        // Create wall instance
        GameObject wall = Instantiate(wallPrefab, worldPosition, Quaternion.identity);
        wall.transform.parent = transform; // Parent to this object for organization
        wall.name = $"Wall_{textureX}_{textureY}";

        // Ensure wall has collider and is on correct layer
        SetupWallCollider(wall);
    }

    void SetupWallCollider(GameObject wall)
    {
        // Add BoxCollider2D if not present
        if (wall.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D wallCollider = wall.AddComponent<BoxCollider2D>();
            wallCollider.size = Vector2.one * wallSize;
        }

        // Set wall to appropriate layer (create "Wall" layer if needed)
        wall.layer = LayerMask.NameToLayer("Wall");
        if (wall.layer == -1)
        {
            wall.layer = LayerMask.NameToLayer("Default");
            Debug.LogWarning("Wall layer not found, using Default layer. Create a 'Wall' layer for better organization.");
        }
    }

    void ClearExistingWalls()
    {
        // Remove all child objects (existing walls)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}