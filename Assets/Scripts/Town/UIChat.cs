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
    [SerializeField] GameObject animationTypeObject;
    [SerializeField] private Button btnSend;
    [SerializeField] private Button btnToggle;
    [SerializeField] private Button btnWhisper;
    [SerializeField] private Button btnParty;
    [SerializeField] private Button btnGlobalChat;
    [SerializeField] private Transform icon;
    [SerializeField] private Transform alarm;
    [SerializeField] private Button btnCloseWhisperInput;  // �ݱ� ��ư


    // �ӼӸ� �Է� UI ���
    [SerializeField] private TMP_InputField inputWhisperUser;  // �ӼӸ� ��� ���� �Է� �ʵ�
    [SerializeField] private GameObject whisperUserInputUI;    // �ӼӸ� �г��� �Է� UI
    [SerializeField] private Button btnConfirmWhisper;          // Ȯ�� ��ư

    private float baseChatItemWidth;
    private Player player;
    private bool isOpen = true;
    private ChatType currentChatType = ChatType.Global;
    private List<(ChatType type, string msg, bool myChat,int id)> chatMessages = new();

    private string selectedWhisperUser;

    public enum ChatType
    {
        Global,  // ��ü ä��
        Whisper, // �ӼӸ�
        Party    // ��Ƽ ä��
    }

    void Start()
    {
        baseChatItemWidth = txtChatItemBase.rectTransform.sizeDelta.x;
        player = TownManager.Instance.MyPlayer;

        btnSend.onClick.AddListener(SendMessage);
        btnToggle.onClick.AddListener(ToggleChatWindow);
        //btnWhisper.onClick.AddListener(OnWhisperButtonClicked);
        //btnParty.onClick.AddListener(() => SetChatType(ChatType.Party));
       // btnGlobalChat.onClick.AddListener(() => SetChatType(ChatType.Global));  // ��ü ä�� ��ư �߰�
        btnCloseWhisperInput.onClick.AddListener(CloseWhisperUserInputUI);

        inputChat.onSubmit.AddListener((text) =>
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                SendMessage();
            }
        });

        whisperUserInputUI.SetActive(false);  // ���� �� �ӼӸ� �Է� UI ����
        btnConfirmWhisper.onClick.AddListener(OnConfirmWhisperClicked);
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

    private void CloseWhisperUserInputUI()
    {
        whisperUserInputUI.SetActive(false);  // �ӼӸ� ��� �Է� UI �����
    }

    private void ToggleChatWindow()
    {
        if (isOpen)
        {
            rectBg.DOSizeDelta(new Vector2(100, 40), 0.3f);
            icon.DORotate(new Vector3(0, 0, 180), 0.3f);
            icon.DOMove(new Vector3(12, 5, 0), 0.3f);
        }
        else
        {
            alarm.gameObject.SetActive(false);
            rectBg.DOSizeDelta(new Vector2(550, 500), 0.3f);
            icon.DORotate(new Vector3(0, 0, 0), 0.3f);
            icon.DOMove(new Vector3(126, 122, 0), 0.3f);
        }

        isOpen = !isOpen;
    }

    private void SetChatType(ChatType type)
    {
        currentChatType = type;
        RefreshChatWindow();
    }

    // �ӼӸ� ��ư Ŭ�� �� �˾� UI ����
    private void OnWhisperButtonClicked()
    {
        whisperUserInputUI.SetActive(true);  // �ӼӸ� �Է� UI Ȱ��ȭ
        inputWhisperUser.text = "";  // ���� �Է� �� �ʱ�ȭ
        inputWhisperUser.ActivateInputField();  // �Է� �ʵ� Ȱ��ȭ
    }

    // �ӼӸ� ��� �Է� �� Ȯ�� ��ư Ŭ�� �� ó��
    private void OnConfirmWhisperClicked()
    {
        string targetUser = inputWhisperUser.text.Trim();  // �Էµ� �г���
        if (string.IsNullOrWhiteSpace(targetUser))
        {
            Debug.LogWarning("�г����� �Է����ּ���.");
            return;
        }

        selectedWhisperUser = targetUser;  // ������ �ӼӸ� ��� ����
        currentChatType = ChatType.Whisper;  // �ӼӸ� ���� ��ȯ
        whisperUserInputUI.SetActive(false);  // �ӼӸ� ��� �Է� UI �����

        // �ش� �������� ��ȭ�� �� �� �ֵ��� UI ����
        RefreshChatWindow();
    }

    public void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(inputChat.text)) return;

        player.SendMessage(inputChat.text);
        PushMessage(inputChat.text, true, currentChatType, player.PlayerId);
        inputChat.text = string.Empty;
        ActivateInputFieldProperly();
    }
    public void SetAnimationSetting()
    {
        if (!animationTypeObject.activeSelf)
        {
            animationTypeObject.SetActive(true);
        }
        else
        {
            animationTypeObject.SetActive(false);
        }
    }
    private void SendMessageToParty(string message)
    {
        // ��Ƽ ä�� ó�� ����
    }

    private void SendWhisperToUser(string message, string targetUser)
    {
        // �ӼӸ� ó�� ����
    }

    private void ActivateInputFieldProperly()
    {
        inputChat.ActivateInputField();
        inputChat.caretPosition = 0;
        ResetIME();
    }
    
    public void PushMessage(string msg, bool myChat, ChatType type, int id)
    {
        chatMessages.Add((type, msg, myChat,id));

        if (type == currentChatType || type == ChatType.Global)
        {
            DisplayMessage(msg, myChat, id);
        }
    }

    private void DisplayMessage(string msg, bool myChat,int id)
    {
        // �ؽ�Ʈ �޽��� �������� ����� �Ҵ�޾Ҵ��� Ȯ��
        if (txtChatItemBase == null)
        {
            Debug.LogError("txtChatItemBase is not assigned!");
            return;
        }

        // �޽��� ������ ����
        var msgItem = Instantiate(txtChatItemBase, chatItemRoot);

        Player playerTemp = TownManager.Instance.GetPlayerAvatarById(id);
        // ä�� Ÿ�Կ� �´� ���� ����
        Color messageColor = Color.white;  // �⺻�� ���

        playerTemp.RecvMessage(msg, id);
        // �� ä���� ��Ȳ������ ǥ�� (���� ����)
        msgItem.color = myChat ? Color.yellow : messageColor;
        msgItem.text = playerTemp.GetNickname()+" : " + msg;

        // Ȱ��ȭ
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

        foreach (var (type, msg, myChat,id) in chatMessages)
        {
            // �ӼӸ� ����� ��� �ش� �������� �޽����� ǥ��
            if (currentChatType == ChatType.Whisper && !msg.Contains(selectedWhisperUser))
                continue;

            if (type == currentChatType || type == ChatType.Global)
            {
                DisplayMessage(msg, myChat,id);
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

