using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputPort;
    [SerializeField] private Button localServerBtn;
    [SerializeField] private Button btnBack; // 이전 화면으로 돌아가는 버튼

    [SerializeField] private GameObject serverPanel; // 첫 화면 서버 입력 패널
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private GameObject characterPanel;

    [Header("Register")]
    [SerializeField] private TMP_InputField registerEmail;
    [SerializeField] private TMP_InputField registerPassword;
    [SerializeField] private TMP_InputField inputNickname;
    [SerializeField] private Button btnRegister;

    [Header("Login")]
    [SerializeField] private TMP_InputField loginEmail;
    [SerializeField] private TMP_InputField loginPassword;
    [SerializeField] private Button btnLogin;
    [SerializeField] private Button btnGoToRegister; // 로그인에서 회원가입 화면으로 가는 버튼

    [SerializeField] private Button btnConfirmCharacter;
    [SerializeField] private Button[] charBtns;
    [SerializeField] private TMP_Text txtCharDescription;
    [SerializeField] private TMP_Text txtMessage;

    private int classIdx = 0;
    private string serverUrl;
    private string nickname;
    private string port;

    private readonly string[] characterDescriptions =
    {
        "섭르탄\n용감한 전사입니다.",
        "클르탄\n신비로운 마법사입니다.",
        "디르탄\n날렵한 궁수입니다.",
        "큐르탄\n강력한 탱커입니다.",
        "기르탄\n치유의 성직자입니다."
    };

    private const string DefaultServerMessage = "Input Server";
    private const string EmailPasswordError = "이메일과 비밀번호를 입력하세요!";
    private const string NicknameError = "닉네임을 입력하세요! (2~10자)";

    void Start()
    {
        InitializeCharacterButtons();
        btnRegister.onClick.AddListener(HandleRegistration);
        btnLogin.onClick.AddListener(HandleLogin);
        btnConfirmCharacter.onClick.AddListener(StartGame);
        localServerBtn.onClick.AddListener(OnClickLocalServer);

        btnBack.onClick.AddListener(ToggleScreens);
        btnGoToRegister.onClick.AddListener(GoToRegisterScreen);
        SetServerUI(); // 첫 화면인 서버 입력 UI 표시
    }

    private void ToggleScreens()
    {
        if (serverPanel.activeSelf)
        {
            // 서버 입력 화면에서 로그인 화면으로
            serverPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        else if (loginPanel.activeSelf)
        {
            // 로그인 화면에서 서버 입력 화면으로
            loginPanel.SetActive(false);
            serverPanel.SetActive(true);
            localServerBtn.gameObject.SetActive(true); // 로컬 서버 버튼 활성화
        }
        else if (registerPanel.activeSelf)
        {
            // 회원가입 화면에서 로그인 화면으로
            registerPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        else if (characterPanel.activeSelf)
        {
            //캐릭터 선택창에서 로그인화면으로
            characterPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
    }

    private void GoToRegisterScreen()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        btnBack.gameObject.SetActive(true); // 회원가입 화면에서 뒤로 가기 버튼 활성화
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

        txtCharDescription.text = characterDescriptions[classIdx];
    }

    // 첫 화면에 서버와 포트 입력을 위한 UI 표시
    private void SetServerUI()
    {
        serverPanel.SetActive(true);
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        characterPanel.SetActive(false);
        btnBack.gameObject.SetActive(false);
    }

    private void ShowLoginUI()
    {
        serverPanel.SetActive(false);
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
        characterPanel.SetActive(false);
        txtMessage.text = string.Empty;
        btnBack.gameObject.SetActive(true);
    }

    private void ShowRegisterUI()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        characterPanel.SetActive(false);
        txtMessage.text = string.Empty;
        btnBack.gameObject.SetActive(true);
    }

    private void HandleRegistration()
    {
        if (string.IsNullOrWhiteSpace(registerEmail.text) || string.IsNullOrWhiteSpace(registerPassword.text))
        {
            txtMessage.text = EmailPasswordError;
            txtMessage.color = Color.red;
            return;
        }

        if (string.IsNullOrWhiteSpace(inputNickname.text) || inputNickname.text.Length < 2 || inputNickname.text.Length > 10)
        {
            txtMessage.text = NicknameError;
            txtMessage.color = Color.red;
            return;
        }

        txtMessage.text = "회원가입 성공! 로그인하세요.";
        txtMessage.color = Color.green;

        Invoke(nameof(ShowLoginUI), 0.5f);
    }

    private void HandleLogin()
    {
        if (string.IsNullOrWhiteSpace(loginEmail.text) || string.IsNullOrWhiteSpace(loginPassword.text))
        {
            txtMessage.text = EmailPasswordError;
            txtMessage.color = Color.red;
            return;
        }

        txtMessage.text = "로그인 성공!";
        txtMessage.color = Color.green;

        Invoke(nameof(ShowCharacterSelection), 0.5f);
    }

    private void ShowCharacterSelection()
    {
        loginPanel.SetActive(false);
        characterPanel.SetActive(true);
        btnBack.gameObject.SetActive(true);
    }

    private void OnClickLocalServer()
    {
        serverUrl = "127.0.0.1";
        port = "3000";
        localServerBtn.gameObject.SetActive(false);
        ShowLoginUI();
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
        ShowLoginUI();
    }

    private void StartGame()
    {
        nickname = inputNickname.text;
        TownManager.Instance.GameStart(serverUrl, port, nickname, classIdx);
        gameObject.SetActive(false);
    }

    private void DisplayError(string errorMessage)
    {
        txtMessage.text = errorMessage;
        txtMessage.color = Color.red;
    }

    // 회원가입 화면으로 이동하는 메서드
    public void ShowRegisterPanel()
    {
        ShowRegisterUI();
    }
}
