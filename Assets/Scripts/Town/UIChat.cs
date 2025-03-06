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
    [SerializeField] private Button btnCloseWhisperInput;  // 닫기 버튼


    // 귓속말 입력 UI 요소
    [SerializeField] private TMP_InputField inputWhisperUser;  // 귓속말 대상 유저 입력 필드
    [SerializeField] private GameObject whisperUserInputUI;    // 귓속말 닉네임 입력 UI
    [SerializeField] private Button btnConfirmWhisper;          // 확인 버튼

    private float baseChatItemWidth;
    private Player player;
    private bool isOpen = true;
    private ChatType currentChatType = ChatType.Global;
    private List<(ChatType type, string msg, bool myChat,int id)> chatMessages = new();

    private string selectedWhisperUser;

    public enum ChatType
    {
        Global,  // 전체 채팅
        Whisper, // 귓속말
        Party    // 파티 채팅
    }

    void Start()
    {
        baseChatItemWidth = txtChatItemBase.rectTransform.sizeDelta.x;
        player = TownManager.Instance.MyPlayer;

        btnSend.onClick.AddListener(SendMessage);
        btnToggle.onClick.AddListener(ToggleChatWindow);
        //btnWhisper.onClick.AddListener(OnWhisperButtonClicked);
        //btnParty.onClick.AddListener(() => SetChatType(ChatType.Party));
       // btnGlobalChat.onClick.AddListener(() => SetChatType(ChatType.Global));  // 전체 채팅 버튼 추가
        btnCloseWhisperInput.onClick.AddListener(CloseWhisperUserInputUI);

        inputChat.onSubmit.AddListener((text) =>
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                SendMessage();
            }
        });

        whisperUserInputUI.SetActive(false);  // 시작 시 귓속말 입력 UI 숨김
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
        whisperUserInputUI.SetActive(false);  // 귓속말 대상 입력 UI 숨기기
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

    // 귓속말 버튼 클릭 시 팝업 UI 띄우기
    private void OnWhisperButtonClicked()
    {
        whisperUserInputUI.SetActive(true);  // 귓속말 입력 UI 활성화
        inputWhisperUser.text = "";  // 이전 입력 값 초기화
        inputWhisperUser.ActivateInputField();  // 입력 필드 활성화
    }

    // 귓속말 대상 입력 후 확인 버튼 클릭 시 처리
    private void OnConfirmWhisperClicked()
    {
        string targetUser = inputWhisperUser.text.Trim();  // 입력된 닉네임
        if (string.IsNullOrWhiteSpace(targetUser))
        {
            Debug.LogWarning("닉네임을 입력해주세요.");
            return;
        }

        selectedWhisperUser = targetUser;  // 선택한 귓속말 대상 설정
        currentChatType = ChatType.Whisper;  // 귓속말 모드로 전환
        whisperUserInputUI.SetActive(false);  // 귓속말 대상 입력 UI 숨기기

        // 해당 유저와의 대화만 할 수 있도록 UI 갱신
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
        // 파티 채팅 처리 로직
    }

    private void SendWhisperToUser(string message, string targetUser)
    {
        // 귓속말 처리 로직
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
        // 텍스트 메시지 프리팹을 제대로 할당받았는지 확인
        if (txtChatItemBase == null)
        {
            Debug.LogError("txtChatItemBase is not assigned!");
            return;
        }

        // 메시지 아이템 생성
        var msgItem = Instantiate(txtChatItemBase, chatItemRoot);

        Player playerTemp = TownManager.Instance.GetPlayerAvatarById(id);
        // 채팅 타입에 맞는 색상 지정
        Color messageColor = Color.white;  // 기본은 흰색

        playerTemp.RecvMessage(msg, id);
        // 내 채팅은 주황색으로 표시 (기존 로직)
        msgItem.color = myChat ? Color.yellow : messageColor;
        msgItem.text = playerTemp.GetNickname()+" : " + msg;

        // 활성화
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
            // 귓속말 모드일 경우 해당 유저와의 메시지만 표시
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

