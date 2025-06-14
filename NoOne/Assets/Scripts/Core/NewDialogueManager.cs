using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SpeakerType
{
    Player,
    NPC1,
    NPC2,
    NPC3,
    NPC4,
    Narrator // 旁白，沒有特定說話者
}

[System.Serializable] // 讓它可以在Inspector中顯示和編輯
public class NewDialogueBlock
{
    public string text;                  // 對話內容
    public SpeakerType speaker;          // 誰在說話
    public Sprite[] speakerSprite;       // 說話者的圖像陣列
    public int spriteIndex = 0;          // 預設使用的圖像索引
    public Sprite backgroundSprite;      // 背景圖
    public bool hideCharacter;           // 是否隱藏人物
    public string speakerName;           // 說話者的名字（可選，用於顯示）
    public float displaySpeed = 0.1f;   // 文字顯示速度（可選）
}

[System.Serializable]
public class DialogueSequence
{
    public string sequenceName;          // 對話序列的名稱
    public NewDialogueBlock[] blocks;    // 對話內容
}

public class NewDialogueManager : MonoBehaviour
{
    [Header("Multiple Dialogue Sequences")]
    public DialogueSequence[] dialogueSequences;  // 多個對話序列

    [Header("Quick Access Dialogues")]
    public NewDialogueBlock[] defaultDialogue;     // 預設對話（向下兼容）
    public NewDialogueBlock[] secondaryDialogue;  // 第二組對話

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
    public Image npc1Image;              // NPC圖像
    public Image npc2Image;              // NPC圖像
    public GameObject backgroundImage;  // 背景圖像
    public SpriteRenderer backgroundRenderer; // 背景的SpriteRenderer (會在Start中自動查找)
    public GameObject dialoguePanel;    // 對話面板
    public GameObject resultPanel;    // 對話面板

    private NewDialogueBlock[] currentDialogue;  // 當前使用的對話序列
    private int currentBlockIndex = 0;
    private int currentDialogueIndex = 0;
    private bool isDisplayingText = false;
    private string fullText = "";
    private PlayerMover playerMover;
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
        // 確保對話面板一開始是隱藏的
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // 如果提供了背景GameObject，嘗試獲取其SpriteRenderer
        if (backgroundImage != null && backgroundRenderer == null)
        {
            backgroundRenderer = backgroundImage.GetComponent<SpriteRenderer>();

            // 如果沒有SpriteRenderer，添加一個
            if (backgroundRenderer == null)
            {
                backgroundRenderer = backgroundImage.AddComponent<SpriteRenderer>();
                Debug.Log("Added SpriteRenderer to background GameObject");
            }
        }
        else if (backgroundImage == null)
        {
            Debug.LogWarning("Background GameObject not assigned! Background changes will not work.");
        }

