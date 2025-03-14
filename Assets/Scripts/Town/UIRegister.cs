using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour
{
    public static UIRegister Instance { get; private set; }  // 싱글톤 패턴으로 UIRegister 접근

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

    [SerializeField] GameObject conectObject;
    public GameObject loginObject;
    [SerializeField] GameObject registerObject;
    public GameObject chuseObject;

    private int selectedCharacterId = -1;
    private string serverUrl;
    private string nickname;
    private string port;

    private readonly string[] characterDescriptions =
    {
        "피오르\n긴 창을 이용해 공격하며, 방어와 반격에 특화된 전사.",
        "녹스\n빠른 기동력을 활용해 적을 교란하며, 적에게 출혈을 일으키는 도적",
        "실피아\n활을 이용해 먼 거리에서 강력하고 빠른 공격을 퍼붓는 궁수",
        "이름구함\n강력한 탱커입니다.",
        "이름구함\n치유의 마법사입니다."
    };

    private const string DefaultServerMessage = "Input Server";
    private const string EmailPasswordError = "이메일과 비밀번호를 입력하세요!";
    private const string NicknameError = "닉네임을 입력하세요! (2~10자)";

    void Start()
    {
        Instance = this;  // UIRegister 인스턴스를 싱글톤으로 설정

        InitializeCharacterButtons();
        btnRegister.onClick.AddListener(HandleRegistration);
        btnLogin.onClick.AddListener(HandleLogin);
        localServerBtn.onClick.AddListener(OnClickLocalServer);
        btnBack.onClick.AddListener(ToggleScreens);

        btnConfirmCharacter.onClick.AddListener(checkchar);
        btnGoToRegister.onClick.AddListener(GoToRegisterScreen);
        SetServerUI(); // 첫 화면인 서버 입력 UI 표시

        TownSEManager.instance.LoopPlaySE("UIRegisterMusic");
    }

    private void ToggleScreens()
    {

        if (loginPanel.activeSelf)
        {
            loginPanel.SetActive(false);
            //serverPanel.SetActive(true);
            localServerBtn.gameObject.SetActive(true); // 로컬 서버 버튼 활성화
        }
        else if (registerPanel.activeSelf)
        {
            registerPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        else if (characterPanel.activeSelf)
        {
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
            int idx = i + 1;
            charBtns[i].onClick.AddListener(() => SelectCharacter(idx));
        }
    }

    private void SelectCharacter(int idx)
    {
        // 이전 선택된 캐릭터 표시 해제
        if (selectedCharacterId != -1)
            charBtns[selectedCharacterId - 1].transform.GetChild(0).gameObject.SetActive(false);

        selectedCharacterId = idx;
        charBtns[selectedCharacterId - 1].transform.GetChild(0).gameObject.SetActive(true); // 선택된 캐릭터 표시

        txtCharDescription.text = characterDescriptions[selectedCharacterId - 1];
        if (idx >= 0 && idx <= 5)
        {
            Debug.Log($"Character selected with id: {selectedCharacterId}");  // 선택된 캐릭터의 id 로그 출력
        }

    }
    private void checkchar()
    {
        TownManager.Instance.SelectCharacterRequest(selectedCharacterId);  // 선택된 캐릭터의 id로 게임 시작 요청
    }

    public void ConfirmCharacter()
    {
        TownSEManager.instance.StopSE("UIRegisterMusic");
        TownSEManager.instance.LoopPlaySE("TownMusic");
    }

    // 캐릭터 선택을 classIdx에 반영
    public void SetSelectedCharacter(int id)
    {
        selectedCharacterId = id;
        txtCharDescription.text = characterDescriptions[selectedCharacterId - 1];
    }

    private void SetServerUI()
    {
        //serverPanel.SetActive(true);
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        characterPanel.SetActive(false);
        btnBack.gameObject.SetActive(false);
    }

    private void ShowLoginUI()
    {
        loginPanel.SetActive(true);
        //serverPanel.SetActive(false);
        registerPanel.SetActive(false);
        characterPanel.SetActive(false);
        txtMessage.text = string.Empty;
        btnBack.gameObject.SetActive(true);
        StartGame();
    }

    private void ShowRegisterUI()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        characterPanel.SetActive(false);
        txtMessage.text = string.Empty;
        btnBack.gameObject.SetActive(true);
    }

    public void RegitserEndLogInUI()
    {
        loginPanel.SetActive(true);
        //serverPanel.SetActive(false);
        registerPanel.SetActive(false);
        characterPanel.SetActive(false);
        txtMessage.text = string.Empty;
        btnBack.gameObject.SetActive(true);
    }

    public void Login()
    {
        TownManager.Instance.Login(loginEmail.text, loginPassword.text);
    }

    public void Registers()
    {
        TownManager.Instance.Register(registerEmail.text, inputNickname.text, registerPassword.text);
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

        Invoke(nameof(Registers), 0.5f);
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

        Invoke(nameof(Login), 0.5f);
        
    }

    public void ShowCharacterSelection()
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
        //serverPanel.SetActive(false); // 서버 입력 화면 숨김
        ShowLoginUI();
    }

    private void StartGame()
    {
        nickname = inputNickname.text;
        TownManager.Instance.GameStart(serverUrl, port, nickname, selectedCharacterId);
    }

    private void DisplayError(string errorMessage)
    {
        txtMessage.text = errorMessage;
        txtMessage.color = Color.red;
    }


    public void ShowRegisterPanel()
    {
        ShowRegisterUI();
    }
}