using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject characterPanel;

    [SerializeField] private TMP_InputField inputEmail;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_InputField inputPasswordConfirm;
    [SerializeField] private Button btnLogin;
    [SerializeField] private Button btnRegister;

    [SerializeField] private TMP_InputField inputNickname;
    [SerializeField] private Button[] charBtns;
    [SerializeField] private TMP_Text txtCharDescription;
    [SerializeField] private Button btnConfirmCharacter;
    [SerializeField] private TMP_Text txtMessage;

    private int selectedCharacterIndex = 0;
    private string nickname;

    private readonly string[] characterDescriptions =
    {
        "����ź : �밨�� �����Դϴ�.",
        "Ŭ��ź : �ź�ο� �������Դϴ�.",
        "��ź : ������ �ü��Դϴ�.",
        "ť��ź : ������ ��Ŀ�Դϴ�.",
        "�⸣ź : ġ���� �������Դϴ�."
    };

    private const string PasswordMismatchError = "��й�ȣ�� ��ġ���� �ʽ��ϴ�.";
    private const string ShortNicknameError = "�г����� 2���� �̻� �Է����ּ���!";
    private const string LongNicknameError = "�г����� 10���� ���Ϸ� �Է����ּ���!";

    void Start()
    {
        InitializeCharacterButtons();
        btnLogin.onClick.AddListener(ShowCharacterSelection);
        btnRegister.onClick.AddListener(HandleRegistration);
        SetLoginUI();
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
        // ���� ĳ���� �ܰ��� ��Ȱ��ȭ
        charBtns[selectedCharacterIndex].transform.GetChild(0).gameObject.SetActive(false);
        // �� ĳ���� �ܰ��� Ȱ��ȭ
        selectedCharacterIndex = idx;
        charBtns[selectedCharacterIndex].transform.GetChild(0).gameObject.SetActive(true);

        // ĳ���� ���� ������Ʈ
        txtCharDescription.text = characterDescriptions[selectedCharacterIndex];
    }

    private void SetLoginUI()
    {
        loginPanel.SetActive(true);
        characterPanel.SetActive(false);
        txtMessage.text = string.Empty;
    }

    private void ShowCharacterSelection()
    {
        if (string.IsNullOrWhiteSpace(inputEmail.text) || string.IsNullOrWhiteSpace(inputPassword.text))
        {
            txtMessage.text = "�̸��ϰ� ��й�ȣ�� �Է��ϼ���!";
            txtMessage.color = Color.red;
            return;
        }

        loginPanel.SetActive(false);
        characterPanel.SetActive(true);
    }

    private void HandleRegistration()
    {
        if (inputPassword.text != inputPasswordConfirm.text)
        {
            txtMessage.text = PasswordMismatchError;
            txtMessage.color = Color.red;
            return;
        }

        txtMessage.text = "ȸ������ ����! �α������ּ���.";
        txtMessage.color = Color.green;
    }

    public void ConfirmCharacterSelection()
    {
        if (inputNickname.text.Length < 2)
        {
            DisplayError(ShortNicknameError);
            return;
        }

        if (inputNickname.text.Length > 10)
        {
            DisplayError(LongNicknameError);
            return;
        }

        nickname = inputNickname.text;
        TownManager.Instance.GameStart("defaultServerUrl", "3000", nickname, selectedCharacterIndex);
        gameObject.SetActive(false);
    }

    private void DisplayError(string errorMessage)
    {
        txtMessage.text = errorMessage;
        txtMessage.color = Color.red;
    }
}
