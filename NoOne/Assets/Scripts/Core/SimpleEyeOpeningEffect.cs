 using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleEyeOpeningEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    public float fadeDuration = 2f;     // 漸變時間
    public float startDelay = 0.5f;     // 開始延遲
    public bool autoStart = false;      // 改為 false - 手動控制開始

    [Header("Advanced Options")]
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Color overlayColor = Color.black; // 遮罩顏色

    [Header("Debug")]
    public bool enableDebugLogs = false;

    private Canvas overlayCanvas;
    private Image blackScreen;
    private bool effectCompleted = false;
    private bool isCreatingOverlay = false;

    void Start()
    {
        // Add a small delay to ensure everything is properly initialized
        StartCoroutine(InitializeWithDelay());
    }

    IEnumerator InitializeWithDelay()
    {
        yield return new WaitForEndOfFrame(); // Wait for one frame

        CreateBlackOverlay();

        if (autoStart)
        {
            StartEyeOpening();
        }
    }

    void CreateBlackOverlay()
    {
        if (isCreatingOverlay)
        {
            if (enableDebugLogs) Debug.Log("SimpleEyeOpeningEffect: Already creating overlay, skipping...");
            return;
        }

        isCreatingOverlay = true;

        try
        {
            // 清理舊的 Canvas（如果存在）
            if (overlayCanvas != null)
            {
                Destroy(overlayCanvas.gameObject);
                overlayCanvas = null;
                blackScreen = null;
            }

            // 創建 Canvas
            GameObject canvasGO = new GameObject("EyeOpeningCanvas");

            // 確保不會被銷毀
            DontDestroyOnLoad(canvasGO);

            overlayCanvas = canvasGO.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = 9999; // 確保在最上層

            // 添加 CanvasScaler (可選，但推薦)
            CanvasScaler canvasScaler = canvasGO.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            // 添加 GraphicRaycaster
            canvasGO.AddComponent<GraphicRaycaster>();

            // 創建黑色遮罩 Image
            GameObject imageGO = new GameObject("BlackOverlay");
            imageGO.transform.SetParent(canvasGO.transform, false);

            blackScreen = imageGO.AddComponent<Image>();
            blackScreen.color = overlayColor;

            // 設置為全螢幕
            RectTransform rectTransform = imageGO.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;

            if (enableDebugLogs) Debug.Log("SimpleEyeOpeningEffect: Black overlay created successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError("SimpleEyeOpeningEffect: Error creating overlay: " + e.Message);
        }
        finally
        {
            isCreatingOverlay = false;
        }
    }


    public void StartEyeOpening()
    {
        // Ensure overlay exists before starting
        if (blackScreen == null)
        {
            if (enableDebugLogs) Debug.Log("SimpleEyeOpeningEffect: Black screen not found, creating overlay...");
            CreateBlackOverlay();

            // Wait a frame for creation to complete
            StartCoroutine(StartEyeOpeningDelayed());
            return;
        }

        if (!effectCompleted)
        {
            StartCoroutine(PlayFadeEffect());
        }
    }

    IEnumerator StartEyeOpeningDelayed()
    {
        yield return new WaitForEndOfFrame();

        if (blackScreen != null && !effectCompleted)
        {
            StartCoroutine(PlayFadeEffect());
        }
        else
        {
            Debug.LogError("SimpleEyeOpeningEffect: Failed to create black screen even after retry!");
        }
    }

    IEnumerator PlayFadeEffect()
    {
        // Double-check that blackScreen exists
        if (blackScreen == null)
        {
            Debug.LogError("SimpleEyeOpeningEffect: Black screen not available!");
            yield break;
        }

        // 等待開始延遲
        yield return new WaitForSeconds(startDelay);

        if (enableDebugLogs) Debug.Log("SimpleEyeOpeningEffect: Starting fade effect");

        float elapsedTime = 0f;
        Color startColor = blackScreen.color;
        Color endColor = startColor;
        endColor.a = 0f;

        while (elapsedTime < fadeDuration && blackScreen != null)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;

            // 使用動畫曲線
            float curveValue = fadeCurve.Evaluate(progress);

            // 漸變透明度
            Color currentColor = Color.Lerp(startColor, endColor, curveValue);
            blackScreen.color = currentColor;

            yield return null;
        }

        // 確保完全透明
        if (blackScreen != null)
        {
            blackScreen.color = endColor;
        }

        // 完全隱藏並清理
        if (overlayCanvas != null)
        {
            overlayCanvas.gameObject.SetActive(false);
        }

        effectCompleted = true;

        if (enableDebugLogs) Debug.Log("SimpleEyeOpeningEffect: Fade effect completed");

        // 可選：完全銷毀 Canvas
        if (overlayCanvas != null)
        {
            Destroy(overlayCanvas.gameObject, 1f);
        }
    }

    // 重置效果
    public void ResetEffect()
    {
        effectCompleted = false;

        if (overlayCanvas != null)
        {
            Destroy(overlayCanvas.gameObject);
            overlayCanvas = null;
            blackScreen = null;
        }

        CreateBlackOverlay();

        if (enableDebugLogs) Debug.Log("SimpleEyeOpeningEffect: Effect reset");
    }

    // 立即設為透明（跳過動畫）
    public void SkipToOpen()
    {
        if (blackScreen == null)
        {
            CreateBlackOverlay();
            if (blackScreen == null)
            {
                Debug.LogError("SimpleEyeOpeningEffect: Cannot skip - black screen creation failed!");
                return;
            }
        }

        Color color = blackScreen.color;
        color.a = 0f;
        blackScreen.color = color;

        if (overlayCanvas != null)
        {
            overlayCanvas.gameObject.SetActive(false);
        }

        effectCompleted = true;
    }

    // 立即設為黑色
    public void SetToBlack()
    {
        if (blackScreen == null)
        {
            CreateBlackOverlay();
        }

        if (overlayCanvas != null)
        {
            overlayCanvas.gameObject.SetActive(true);
        }

        if (blackScreen != null)
        {
            Color color = blackScreen.color;
            color.a = 1f;
            blackScreen.color = color;
        }

        effectCompleted = false;
    }

    // Public method to check if overlay is ready
    public bool IsOverlayReady()
    {
        return blackScreen != null && overlayCanvas != null;
    }

    // Method to force recreation of overlay
    public void ForceRecreateOverlay()
    {
        if (enableDebugLogs) Debug.Log("SimpleEyeOpeningEffect: Force recreating overlay...");

        if (overlayCanvas != null)
        {
            Destroy(overlayCanvas.gameObject);
            overlayCanvas = null;
            blackScreen = null;
        }

        CreateBlackOverlay();
    }

    void OnDestroy()
    {
        // 清理
        if (overlayCanvas != null)
        {
            Destroy(overlayCanvas.gameObject);
        }
    }

    // Debug method to check status
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugStatus()
    {
        Debug.Log($"SimpleEyeOpeningEffect Status:");
        Debug.Log($"- overlayCanvas: {(overlayCanvas != null ? "Valid" : "NULL")}");
        Debug.Log($"- blackScreen: {(blackScreen != null ? "Valid" : "NULL")}");
        Debug.Log($"- effectCompleted: {effectCompleted}");
        Debug.Log($"- isCreatingOverlay: {isCreatingOverlay}");
    }
}