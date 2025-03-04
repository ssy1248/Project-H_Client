using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour
{
    public static UIRegister Instance { get; private set; }  // �̱��� �������� UIRegister ����

    [SerializeField] private TMP_InputField inputPort;
    [SerializeField] private Button localServerBtn;
    [SerializeField] private Button btnBack; // ���� ȭ������ ���ư��� ��ư

    [SerializeField] private GameObject serverPanel; // ù ȭ�� ���� �Է� �г�
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
    [SerializeField] private Button btnGoToRegister; // �α��ο��� ȸ������ ȭ������ ���� ��ư

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
        "�̸�����\n�밨�� �����Դϴ�.",
        "�̸�����\n�ź�ο� �������Դϴ�.",
        "�̸�����\n������ �ü��Դϴ�.",
        "�̸�����\n������ ��Ŀ�Դϴ�.",
        "�̸�����\nġ���� �������Դϴ�."
    };

    private const string DefaultServerMessage = "Input Server";
    private const string EmailPasswordError = "�̸��ϰ� ��й�ȣ�� �Է��ϼ���!";
    private const string NicknameError = "�г����� �Է��ϼ���! (2~10��)";

    void Start()
    {
        Instance = this;  // UIRegister �ν��Ͻ��� �̱������� ����

        InitializeCharacterButtons();
        btnRegister.onClick.AddListener(HandleRegistration);
        btnLogin.onClick.AddListener(HandleLogin);
        localServerBtn.onClick.AddListener(OnClickLocalServer);
        btnBack.onClick.AddListener(ToggleScreens);

        btnConfirmCharacter.onClick.AddListener(ConfirmCharacter);

        btnGoToRegister.onClick.AddListener(GoToRegisterScreen);
        SetServerUI(); // ù ȭ���� ���� �Է� UI ǥ��
    }

    private void ToggleScreens()
    {
        if (serverPanel.activeSelf)
        {
            serverPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        else if (loginPanel.activeSelf)
        {
            loginPanel.SetActive(false);
            serverPanel.SetActive(true);
            localServerBtn.gameObject.SetActive(true); // ���� ���� ��ư Ȱ��ȭ
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
        btnBack.gameObject.SetActive(true); // ȸ������ ȭ�鿡�� �ڷ� ���� ��ư Ȱ��ȭ
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
        // ���� ���õ� ĳ���� ǥ�� ����
        if (selectedCharacterId != -1)
            charBtns[selectedCharacterId - 1].transform.GetChild(0).gameObject.SetActive(false);

        selectedCharacterId = idx;
        charBtns[selectedCharacterId - 1].transform.GetChild(0).gameObject.SetActive(true); // ���õ� ĳ���� ǥ��

        txtCharDescription.text = characterDescriptions[selectedCharacterId - 1];
        if (idx >= 0 && idx <= 5)
        {
            //Debug.Log($"Character selected with id: {selectedCharacterId}");  // ���õ� ĳ������ id �α� ���
        }

    }

    private void ConfirmCharacter()
    {
        if (selectedCharacterId >= 0 && selectedCharacterId <= 5)
        {
            TownManager.Instance.SelectCharacterRequest(selectedCharacterId);  // ���õ� ĳ������ id�� ���� ���� ��û
        }
        else
        {
            Debug.LogError("No character selected!");
        }
    }

    // ĳ���� ������ classIdx�� �ݿ�
    public void SetSelectedCharacter(int id)
    {
        selectedCharacterId = id;
        txtCharDescription.text = characterDescriptions[selectedCharacterId - 1];
    }

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
        loginPanel.SetActive(true);
        serverPanel.SetActive(false);
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

        txtMessage.text = "ȸ������ ����! �α����ϼ���.";
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

        txtMessage.text = "�α��� ����!";
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
        serverPanel.SetActive(false); // ���� �Է� ȭ�� ����
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