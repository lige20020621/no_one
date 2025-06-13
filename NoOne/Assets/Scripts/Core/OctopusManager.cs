// OctopusManager.cs - Manages all three octopuses and leg collection
using UnityEngine;

public class OctopusManager : MonoBehaviour
{
    [Header("Leg Images")]
    public GameObject[] legImages = new GameObject[3]; // Array of 3 leg GameObjects

    [Header("Octopuses")]
    public OctopusController[] octopuses = new OctopusController[3]; // Array of 3 octopuses

    private int defeatedOctopusCount = 0;
    private bool[] octopusDefeated = new bool[3]; // Track which octopuses are defeated

    void Start()
    {
        // Make sure all leg images are initially hidden
        for (int i = 0; i < legImages.Length; i++)
        {
            if (legImages[i] != null)
            {
                legImages[i].SetActive(false);
            }
        }

        // Assign IDs to octopuses if not already set
        for (int i = 0; i < octopuses.Length; i++)
        {
            if (octopuses[i] != null)
            {
                octopuses[i].octopusID = i;
            }
        }
    }

    public void OnOctopusDefeated(int octopusID)
    {
        // Check if this octopus was already defeated
        if (octopusDefeated[octopusID]) return;

        // Mark as defeated
        octopusDefeated[octopusID] = true;
        defeatedOctopusCount++;

        Debug.Log($"Total hits across all octopuses: {defeatedOctopusCount}/3");

        // Show the corresponding leg image
        if (octopusID < legImages.Length && legImages[octopusID] != null)
        {
            legImages[defeatedOctopusCount - 1].SetActive(true);
            Debug.Log($"Leg image {octopusID} shown!");
        }

        // Check if all octopuses are defeated
        if (defeatedOctopusCount >= 3)
        {

            StartCoroutine(OnAllOctopusesDefeated());
        }
    }

    // Ensure all code paths in OnAllOctopusesDefeated return a value.
    private System.Collections.IEnumerator OnAllOctopusesDefeated()
    {
        Debug.Log("All 3 octopuses defeated with 3 total hits! Starting secondary dialogue...");

        // Disable player movement
        PlayerControllerLevel02 player = FindObjectOfType<PlayerControllerLevel02>();
        if (player != null)
        {
            player.DisableMovement();
        }

        // Start the secondary dialogue
        GameManagerLevel2 gameManager = GameManagerLevel2.instance;
        if (gameManager != null && gameManager.dialogueManager != null)
        {
            yield return new WaitForSeconds(2f);
            gameManager.ShowActionPanel();
        }

        // Return null to satisfy IEnumerator return type
        yield return null;
    }

    // Optional: Method to reset the game state
    public void ResetOctopuses()
    {
        defeatedOctopusCount = 0;

        for (int i = 0; i < 3; i++)
        {
            octopusDefeated[i] = false;

            // Hide leg images
            if (legImages[i] != null)
            {
                legImages[i].SetActive(false);
            }

            // Reactivate octopuses
            if (octopuses[i] != null)
            {
                octopuses[i].gameObject.SetActive(true);
                // Reset octopus hit count (you might need to add a Reset method to OctopusController)
            }
        }
    }
}