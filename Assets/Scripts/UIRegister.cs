using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour
{
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

    [SerializeField] private Button btnConfirmCharacter;
    [SerializeField] private Button[] charBtns;
    [SerializeField] private TMP_Text txtCharDescription;
    [SerializeField] private TMP_Text txtMessage;

    private int selectedCharacterIndex = 0;

    private readonly string[] characterDescriptions =
    {
        "����ź\n�밨�� �����Դϴ�.",
        "Ŭ��ź\n�ź�ο� �������Դϴ�.",
        "��ź\n������ �ü��Դϴ�.",
        "ť��ź\n������ ��Ŀ�Դϴ�.",
        "�⸣ź\nġ���� �������Դϴ�."
    };

    private const string EmailPasswordError = "�̸��ϰ� ��й�ȣ�� �Է��ϼ���!";
    private const string NicknameError = "�г����� �Է��ϼ���! (2~10��)";

    void Start()
    {
        InitializeCharacterButtons();
        btnRegister.onClick.AddListener(HandleRegistration);
        btnLogin.onClick.AddListener(HandleLogin);
        btnConfirmCharacter.onClick.AddListener(StartGame);

        SetRegisterUI();
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
        charBtns[selectedCharacterIndex].transform.GetChild(0).gameObject.SetActive(false);
        selectedCharacterIndex = idx;
        charBtns[selectedCharacterIndex].transform.GetChild(0).gameObject.SetActive(true);

        txtCharDescription.text = characterDescriptions[selectedCharacterIndex];
    }

    private void SetRegisterUI()
    {
        registerPanel.SetActive(true);
        loginPanel.SetActive(false);
        characterPanel.SetActive(false);
        txtMessage.text = string.Empty;
    }

    private void ShowLoginUI()
    {
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
        txtMessage.text = string.Empty;
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

        Invoke(nameof(ShowLoginUI), 1.5f);
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
    }

    private void StartGame()
    {
        // ���� URL, ��Ʈ, �г���, ĳ���� �ε����� GameStart�� ����
        string serverUrl = "127.0.0.1"; // ����: ���� URL �Է� �ʵ�
        string port = "5555";  // ����: ��Ʈ �Է� �ʵ�

        // ĳ���� ���ð� �г����� �̹� �Է��� �����ٰ� ����
        string nickname = inputNickname.text;
        int selectedCharacterIndex = this.selectedCharacterIndex;

        TownManager.Instance.GameStart(serverUrl, port, nickname , selectedCharacterIndex);
        gameObject.SetActive(false);
    }
}
