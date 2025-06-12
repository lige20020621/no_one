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

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip normalMusic;
    public AudioClip fireMusic;
    public AudioClip startMusic;
    public float musicFadeTime = 0.5f;
    public float backgroundMusicVolume = 0.3f;

    [Header("Typing Audio")]
    public AudioClip typingSound;
    public AudioSource typingAudioSource;
    public float typingVolume = 0.2f;
    public bool playTypingSoundOnEveryChar = false; // If false, plays continuously while typing

    [Header("Next Block Audio")]
    public AudioClip nextBlockSound;
    public AudioSource nextBlockAudioSource;
    public float nextBlockVolume = 0.5f;

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
        // Setup audio
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        // Play background music
        if (normalMusic != null)
        {
            musicSource.clip = normalMusic;
            musicSource.loop = true;
            musicSource.volume = 0.3f;
            musicSource.Play();
        }
        SetupAudio();
    }

    void SetupAudio()
    {
        // Setup main music source
        if (musicSource == null)
        {
            GameObject musicGO = new GameObject("MusicSource");
            musicGO.transform.SetParent(transform);
            musicSource = musicGO.AddComponent<AudioSource>();
        }
        musicSource.loop = true;
        musicSource.volume = backgroundMusicVolume;

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

        // Play initial background music
        if (normalMusic != null)
        {
            musicSource.clip = normalMusic;
            musicSource.Play();
        }
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

    void Update()
    {
        if (dialogueBox.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            PlayNextBlockSound();

            if (isTyping)
            {
                StopAllCoroutines();
                StopTyping();
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

            // Check if we need to change music
            HandleMusicChange(block.backgroundSprite);
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

        // Start typing sound
        StartTypingSound();

        foreach (char c in block.texts[currentTextIndex].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Stop typing sound
        StopTyping();
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

    void HandleMusicChange(Sprite newBackground)
    {
        AudioClip targetMusic = null;

        // Determine which music to play based on background
        if (newBackground == bgKitchenFire)
        {
            targetMusic = fireMusic;
        }
        else if(newBackground == bgLevel00)
        {
            targetMusic = startMusic;
        }
        else
        {
            targetMusic = normalMusic;
        }

        // Change music if different from current
        if (targetMusic != null && musicSource.clip != targetMusic)
        {
            StartCoroutine(CrossfadeMusic(targetMusic));
        }
    }

    IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        // Fade out current music
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / musicFadeTime;
            yield return null;
        }

        // Change clip and fade in
        musicSource.clip = newClip;
        musicSource.Play();

        while (musicSource.volume < startVolume)
        {
            musicSource.volume += startVolume * Time.deltaTime / musicFadeTime;
            yield return null;
        }

        musicSource.volume = startVolume;
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
        ChangeSceneManager.Instance.onChangeScene(5,"content","未能鼓起勇氣的糯米，沒能踏出第一步，沒能接近山藥玩偶\n也沒能等到接她回家的那個人，永遠的迷失在，這個奇怪的無限長廊當中...");
    }
}