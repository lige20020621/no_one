using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManagerGoodEnd : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public Text dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.05f;

    // The good ending text - editable in inspector
    [TextArea(5, 10)]
    public string endingText = "當糯米睜開眼睛的時候，看到的是陌生的天花板...\n然後身邊傳來的是爸爸媽媽説話的聲音，急切地關心在糯米耳邊傳來\n好像做了一場怪夢......糯米想著糯米撲向爸爸媽媽的懷抱嘟囔著想要多陪陪自己，不想要一個人\n爸爸媽媽們似乎也意識到了錯誤：\"對不起寶貝...以後再也不會沒有人陪著你啦...\"";
    
    private bool isTyping = false;

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // Start dialogue automatically
        StartCoroutine(ShowDialogue());
    }

    IEnumerator ShowDialogue()
    {
        isTyping = true;

        if (dialogueText != null)
        {
            dialogueText.text = "";

            foreach (char c in endingText)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        isTyping = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Skip typing
                StopAllCoroutines();
                if (dialogueText != null)
                {
                    dialogueText.text = endingText;
                }
                isTyping = false;
            }
            else
            {
                // Close game or go to menu
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
            }
        }
    }
}