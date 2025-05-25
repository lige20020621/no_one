// CollectionNotification.cs - 收集提示系統
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CollectionNotification : MonoBehaviour
{
    [Header("通知UI")]
    public GameObject notificationPanel;
    public Text notificationText;
    public float displayDuration = 2f;
    public float fadeSpeed = 0.5f;

    void Start()
    {
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
    }

    public void ShowItemCollected(string itemName)
    {
        if (notificationPanel != null && notificationText != null)
        {
            notificationText.text = "獲得了 " + itemName + "！";
            StartCoroutine(ShowNotification());
        }
    }

    public void ShowAllItemsCollected()
    {
        if (notificationPanel != null && notificationText != null)
        {
            notificationText.text = "所有物品已收集完成！\n準備開始對話...";
            StartCoroutine(ShowNotification());
        }
    }

    IEnumerator ShowNotification()
    {
        notificationPanel.SetActive(true);

        // 淡入效果
        CanvasGroup canvasGroup = notificationPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = notificationPanel.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;
        float elapsed = 0;
        while (elapsed < fadeSpeed)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeSpeed);
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayDuration);

        // 淡出效果
        elapsed = 0;
        while (elapsed < fadeSpeed)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeSpeed);
            yield return null;
        }
        canvasGroup.alpha = 0;

        notificationPanel.SetActive(false);
    }
}