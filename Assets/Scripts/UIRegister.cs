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
        "섭르탄\n용감한 전사입니다.",
        "클르탄\n신비로운 마법사입니다.",
        "디르탄\n날렵한 궁수입니다.",
        "큐르탄\n강력한 탱커입니다.",
        "기르탄\n치유의 성직자입니다."
    };

    private const string EmailPasswordError = "이메일과 비밀번호를 입력하세요!";
    private const string NicknameError = "닉네임을 입력하세요! (2~10자)";

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

        txtMessage.text = "회원가입 성공! 로그인하세요.";
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

        txtMessage.text = "로그인 성공!";
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
        // 서버 URL, 포트, 닉네임, 캐릭터 인덱스를 GameStart에 전달
        string serverUrl = "127.0.0.1"; // 예시: 서버 URL 입력 필드
        string port = "5555";  // 예시: 포트 입력 필드

        // 캐릭터 선택과 닉네임은 이미 입력이 끝났다고 가정
        string nickname = inputNickname.text;
        int selectedCharacterIndex = this.selectedCharacterIndex;

        TownManager.Instance.GameStart(serverUrl, port, nickname , selectedCharacterIndex);
        gameObject.SetActive(false);
    }
}