        // 在Start中初始化完整對話序列
        InitializeAllDialogues();
        SetupAudio();
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
            StopTyping();
            nextBlockAudioSource.PlayOneShot(nextBlockSound);
        }
    }

    void StartTypingSound()
    {
        if (typingSound != null && typingAudioSource != null)
        {
            if (!playTypingSoundOnEveryChar)
            {
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
        // 如果在Inspector中已經設置了對話序列，則不需要在這裡初始化
        if (defaultDialogue == null || defaultDialogue.Length == 0)
        {
            defaultDialogue = new NewDialogueBlock[]
            {
                // 與藤蔓的對話部分
                new NewDialogueBlock{
                    text = "已經好久沒人來了呢...\n看來是個小家夥呢...",
                    speaker = SpeakerType.NPC1,
                    speakerName = "藤曼",
                    speakerSprite = new Sprite[]{ npc1Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                new NewDialogueBlock{
                    text = "你...你好哇...\n藤曼先生你看起來好大呢...\n我是糯米！這裏是哪裏呀？",
                    speaker = SpeakerType.Player,
                    speakerName = "糯米",
                    speakerSprite = new Sprite[]{ playerTalk00, playerTalk01 },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                new NewDialogueBlock{
                    text = "你好...糯米...真是個可愛的小家夥..\n這裏在很久以前...可是生機盎然...\n這裏是曾經是一片農田呢...哈哈哈..哈...",
                    speaker = SpeakerType.NPC1,
                    speakerName = "藤曼",
                    speakerSprite = new Sprite[]{ npc1Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                
                // 與山藥的對話部分
                new NewDialogueBlock{
                    text = "#！~￥%#&*）+@@&**~%......\n#@~&*&*+&@！...%￥......",
                    speaker = SpeakerType.NPC2,
                    speakerName = "山藥",
                    speakerSprite = new Sprite[]{ npc2Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                new NewDialogueBlock{
                    text = "你...你看起來很像我的山藥玩偶...\n嗯...山藥先生在念咒語嗎？\n我是糯米！",
                    speaker = SpeakerType.Player,
                    speakerName = "糯米",
                    speakerSprite = new Sprite[]{ playerTalk00, playerTalk01 },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                new NewDialogueBlock{
                    text = "#！~￥%#&......\n哼，可惡的人類小孩嗎...?\n我恨...這片￥%#土地曾經居住的人...",
                    speaker = SpeakerType.NPC2,
                    speakerName = "山藥",
                    speakerSprite = new Sprite[]{ npc2Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                new NewDialogueBlock{
                    text = "我和我的朋友們...瘋狂#！~...瘋狂的...被迫長大...\n農場主人聽信可怕的商人...魔法藥水+&@！\n這片土地被喂養了奇怪的%#東西..有什麽正在失控&**~%...早已失控.",
                    speaker = SpeakerType.NPC2,
                    speakerName = "山藥",
                    speakerSprite = new Sprite[]{ npc2Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                new NewDialogueBlock{
                    text = "可憐人類的小孩啊..這是被傳承的..詛咒的土地呢#$!...哈....\n夜晚&*^%~...夜晚不要踏進農田...\n否則...我將會來^%$*!~索命...",
                    speaker = SpeakerType.NPC2,
                    speakerName = "山藥",
                    speakerSprite = new Sprite[]{ npc2Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                }
            };
        }

        if (secondaryDialogue == null || secondaryDialogue.Length == 0)
        {
            secondaryDialogue = new NewDialogueBlock[]
            {
                new NewDialogueBlock{
                    text = "#！~嗚嗚*）+@@&*嗚嗚嗚~%......\n#@~&*嗚嗚嗚！...%￥......",
                    speaker = SpeakerType.NPC2,
                    speakerName = "山藥",
                    speakerSprite = new Sprite[]{ npc2Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                new NewDialogueBlock{
                    text = "山藥先生...你還好嗎？\n你爲什麽在哭呀...發生了什麽？",
                    speaker = SpeakerType.Player,
                    speakerName = "糯米",
                    speakerSprite = new Sprite[]{ playerTalk00, playerTalk01 },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                new NewDialogueBlock{
                    text = "一開始只*&￥是生氣......\n我們被藥水操縱...*&#...被迫長大…瘋狂長大...好久...久",
                    speaker = SpeakerType.NPC2,
                    speakerName = "山藥",
                    speakerSprite = new Sprite[]{ npc2Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                 new NewDialogueBlock{
                    text = "現在已經沒事了...沒有人擁有魔法藥水了...",
                    speaker = SpeakerType.Player,
                    speakerName = "糯米",
                    speakerSprite = new Sprite[]{ playerTalk00, playerTalk01 },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
                  new NewDialogueBlock{
                    text = "謝謝你...糯米......我們%@!~經歷了長久...長久的絕望\n一直...一直都...沒有人傾聽這片土地的聲音...\n謝謝你...糯米......",
                    speaker = SpeakerType.NPC2,
                    speakerName = "山藥",
                    speakerSprite = new Sprite[]{ npc2Talk },
                    spriteIndex = 0,
                    backgroundSprite = bgImage,
                    hideCharacter = false
                },
            };
        }
    }

    public void StartCompleteDialogue()
    {
        StartDialogue(defaultDialogue);
    }

    // 開始指定的對話序列
    public void StartDialogue(NewDialogueBlock[] dialogue)
    {
        if (dialogue == null || dialogue.Length == 0)
        {
            Debug.LogWarning("嘗試開始空的對話序列！");
            return;
        }

        currentDialogue = dialogue;
        currentBlockIndex = 0;
        dialoguePanel.SetActive(true);

        // 停用玩家移動
        playerMover = FindObjectOfType<PlayerMover>();
        if (playerMover != null)
        {
            playerMover.DisableMovement();
        }

        DisplayCurrentDialogueBlock();
    }


    // 開始指定名稱的對話序列
    public void StartDialogueByName(string sequenceName)
    {
        if (dialogueSequences != null)
        {
            foreach (DialogueSequence sequence in dialogueSequences)
            {
                if (sequence.sequenceName == sequenceName)
                {
                    StartDialogue(sequence.blocks);
                    return;
                }
            }
        }
        Debug.LogWarning("找不到名為 '" + sequenceName + "' 的對話序列！");
    }

    public void StartDialogueByIndex(int index)
    {
        if (dialogueSequences != null && index >= 0 && index < dialogueSequences.Length)
        {
            StartDialogue(dialogueSequences[index].blocks);
        }
        else
        {
            Debug.LogWarning("對話序列索引 " + index + " 超出範圍！");
        }
    }

    // 快速存取方法
    public void StartDefaultDialogue()
    {
        StartDialogue(defaultDialogue);
    }

    public void StartSecondaryDialogue()
    {
        currentDialogueIndex = 1;
        resultPanel.SetActive(false);
        StartDialogue(secondaryDialogue);
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
            }
            else if (block.speaker == SpeakerType.NPC2)
            {
                npc2Image.gameObject.SetActive(true);
                npc1Image.gameObject.SetActive(false);
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
            if (backgroundRenderer != null && backgroundImage != null && block.backgroundSprite != null)
            {
                backgroundRenderer.sprite = block.backgroundSprite;
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
        if(currentDialogueIndex == 1)
        {
            ChangeSceneManager.Instance.onChangeScene(3);
        } else
        {
            dialoguePanel.SetActive(false);
            npc1Image.gameObject.SetActive(false);
            npc2Image.gameObject.SetActive(false);
            playerImage.gameObject.SetActive(false);
            // 只有在resultPanel存在時才顯示
            if (resultPanel != null)
            {
                resultPanel.SetActive(true);
            }
            else
            {
                // 如果沒有結果面板，重新啟用玩家移動
                if (playerMover != null)
                {
                    playerMover.EnableMovement();
                }
            }

            // 停止所有可能仍在運行的協程
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
        }
       
    }

    // 保持向後兼容性的方法（可選）
    public void TriggerNPCDialogue(int npcId)
    {
        // 現在不管傳入什麼ID都開始完整對話
        StartCompleteDialogue();
    }

    // 檢查對話是否正在進行（供GameManager使用）
    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }
}