using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour
{
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

    private int classIdx = 0;
    private string serverUrl;
    private string nickname;
    private string port;

    private readonly string[] characterDescriptions =
    {
        "����ź\n�밨�� �����Դϴ�.",
        "Ŭ��ź\n�ź�ο� �������Դϴ�.",
        "��ź\n������ �ü��Դϴ�.",
        "ť��ź\n������ ��Ŀ�Դϴ�.",
        "�⸣ź\nġ���� �������Դϴ�."
    };

    private const string DefaultServerMessage = "Input Server";
    private const string EmailPasswordError = "�̸��ϰ� ��й�ȣ�� �Է��ϼ���!";
    private const string NicknameError = "�г����� �Է��ϼ���! (2~10��)";

    void Start()
    {
        InitializeCharacterButtons();
        btnRegister.onClick.AddListener(HandleRegistration);
        btnLogin.onClick.AddListener(HandleLogin);
        btnConfirmCharacter.onClick.AddListener(StartGame);
        localServerBtn.onClick.AddListener(OnClickLocalServer);

        btnBack.onClick.AddListener(ToggleScreens);
        btnGoToRegister.onClick.AddListener(GoToRegisterScreen);
        SetServerUI(); // ù ȭ���� ���� �Է� UI ǥ��
    }

    private void ToggleScreens()
    {
        if (serverPanel.activeSelf)
        {
            // ���� �Է� ȭ�鿡�� �α��� ȭ������
            serverPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        else if (loginPanel.activeSelf)
        {
            // �α��� ȭ�鿡�� ���� �Է� ȭ������
            loginPanel.SetActive(false);
            serverPanel.SetActive(true);
            localServerBtn.gameObject.SetActive(true); // ���� ���� ��ư Ȱ��ȭ
        }
        else if (registerPanel.activeSelf)
        {
            // ȸ������ ȭ�鿡�� �α��� ȭ������
            registerPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        else if (characterPanel.activeSelf)
        {
            //ĳ���� ����â���� �α���ȭ������
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

    // ù ȭ�鿡 ������ ��Ʈ �Է��� ���� UI ǥ��
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

        txtMessage.text = "ȸ������ ����! �α����ϼ���.";
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

        txtMessage.text = "�α��� ����!";
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

    // ȸ������ ȭ������ �̵��ϴ� �޼���
    public void ShowRegisterPanel()
    {
        ShowRegisterUI();
    }
}
