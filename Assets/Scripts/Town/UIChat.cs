using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{
    [SerializeField] private Scrollbar scroll;
    [SerializeField] private RectTransform rectBg;
    [SerializeField] private Transform chatItemRoot;
    [SerializeField] private TMP_Text txtChatItemBase;
    [SerializeField] private TMP_InputField inputChat;
    [SerializeField] private Button btnSend;
    [SerializeField] private Button btnToggle;
    [SerializeField] private Button btnWhisper;
    [SerializeField] private Button btnParty;
    [SerializeField] private Transform icon;
    [SerializeField] private Transform alarm;

    private float baseChatItemWidth;
    private Player player;
    private bool isOpen = true;
    private ChatType currentChatType = ChatType.Global;
    private List<(ChatType type, string msg, bool myChat)> chatMessages = new();

    public enum ChatType
    {
        Global,
        Whisper,
        Party
    }

    void Start()
    {
        baseChatItemWidth = txtChatItemBase.rectTransform.sizeDelta.x;
        //player = TownManager.Instance.MyPlayer;

        btnSend.onClick.AddListener(SendMessage);
        btnToggle.onClick.AddListener(ToggleChatWindow);
        btnWhisper.onClick.AddListener(() => SetChatType(ChatType.Whisper));
        btnParty.onClick.AddListener(() => SetChatType(ChatType.Party));

        inputChat.onSubmit.AddListener((text) =>
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                SendMessage();
            }
        });
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (inputChat.IsActive() && inputChat.isFocused)
            {
                SendMessage();
            }
            else
            {
                ActivateInputFieldProperly();
            }
        }
    }

    private void ToggleChatWindow()
    {
        if (isOpen)
        {
            rectBg.DOSizeDelta(new Vector2(100, 40), 0.3f);
            icon.DORotate(new Vector3(0, 0, 180), 0.3f);
            icon.DOMove(new Vector3(14, 6, 0), 0.3f);
        }
        else
        {
            alarm.gameObject.SetActive(false);
            rectBg.DOSizeDelta(new Vector2(550, 500), 0.3f);
            icon.DORotate(new Vector3(0, 0, 0), 0.3f);
            icon.DOMove(new Vector3(142, 138, 0), 0.3f);

        }

        isOpen = !isOpen;
    }

    private void SetChatType(ChatType type)
    {
        currentChatType = type;
        RefreshChatWindow();
    }

    public void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(inputChat.text)) return;

        // 파티 채팅일 경우
        if (currentChatType == ChatType.Party)
        {
            SendMessageToParty(inputChat.text);
        }
        // 귓속말일 경우
        else if (currentChatType == ChatType.Whisper)
        {
            SendWhisperToUser(inputChat.text, selectedWhisperUser);
        }
        else // 일반 채팅
        {
            player.SendMessage(inputChat.text);
        }

        PushMessage(inputChat.text, true, currentChatType);
        inputChat.text = string.Empty;
        ActivateInputFieldProperly();
    }

    private void SendMessageToParty(string message)
    {
        // 파티 채팅 처리 로직
        // 예: TownManager.Instance.SendMessageToParty(message);
    }

    private void SendWhisperToUser(string message, string targetUser)
    {
        // 귓속말 처리 로직
        // 예: TownManager.Instance.SendWhisper(message, targetUser);
    }

    private void ActivateInputFieldProperly()
    {
        inputChat.ActivateInputField();
        inputChat.caretPosition = 0;
        ResetIME();
    }

    public void PushMessage(string msg, bool myChat, ChatType type)
    {
        chatMessages.Add((type, msg, myChat));
        
        if (type == currentChatType || type == ChatType.Global)
        {
            DisplayMessage(msg, myChat);
        }
    }

    private void DisplayMessage(string msg, bool myChat)
    {
        var msgItem = Instantiate(txtChatItemBase, chatItemRoot);
        msgItem.color = myChat ? Color.green : Color.white;
        msgItem.text = msg;
        msgItem.gameObject.SetActive(true);

        StartCoroutine(AdjustTextSize(msgItem));
        StartCoroutine(ScrollToBottom());
    }

    private void RefreshChatWindow()
    {
        foreach (Transform child in chatItemRoot)
        {
            Destroy(child.gameObject);
        }

        foreach (var (type, msg, myChat) in chatMessages)
        {
            if (type == currentChatType || type == ChatType.Global)
            {
                DisplayMessage(msg, myChat);
            }
        }
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        scroll.value = 0;
    }

    private IEnumerator AdjustTextSize(TMP_Text textComp)
    {
        yield return new WaitForEndOfFrame();

        if (textComp.textInfo.lineCount > 1)
        {
            textComp.rectTransform.sizeDelta = new Vector2(baseChatItemWidth, textComp.preferredHeight + 12);
        }
    }

    private void ResetIME()
    {
        Input.imeCompositionMode = IMECompositionMode.Off;
        Input.imeCompositionMode = IMECompositionMode.On;
    }
}
