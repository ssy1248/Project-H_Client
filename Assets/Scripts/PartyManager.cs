using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance; // 싱글톤

    [Header("UI Elements")]
    public GameObject partyMemberPrefab; // 파티원 UI 프리팹
    public Transform partyUIParent;      // 파티 UI가 표시될 부모 오브젝트
    public Text partyNameText;           // 파티 이름 표시 UI
    public InputField partyNameInput;    // 파티 이름 입력창
    public InputField memberNameInput;   // 파티원 추가 입력창
    public Button createPartyButton;     // 파티 생성 버튼
    public Button addMemberButton;       // 파티원 추가 버튼

    private List<PartyMemberUI> partyUIList = new List<PartyMemberUI>();
    private List<string> partyMembers = new List<string>(); // 파티원 닉네임 리스트
    private string partyName = "Default Party"; // 기본 파티 이름
    private bool isPartyCreated = false; // 파티 생성 여부 체크
    private string currentPlayerName = "User"; // 현재 플레이어 닉네임 (유저)

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        // 버튼 클릭 이벤트 연결
        createPartyButton.onClick.AddListener(CreateParty);
        addMemberButton.onClick.AddListener(AddMember);
    }

    // 🔹 파티 생성 (유저가 파티장이 됨)
    public void CreateParty()
    {
        if (isPartyCreated)
        {
            Debug.Log("이미 파티가 존재합니다!");
            return;
        }

        partyName = partyNameInput.text;
        if (string.IsNullOrEmpty(partyName))
        {
            Debug.Log("파티 이름을 입력하세요!");
            return;
        }

        partyMembers.Clear();
        partyMembers.Add(currentPlayerName); // 현재 플레이어를 파티장으로 설정
        isPartyCreated = true;

        UpdatePartyUI();
    }

    // 🔹 파티원 추가
    public void AddMember()
    {
        if (!isPartyCreated)
        {
            Debug.Log("먼저 파티를 생성하세요!");
            return;
        }

        string memberName = memberNameInput.text;
        if (string.IsNullOrEmpty(memberName))
        {
            Debug.Log("추가할 닉네임을 입력하세요!");
            return;
        }

        if (partyMembers.Count >= 4)
        {
            Debug.Log("파티는 최대 4명까지만 가능합니다.");
            return;
        }

        if (partyMembers.Contains(memberName))
        {
            Debug.Log("이미 존재하는 닉네임입니다!");
            return;
        }

        partyMembers.Add(memberName);
        UpdatePartyUI();
    }

    // 🔹 특정 파티원 제거 (파티장이 직접 선택해서 추방)
    public void RemoveMember(string memberName)
    {
        if (partyMembers.Count <= 1)
        {
            Debug.Log("파티장은 추방할 수 없습니다!");
            return;
        }

        if (partyMembers.Contains(memberName))
        {
            partyMembers.Remove(memberName);
            UpdatePartyUI();
        }
    }

    // 🔹 UI 업데이트 (파티원 목록을 갱신)
    private void UpdatePartyUI()
    {
        // 파티 이름 UI 업데이트
        partyNameText.text = $"{partyName}";

        // 기존 UI 삭제
        foreach (var ui in partyUIList)
        {
            Destroy(ui.gameObject);
        }
        partyUIList.Clear();

        // 새로운 UI 생성
        for (int i = 0; i < partyMembers.Count; i++)
        {
            GameObject uiObj = Instantiate(partyMemberPrefab, partyUIParent);
            PartyMemberUI ui = uiObj.GetComponent<PartyMemberUI>();

            bool isLeader = (i == 0); // 첫 번째 멤버가 리더
            ui.SetLeader(isLeader);
            ui.SetName(partyMembers[i]);

            // 파티원 개별 제거 버튼 추가
            if (!isLeader)
            {
                ui.SetRemoveButton(() => RemoveMember(partyMembers[i]));
            }

            partyUIList.Add(ui);
        }
    }
}


public class PartyMemberUI : MonoBehaviour
{
    public GameObject leaderHPBar;  // 왕관이 있는 HP 바
    public GameObject memberHPBar;  // 하얀 사람 아이콘이 있는 HP 바
    public TMP_Text nameText;          // HP 바 내부 닉네임 텍스트
    public Button removeButton;     // 개별 파티원 제거 버튼

    public void SetLeader(bool isLeader)
    {
        leaderHPBar.SetActive(isLeader);
        memberHPBar.SetActive(!isLeader);
        removeButton.gameObject.SetActive(!isLeader); // 리더는 제거 버튼 비활성화
    }

    public void SetName(string playerName)
    {
        nameText.text = playerName;
    }

    public void SetRemoveButton(UnityEngine.Events.UnityAction removeAction)
    {
        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(removeAction);
    }
}
