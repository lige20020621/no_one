using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialogueManagerLevel02 : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite playerTalk00;
    public Sprite playerTalk01;
    public Sprite npc1Talk;
    public Sprite npc2Talk;

    [Header("Backgrounds")]
    public Sprite bgImage;

    [Header("UI References")]
    public Text dialogueText;           // UI 文字元素
    public Text speakerNameText;        // 說話者名字的UI元素
    public Image playerImage;           // 玩家圖像
    public Image npc1Image;              // NPC1圖像
    public Image npc2Image;              // NPC2圖像
    public Image dialogueBlackBg;
    public GameObject backgroundImage;  // 背景圖像
    public GameObject dialoguePanel;    // 對話面板
    public GameObject resultPanel;    // 對話面板

    private NewDialogueBlock[] firstDialogue;  // 當前使用的對話序列
    private NewDialogueBlock[] secondaryDialogue;  // 當前使用的對話序列
    private NewDialogueBlock[] currentDialogue;  // 當前正在使用的對話序列 
    private int currentBlockIndex = 0;
    private int currentDialogueIndex = 0;
    private bool isDisplayingText = false;
    private string fullText = "";
    private PlayerControllerLevel02 playerMover;
    private Coroutine typingCoroutine;

    [Header("Typing Audio")]
    public AudioClip typingSound;
    public AudioSource typingAudioSource;
    public float typingVolume = 0.5f;
    public bool playTypingSoundOnEveryChar = false; // If false, plays continuously while typing

    [Header("Next Block Audio")]
    public AudioClip nextBlockSound;
    public AudioSource nextBlockAudioSource;
    public float nextBlockVolume = 0.5f;

    private void Start()
    {

        // 在Start中初始化完整對話序列
        InitializeAllDialogues();
        SetupAudio();
        StartFirstDialogue();
     
    }

    void SetupAudio()
    { 
        // Setup typing audio source
        if (typingAudioSource == null)
        {
            GameObject typingGO = new GameObject("TypingAudioSource");
            typingGO.transform.SetParent(transform);
            typingAudioSource = typingGO.AddComponent<AudioSource>();
        }
        typingAudioSource.loop = true; // For continuous typing sound
        typingAudioSource.volume = typingVolume;

        // Setup next block audio source
        if (nextBlockAudioSource == null)
        {
            GameObject nextBlockGO = new GameObject("NextBlockAudioSource");
            nextBlockGO.transform.SetParent(transform);
            nextBlockAudioSource = nextBlockGO.AddComponent<AudioSource>();
        }
        nextBlockAudioSource.loop = false;
        nextBlockAudioSource.volume = nextBlockVolume;

    }

    void PlayNextBlockSound()
    {
        if (nextBlockSound != null && nextBlockAudioSource != null)
        {
            nextBlockAudioSource.PlayOneShot(nextBlockSound);
        }
    }

    void StartTypingSound()
    {
        if (typingSound != null && typingAudioSource != null)
        {
            if (!playTypingSoundOnEveryChar)
            {
                Debug.Log("StartTypingSound......+++");
                // Play continuous typing sound
                typingAudioSource.clip = typingSound;
                typingAudioSource.Play();
            }
        }
    }

    void StopTyping()
    {
        if (typingAudioSource != null && typingAudioSource.isPlaying)
        {
            typingAudioSource.Stop();
        }
    }

    // 初始化完整對話序列
    private void InitializeAllDialogues()
    {
        firstDialogue = new NewDialogueBlock[]
            {
                // 與藤蔓的對話部分
                new NewDialogueBlock{
                    text = "這裏有一本筆記本呢...？\n真奇怪...怎麽隨便亂丟呀...\n“打開看看裏面寫了什麽吧.",
                   speaker = SpeakerType.Player,
                    speakerName = "糯米",
                    speakerSprite = new Sprite[]{ playerTalk00, playerTalk01 },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                new NewDialogueBlock{
                    text = "我們...做錯了嗎？...我們只是想讓自己和家人不再挨餓.\n我們犯下了錯誤...已經...已經無法彌補了...嗎?\n孩子他爸出海...再也沒有回來...有...沒有人可以救救我們.",
                    speaker = SpeakerType.Narrator,
                    speakerName = "日記",
                    speakerSprite = new Sprite[]{ },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                 new NewDialogueBlock{
                    text = "孩子他爸出海...再也沒有回來...有...沒有人可以救救我們.",
                    speaker = SpeakerType.Narrator,
                    speakerName = "日記",
                    speakerSprite = new Sprite[]{ },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },

            };
        secondaryDialogue = new NewDialogueBlock[]
           {
                // 與藤蔓的對話部分
                new NewDialogueBlock{
                    text = "...",
                    speaker = SpeakerType.NPC1,
                    speakerName = "巨章",
                    speakerSprite = new Sprite[]{ npc1Talk },
                    spriteIndex = 0 ,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                new NewDialogueBlock{
                    text = "...",
                    speaker = SpeakerType.NPC2,
                    speakerName = "村民",
                    speakerSprite = new Sprite[]{ npc2Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                 new NewDialogueBlock{
                    text = "你們好呀...\n我想你們之間是不是有什麽誤會呢...？",
                    speaker = SpeakerType.Player,
                    speakerName = "糯米",
                    speakerSprite = new Sprite[]{ playerTalk00, playerTalk01 },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                   new NewDialogueBlock{
                    text = "對不起我們犯下了...犯下了...無法彌補的錯誤...",
                    speaker = SpeakerType.NPC2,
                    speakerName = "村民",
                    speakerSprite = new Sprite[]{ npc2Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                   new NewDialogueBlock{
                    text = "......\n你們...你們獵殺我們...",
                    speaker = SpeakerType.NPC1,
                    speakerName = "巨章",
                    speakerSprite = new Sprite[]{ npc1Talk },
                    spriteIndex = 0 ,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                   new NewDialogueBlock{
                    text = "對不起...一開始我們只是想要裹腹...後來...只是後來.\n有一天...有人來到我們村子...",
                    speaker = SpeakerType.NPC2,
                    speakerName = "村民",
                    speakerSprite = new Sprite[]{ npc2Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                    new NewDialogueBlock{
                    text = "想要高價購買巨型章魚的牙齒那是很多很多的錢...我們是一個很貧困的村子...所以...",
                    speaker = SpeakerType.NPC2,
                    speakerName = "村民",
                    speakerSprite = new Sprite[]{ npc2Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                    new NewDialogueBlock{
                    text = "我們很生氣...但...那已經是很久很久以前了.\n這片土地除了亡魂...已經沒有人了.\n罷了...一切都只是回到...回到一開始最原本的樣子...\n小家夥你該走了.",
                    speaker = SpeakerType.NPC1,
                    speakerName = "巨章",
                    speakerSprite = new Sprite[]{ npc1Talk },
                    spriteIndex = 0 ,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                     new NewDialogueBlock{
                    text = "小家夥你該走了.",
                    speaker = SpeakerType.NPC1,
                    speakerName = "巨章",
                    speakerSprite = new Sprite[]{ npc1Talk },
                    spriteIndex = 0 ,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },

           };
    }

    public void StartFirstDialogue()
    {
        currentDialogue = firstDialogue;  // 設置當前對話序列
        currentBlockIndex = 0;
        currentDialogueIndex = 0;

        // 停用玩家移動
        playerMover = FindObjectOfType<PlayerControllerLevel02>();
        if (playerMover != null)
        {
            playerMover.DisableMovement();
        }
        DisplayCurrentDialogueBlock();
    }

    // 開始指定的對話序列

    public void StartSecondaryDialogue()
    {
        currentDialogue = secondaryDialogue;  // 設置當前對話序列
        currentBlockIndex = 0;
        currentDialogueIndex = 1;
        dialoguePanel.SetActive(true);

        // 停用玩家移動
        playerMover = FindObjectOfType<PlayerControllerLevel02>();
        if (playerMover != null)
        {
            playerMover.DisableMovement();
        }
        DisplayCurrentDialogueBlock();
    }


    // 顯示當前對話塊
    private void DisplayCurrentDialogueBlock()
    {
        if (currentBlockIndex < currentDialogue.Length)
        {
            NewDialogueBlock block = currentDialogue[currentBlockIndex];
            fullText = block.text;

            // 更新UI元素
            speakerNameText.text = block.speakerName;

            // 設置不同角色的位置
            if (block.speaker == SpeakerType.Player)
            {
                npc1Image.gameObject.SetActive(false);
                npc2Image.gameObject.SetActive(false);
                playerImage.gameObject.SetActive(true);
            }
            else if (block.speaker == SpeakerType.NPC1)
            {
                npc1Image.gameObject.SetActive(true);
                npc2Image.gameObject.SetActive(false);
                playerImage.gameObject.SetActive(false);
            } else if (block.speaker == SpeakerType.NPC2)
            {
                npc1Image.gameObject.SetActive(false);
                npc2Image.gameObject.SetActive(true);
                playerImage.gameObject.SetActive(false);
            }
            else if (block.speaker == SpeakerType.Narrator)
            {
                // 旁白時隱藏所有角色圖像
                npc1Image.gameObject.SetActive(false);
                npc2Image.gameObject.SetActive(false);
                playerImage.gameObject.SetActive(false);
            }

            // 處理角色圖像
            if (block.speakerSprite != null && block.speakerSprite.Length > 0)
            {
                int index = Mathf.Clamp(block.spriteIndex, 0, block.speakerSprite.Length - 1);

                if (block.speaker == SpeakerType.Player)
                {
                    playerImage.sprite = block.speakerSprite[index];
                }
                else if (block.speaker == SpeakerType.NPC1)
                {
                    npc1Image.sprite = block.speakerSprite[index];
                }
                else if (block.speaker == SpeakerType.NPC2)
                {
                    npc2Image.sprite = block.speakerSprite[index];
                }
            }

            // 處理背景圖
            if (backgroundImage != null && block.backgroundSprite != null)
            {
                backgroundImage.SetActive(true);
            }

            // 開始逐字顯示文字效果
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText(block.displaySpeed));
        }
        else
        {
            // 對話結束
            EndDialogue();
        }
    }

    // 逐字顯示文字的協程
    private IEnumerator TypeText(float typingSpeed)
    {
        isDisplayingText = true;
        dialogueText.text = "";


        // Start typing sound
        StartTypingSound();

        foreach (char letter in fullText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Stop typing sound
        StopTyping();

        isDisplayingText = false;
        typingCoroutine = null;
    }

    // 使用者輸入處理
    private void Update()
    {

        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            PlayNextBlockSound();
            if (isDisplayingText)
            {
                // 如果文字還在打字中，則立即顯示全部文字
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }
                StopTyping(); 
                dialogueText.text = fullText;
                isDisplayingText = false;
            }
            else
            {
                // 顯示下一段對話
                currentBlockIndex++;
                DisplayCurrentDialogueBlock();
            }
        }
    }

    // 結束對話
    private void EndDialogue()
    {
        Debug.LogWarning("EndDialogue");

        // 立即停止所有可能仍在運行的協程
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // 隱藏所有UI元素
        dialoguePanel.SetActive(false);
        npc1Image.gameObject.SetActive(false);
        npc2Image.gameObject.SetActive(false);
        playerImage.gameObject.SetActive(false);

        if (currentDialogueIndex == 1)
        {
            // 第二段對話結束 - 延遲一幀再執行場景切換
            StartCoroutine(DelayedSceneChange());
        }
        else
        {
            dialogueBlackBg.enabled = false;
            // 第一段對話結束 - 延遲一幀再啟用玩家移動
            StartCoroutine(DelayedEnableMovement());
        }
    }

    private IEnumerator DelayedEnableMovement()
    {
        yield return null; // 等待一幀

        if (playerMover != null)
        {
            playerMover.EnableMovement();
        }
    }

    // 延遲場景切換
    private IEnumerator DelayedSceneChange()
    {
        yield return null; // 等待一幀

        ChangeSceneManager.Instance.onChangeScene(4);
    }

    // 檢查對話是否正在進行（供GameManager使用）
    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }
}