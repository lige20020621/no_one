// DialogueManager.cs - 對話管理器
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManagerLevel1 : MonoBehaviour
{
    public static GameManagerLevel1 instance;

    [Header("物品收集設定")]
    public int totalItemsToCollect = 3; // 總共需要收集的物品數量
    private int collectedItemsCount = 0; // 已收集的物品數量
    private Dictionary<int, bool> collectedItems = new Dictionary<int, bool>();

    [Header("對話系統引用")]
    public NewDialogueManager dialogueManager; // 引用您的NewDialogueManager

    [Header("收集完成後的行為")]
    public bool startDialogueAfterCollection = true; // 收集完成後是否開始對話

    [Header("收集通知")]
    public CollectionNotification notificationSystem; // 可選的通知系統


    [Header("Audio")]

    public AudioClip backgroundMusic;
    public AudioSource audioSource;
    public float backgroundMusicVolume = 0.3f;
    public AudioClip itemBackgroundMusic;
    public AudioSource itemAudioSource;

    [Header("Fire Button")]
    public Button fireButton;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Simplified null checks for IDE0031
        dialogueManager ??= FindObjectOfType<NewDialogueManager>();
        notificationSystem ??= FindObjectOfType<CollectionNotification>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Play background music
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.volume = backgroundMusicVolume;
            audioSource.Play();
        }

        if (itemAudioSource == null)
            itemAudioSource = gameObject.AddComponent<AudioSource>();

      

        // Setup fire button
        SetupFireButton();

    }

    private void SetupFireButton()
    {
        if (fireButton != null)
        {
            // Remove any existing listeners to avoid duplicates
            fireButton.onClick.RemoveAllListeners();

            // Add the fire button click listener
            fireButton.onClick.AddListener(OnFireButtonClicked);

            Debug.Log("Fire button setup complete");
        }
        else
        {
            Debug.LogWarning("Fire button not assigned in inspector!");
        }
    }

    public void OnFireButtonClicked()
    {
        Debug.Log("Fire button clicked! Changing to scene 5 with content parameter");

        // Change to scene 5 with "content" parameter
        ChangeSceneManager.Instance.onChangeScene(5, "content", "雖然糯米燒掉了藤曼，但真正原因問未被挖掘，土地的聲音仍然未能被傾聽...");
    }

    public void ItemCollected(Item item)
    {
        // 檢查物品是否已經被收集
        if (!collectedItems.ContainsKey(item.itemID) || !collectedItems[item.itemID])
        {
            // Play background music
            if (itemBackgroundMusic != null)
            {
                itemAudioSource.clip = itemBackgroundMusic;
                itemAudioSource.loop = false;
                itemAudioSource.volume = 0.3f;
                itemAudioSource.Play();
            }
            // 記錄物品已被收集
            collectedItems[item.itemID] = true;
            collectedItemsCount++;

            Debug.Log("已收集 " + collectedItemsCount + " 個物品，共 " + totalItemsToCollect + " 個");

            // 顯示收集通知
            if (notificationSystem != null)
            {
                notificationSystem.ShowItemCollected(item.itemName);
            }

            // 檢查是否已收集所有物品
            if (collectedItemsCount >= totalItemsToCollect)
            {
                // 所有物品都已收集，開始對話
                OnAllItemsCollected();
            }
        }
    }

    // 當所有物品被收集時調用
    void OnAllItemsCollected()
    {
        Debug.Log("所有物品已收集完成！");

        // 顯示完成通知
        if (notificationSystem != null)
        {
            notificationSystem.ShowAllItemsCollected();
        }

        if (startDialogueAfterCollection && dialogueManager != null)
        {
            // 稍微延遲一下再開始對話，讓通知有時間顯示
            Invoke("StartDialogueSequence", 0.5f);
        }
        else
        {
            // 如果沒有對話系統，可以在這裡添加其他完成後的行為
            Debug.Log("遊戲完成！");
        }
    }

    void StartDialogueSequence()
    {
        // 停止玩家移動
        PlayerMover player = FindObjectOfType<PlayerMover>();
        if (player != null)
        {
            player.DisableMovement();
        }

        // 觸發指定的對話序列
        if (dialogueManager != null)
        {
            dialogueManager.StartCompleteDialogue();
        }
    }

    // 檢查對話是否正在進行
    public bool IsDialogueActive()
    {
        if (dialogueManager != null && dialogueManager.dialoguePanel != null)
        {
            return dialogueManager.dialoguePanel.activeSelf;
        }
        return false;
    }

    public bool HasCollectedItem(int itemID)
    {
        return collectedItems.ContainsKey(itemID) && collectedItems[itemID];
    }

    public bool HasCollectedAllItems()
    {
        return collectedItemsCount >= totalItemsToCollect;
    }
     
    public int GetCollectedItemsCount()
    {
        return collectedItemsCount;
    }

    public void GoToScene(int sceneIndex)
    {
        ChangeSceneManager.Instance.onChangeScene(sceneIndex);
    }
    // 第二個對話功能
    public void StartSecondaryDialogue()
    {
        PlayerMover player = FindObjectOfType<PlayerMover>();
        if (player != null)
        {
            player.DisableMovement();
        }

        if (dialogueManager != null)
        {
            dialogueManager.StartSecondaryDialogue(); // 開始第二組對話
        }
    }
}