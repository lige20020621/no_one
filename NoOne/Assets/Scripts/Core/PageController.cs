// PageController.cs
using System.Collections.Generic;
using UnityEngine;

public class PageController : MonoBehaviour
{
    // Singleton pattern
    public static PageController Instance { get; private set; }

    // Add more pages as needed

    private Dictionary<PageType, GameObject> pageInstances = new Dictionary<PageType, GameObject>();
    private PageType currentPage = PageType.None;

    public enum PageType
    {
        None,
        Landing,
        Room,
        // Add more page types as needed
    }

    private void Awake()
    {
        // Ensure singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Start with no active page
        foreach (var page in pageInstances.Values)
        {
            page.SetActive(false);
        }
    }

    private void Start()
    {
        // Start with the landing page
        NavigateToPage(PageType.Landing);
    }

    private void InstantiatePage(PageType type, GameObject prefab)
    {
        if (prefab != null)
        {
            GameObject pageInstance = Instantiate(prefab, transform);
            pageInstance.name = type.ToString() + "Page";
            pageInstances[type] = pageInstance;
        }
        else
        {
            Debug.LogError($"Missing prefab for page type: {type}");
        }
    }

    public void NavigateToPage(PageType pageType)
    {
        // Ignore if already on this page
        if (currentPage == pageType) return;

        // Hide current page
        if (currentPage != PageType.None && pageInstances.ContainsKey(currentPage))
        {
            pageInstances[currentPage].SetActive(false);
        }

        // Show new page
        if (pageInstances.ContainsKey(pageType))
        {
            pageInstances[pageType].SetActive(true);
            currentPage = pageType;
            
            // Notify UIManager that the page has changed
            UIManager.Instance.OnPageChanged(pageType);
        }
        else
        {
            Debug.LogError($"Trying to navigate to non-existent page: {pageType}");
        }
    }

    public PageType GetCurrentPage()
    {
        return currentPage;
    }
}