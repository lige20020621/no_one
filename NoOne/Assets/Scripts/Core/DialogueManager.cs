using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class DialogueBlock
{
    public string[] texts;                // 一組對話內容
    public Sprite[] characterSprites;     // 人物圖輪播
    public Sprite backgroundSprite;       // 背景圖
    public bool hideCharacter;            // 是否隱藏人物
}

public class DialogueManager : MonoBehaviour
{
    [Header("Fade Control")]
    public CanvasGroup characterCanvasGroup; // Attach to the character Image
    public CanvasGroup dialogueBoxCanvasGroup; // Attach to the dialogue box

    [Header("Sprites")]
    public Sprite talk00;
    public Sprite talk01;
    public Sprite talk000;
    public Sprite talk001;
    public Sprite talk002;
    public Sprite talk003;
    public Sprite talk004;
    public Sprite talk005;
    public Sprite talk006;

    [Header("Backgrounds")]
    public Sprite bgRoom;
    public Sprite bgKitchen;
    public Sprite bgKitchenFire;
    public Sprite bgLevel00;

    [Header("UI References")]
    public GameObject dialogueBox;
    public Text dialogueText;
    public Image characterImage;
    public Image backgroundImage;

    [Header("Choice Menu")]
    public GameObject choiceMenu; // Yes/No選單
    public Button yesButton;
    public Button noButton;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.03f;

    [Header("Dialogue Blocks")]
    public DialogueBlock[] dialogueBlocks;

    private int currentBlockIndex = 0;
    private int currentTextIndex = 0;
    private bool isTyping;
    private Coroutine spriteSwitchCoroutine;

    void Start()
    {
        characterCanvasGroup.alpha = 0;
        dialogueBoxCanvasGroup.alpha = 0;
        characterImage.gameObject.SetActive(true);
        dialogueBox.SetActive(true);

        currentBlockIndex = 0;
        currentTextIndex = 0;
        SetupDialogueBlocks();
        StartCoroutine(FadeInSequence());
    }


