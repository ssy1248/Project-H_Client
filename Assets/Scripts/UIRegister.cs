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
        "섭르탄 : 용감한 전사입니다.",
        "클르탄 : 신비로운 마법사입니다.",
        "디르탄 : 날렵한 궁수입니다.",
        "큐르탄 : 강력한 탱커입니다.",
        "기르탄 : 치유의 성직자입니다."
    };

    private const string PasswordMismatchError = "비밀번호가 일치하지 않습니다.";
    private const string ShortNicknameError = "닉네임을 2글자 이상 입력해주세요!";
    private const string LongNicknameError = "닉네임을 10글자 이하로 입력해주세요!";

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
        // 기존 캐릭터 외곽선 비활성화
        charBtns[selectedCharacterIndex].transform.GetChild(0).gameObject.SetActive(false);
        // 새 캐릭터 외곽선 활성화
        selectedCharacterIndex = idx;
        charBtns[selectedCharacterIndex].transform.GetChild(0).gameObject.SetActive(true);

        // 캐릭터 설명 업데이트
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
            txtMessage.text = "이메일과 비밀번호를 입력하세요!";
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

        txtMessage.text = "회원가입 성공! 로그인해주세요.";
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
