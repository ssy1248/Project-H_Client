using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStart : MonoBehaviour
{
    [SerializeField] private GameObject charList;
    [SerializeField] private Button[] charBtns;
    [SerializeField] private Button localServerBtn;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnBack;
    [SerializeField] private TMP_InputField inputNickname;
    [SerializeField] private TMP_InputField inputPort;
    [SerializeField] private TMP_Text txtMessage;

    //제가 테스트 용도로 추가한 오브젝트들 입니다.
    [SerializeField] TMP_InputField inputEmail;
    [SerializeField] TMP_InputField inputname;
    [SerializeField] TMP_InputField inputpasword;

    [SerializeField] TMP_InputField inputLoginEmail;
    [SerializeField] TMP_InputField inputLogipasword;

    [SerializeField] GameObject conectObject;
    public GameObject loginObject;
    [SerializeField] GameObject registerObject;
    public GameObject chuseObject;


    private TMP_Text placeHolder;

    private int classIdx = 0;
    private string serverUrl;
    private string nickname;
    private string port;

    private const string DefaultServerMessage = "Input Server";
    private const string DefaultNicknameMessage = "닉네임 (2~10글자)";
    private const string WelcomeMessage = "Welcome!";
    private const string ShortNicknameError = "이름을 2글자 이상 입력해주세요!";
    private const string LongNicknameError = "이름을 10글자 이하로 입력해주세요!";

    void Start()
    {
        placeHolder = inputNickname.placeholder.GetComponent<TMP_Text>();
        btnBack.onClick.AddListener(SetServerUI);
        localServerBtn.onClick.AddListener(OnClickLocalServer);
        SetServerUI();
        InitializeCharacterButtons();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (inputNickname.IsActive())
                btnConfirm.onClick.Invoke();
        }
    }

    private void InitializeCharacterButtons()
    {
        for (int i = 0; i < charBtns.Length; i++)
        {
            int idx = i;
            charBtns[i].onClick.AddListener(() => SelectCharacter(idx));
        }
    }

    private void SelectCharacter(int idx)
    {
        charBtns[classIdx].transform.GetChild(0).gameObject.SetActive(false);
        classIdx = idx;
        charBtns[classIdx].transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetServerUI()
    {
        UpdateUI(WelcomeMessage, Color.white, DefaultServerMessage, false, true);
        btnConfirm.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(ConfirmServer);
    }
    // 여기서 수정할 함수들 작성 예정
    public void SetNicknameUI()
    {
        charList.SetActive(false);
        if (registerObject.activeSelf)
        {
            registerObject.SetActive(false);
        }
        conectObject.SetActive(false);
        loginObject.SetActive(true);
        //UpdateUI(WelcomeMessage, Color.white, DefaultNicknameMessage, true, false);
        //btnConfirm.onClick.RemoveAllListeners();
        //btnConfirm.onClick.AddListener(ConfirmNickname);
        ConfirmNickname();
    }

    public void Login()
    {
        Debug.Log(inputLoginEmail.text + inputLogipasword.text);

        TownManager.Instance.Login(inputLoginEmail.text, inputLogipasword.text);
    }
    public void Register()
    {
        Debug.Log(inputname.text);

        TownManager.Instance.Register(inputEmail.text, inputname.text, inputpasword.text);
    }
    public void SetLogin()
    {
        if (registerObject.activeSelf)
        {
            registerObject.SetActive(false);
        }
        loginObject.SetActive(true);
    }

    public void SetRegister()
    {
        if (loginObject.activeSelf)
        {
            loginObject.SetActive(false);
        }
        registerObject.SetActive(true);
    }

    private void UpdateUI(string message, Color messageColor, string placeholderText, bool showCharList, bool showPortInput)
    {
        txtMessage.text = message;
        txtMessage.color = messageColor;

        placeHolder.text = placeholderText;
        inputNickname.text = string.Empty;

        charList.SetActive(showCharList);
        btnBack.gameObject.SetActive(showCharList);
        localServerBtn.gameObject.SetActive(showPortInput);
        inputPort.gameObject.SetActive(showPortInput);
    }

    private void OnClickLocalServer()
    {
        serverUrl = "127.0.0.1";
        port = "3000";
        localServerBtn.gameObject.SetActive(false);
        SetNicknameUI();
    }

    private void ConfirmServer()
    {
        if (string.IsNullOrWhiteSpace(inputNickname.text))
        {
            DisplayError(DefaultServerMessage);
            return;
        }

        serverUrl = inputNickname.text;
        port = inputPort.text;
        SetNicknameUI();
    }

    private void ConfirmNickname()
    {
        /*
        if (inputNickname.text.Length < 2)
        {
            DisplayError(ShortNicknameError);
            return;
        }

        if (inputNickname.text.Length > 10)
        {
            DisplayError(LongNicknameError);
            return;
        }*/

        nickname = inputNickname.text;
        TownManager.Instance.GameStart(serverUrl, port, nickname, classIdx);
        //gameObject.SetActive(false);
    }

    private void DisplayError(string errorMessage)
    {
        txtMessage.text = errorMessage;
        txtMessage.color = Color.red;
    }
}