    void Update()
    {
        if (dialogueBox.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = dialogueBlocks[currentBlockIndex].texts[currentTextIndex];
                isTyping = false;
                if (spriteSwitchCoroutine != null)
                {
                    StopCoroutine(spriteSwitchCoroutine);
                }
            }
            else
            {
                currentTextIndex++;
                if (currentTextIndex < dialogueBlocks[currentBlockIndex].texts.Length)
                {
                    StartCoroutine(TypeLine());
                }
                else
                {
                    currentBlockIndex++;
                    currentTextIndex = 0;
                    if (currentBlockIndex < dialogueBlocks.Length)
                    {
                        StartCoroutine(TypeLine());
                    }
                    else
                    {
                        dialogueBox.SetActive(false);
                        ShowChoiceMenu();
                    }
                }
            }
        }
    }

    IEnumerator FadeInSequence()
    {
        yield return new WaitForSeconds(0.5f); // Wait 1 second
        yield return StartCoroutine(FadeCanvasGroup(characterCanvasGroup, 0f, 1f, 0.75f)); // Fade in player over 1 sec

        yield return new WaitForSeconds(0.5f); // Wait another second
        yield return StartCoroutine(FadeCanvasGroup(dialogueBoxCanvasGroup, 0f, 1, 0.75f)); // Fade in dialogue box over 1 sec

        yield return new WaitForSeconds(0.5f); // Wait 1 second
        StartCoroutine(TypeLine()); // Start showing dialogue
    }
    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        cg.alpha = start;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
    }
    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        DialogueBlock block = dialogueBlocks[currentBlockIndex];

        // 設定背景
        if (block.backgroundSprite != null)
        {
            backgroundImage.sprite = block.backgroundSprite;
        }

        // 設定人物
        if (block.hideCharacter)
        {
            characterImage.gameObject.SetActive(false);
        }
        else
        {
            characterImage.gameObject.SetActive(true);
            if (block.characterSprites != null && block.characterSprites.Length > 0)
            {
                if (spriteSwitchCoroutine != null)
                {
                    StopCoroutine(spriteSwitchCoroutine);
                }
                spriteSwitchCoroutine = StartCoroutine(SwitchCharacterSprites(block.characterSprites));
            }
        }

        foreach (char c in block.texts[currentTextIndex].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if (spriteSwitchCoroutine != null)
        {
            StopCoroutine(spriteSwitchCoroutine);
            spriteSwitchCoroutine = null;
        }
    }

    IEnumerator SwitchCharacterSprites(Sprite[] sprites)
    {
        int index = 0;
        while (isTyping)
        {
            characterImage.sprite = sprites[index];
            index = (index + 1) % sprites.Length;
            yield return new WaitForSeconds(0.3f);
        }
    }

    void SetupDialogueBlocks()
    {
        dialogueBlocks = new DialogueBlock[]
        {
            new DialogueBlock{
                texts = new string[]{
                    "你好...我叫糯米\n很高興認識你...",
                    "爸爸媽媽已經很久沒回家了\n今天保姆姐姐也有事不能來陪我...\n今天只有我們呢...",
                },
                characterSprites = new Sprite[]{ talk01, talk00 },
                backgroundSprite = bgRoom,
                hideCharacter = false
            },
            new DialogueBlock{
                texts = new string[]{ "二人世界...開玩笑的啦." },
                characterSprites = new Sprite[]{ talk003, talk001  },
                backgroundSprite = bgRoom,
                hideCharacter = false
            },
            new DialogueBlock{
                texts = new string[]{
                    "我有好多好多玩偶！\n爸爸媽媽不在的時候他們會陪著我...\n他們是沒有人時的好朋友...",
                },
                characterSprites = new Sprite[]{  talk003, talk001  },
                backgroundSprite = bgRoom,
                hideCharacter = false
            },
             new DialogueBlock{
                texts = new string[]{
                     "他們毛茸茸的超可愛..我猜...你也一定會喜歡...\n我來向你介紹他們吧...",
                },
                characterSprites = new Sprite[]{ talk005 },
                backgroundSprite = bgRoom,
                hideCharacter = false
            },
            new DialogueBlock{
                texts = new string[]{
                    "你看你看我有好多熊熊玩偶...\n在我很舒服的小床上有...\n章魚...山藥...睡着的小熊www\n床旁邊還有可愛小蛇和小貓...",
                },
                characterSprites = null,
                backgroundSprite = bgRoom,
                hideCharacter = true
            },
            new DialogueBlock{
                texts = new string[]{
                    "我還非常喜歡看書...書中的故事真的好有趣...\n你看...整面墻的書都是爸爸媽媽買給我的www...\n我常常在小毯子上面看書呢！\n我們來一起看有趣的書吧~！",
                },
                characterSprites = new Sprite[]{ talk003, talk001 },
                backgroundSprite = bgRoom,
                hideCharacter = false
            },
            new DialogueBlock{
                texts = new string[]{
                    "今天的書特別有趣呢..\n平時都沒有人陪我一起看...\n如果爸爸媽媽也在就好了",
                },
                characterSprites = new Sprite[]{ talk003, talk001 },
                backgroundSprite = bgRoom,
                hideCharacter = false
            },
            new DialogueBlock{
                texts = new string[]{
                    "咕嚕嚕...\n咕嚕嚕...咕嚕嚕...\n好像是糯米的肚子在叫...",
                    "",
                },
                characterSprites = new Sprite[]{ talk006 },
                backgroundSprite = bgRoom,
                hideCharacter = false
            },
            new DialogueBlock{
                texts = new string[]{
                    "今天保姆姐姐好像不來了呢...\n那今天，我自己來試試看吧...\n應該不會很難吧...應該...",
                },
                characterSprites = new Sprite[]{ talk01, talk00, },
                backgroundSprite = bgRoom,
                hideCharacter = false
            },
            new DialogueBlock{
                texts = new string[]{
                    "lalalalalala…踩上小椅子就可以啦\n嗯...先開火....\n好耶...成功啦\n加點油吧....唔...",
                },
                characterSprites = new Sprite[]{talk003, talk001 },
                backgroundSprite = bgKitchen,
                hideCharacter = false
            },
             new DialogueBlock{
                texts = new string[]{
                    "怎麽這麽重呀...\n呀....啊啊啊...油撒出來了...\n怎麽辦呀...咳咳咳...",
                },
                characterSprites = new Sprite[]{talk003, talk001 },
                backgroundSprite = bgKitchen,
                hideCharacter = false
            },
            new DialogueBlock{
                texts = new string[]{
                    "咳咳咳...\n好黑呀...咳咳咳...\n糯米不能呼吸了...咳咳咳...",
                },
                characterSprites = new Sprite[]{ talk006 },
                backgroundSprite = bgKitchenFire,
                hideCharacter = false
            },
            new DialogueBlock{
                texts = new string[]{
                    "嗚嗚嗚...\n這裏是哪裏呀...\n咦...?...",
                },
                characterSprites = new Sprite[]{ talk002 },
                backgroundSprite = bgLevel00,
                hideCharacter = false
            },
            new DialogueBlock{
                texts = new string[]{
                    "那是我的山藥玩偶嗎...？\n但...看起來好像不太一樣...\n要不要走進看看呢...?",
                },
                characterSprites = new Sprite[]{ talk01, talk00,  },
                backgroundSprite = bgLevel00,
                hideCharacter = false
            }
        };
    }

    void ShowChoiceMenu()
    {
        choiceMenu.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
    }

    void OnYesClicked()
    {
        Debug.Log("玩家選了 YES");
        // 進入下一關
        ChangeSceneManager.Instance.onChangeScene(2); // 換成你的下一關名字
    }

    void OnNoClicked()
    {
        Debug.Log("玩家選了 NO");
        // 進入Bad Ending 或直接結束
        ChangeSceneManager.Instance.onChangeScene(4); // 換成你的結束場景名字
    }
}