using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PartyManager : MonoBehaviour
{
    [Header("파티 관련")]
    [SerializeField] private GameObject PartyListPrefab;
    [SerializeField] private GameObject PartyLeaderCharPrefab;
    [SerializeField] private GameObject PartyMemberCharPrefab;

    // 던전에 들어온 파티 정보를 전달할 객체
    public PartyInfo InDungeonPartyInfo;

    // 생성된 UI 오브젝트의 참조를 저장합니다.
    private GameObject partyUI;

    private void Awake()
    {
        // 마을에서 부터 던전으로 이동시킬 오브젝트
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {

    }

    void Update()
    {
        // 이미 UI가 생성되었으면 더 이상 생성하지 않음
        if (partyUI == null)
        {
            DungeonPartyUISetUp();
        }
        else if (partyUI != null && InDungeonPartyInfo.PartyName != partyUI.GetComponentInChildren<TMP_InputField>().text)
        {
            DungeonPartyUISetUp();
        }
    }

    public void DungeonPartyUISetUp()
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Dungeon"))
        {
            // 특정 이름의 Canvas를 찾거나, 이미 Canvas가 Scene에 있는지 확인
            Canvas canvas = GameObject.Find("DungeonCanvas")?.GetComponent<Canvas>();
            if (canvas != null)
            {
                // Canvas의 자식으로 PartyListPrefab을 Instantiate하고 참조를 저장합니다.
                partyUI = Instantiate(PartyListPrefab, canvas.transform);

                // UI 요소이므로 RectTransform을 사용합니다.
                RectTransform rt = partyUI.GetComponent<RectTransform>();
                if (rt != null)
                {
                    // 기존 y, z 값은 그대로 두고 x 좌표를 -800으로 설정합니다.
                    rt.anchoredPosition = new Vector2(-800f, rt.anchoredPosition.y);
                }
                else
                {
                    Debug.LogWarning("PartyListPrefab에 RectTransform 컴포넌트가 없습니다.");
                }

                // 인스턴스화된 객체에서 TMP_InputField를 가져와 텍스트 설정
                TMP_InputField partyNameField = partyUI.GetComponentInChildren<TMP_InputField>();
                if (partyNameField != null)
                {
                    partyNameField.text = InDungeonPartyInfo.PartyName;
                }
                else
                {
                    Debug.LogWarning("인스턴스에서 TMP_InputField를 찾을 수 없습니다.");
                }

                // 이제 파티 정보를 기반으로 파티 캐릭터 UI를 생성합니다.
                InstantiatePartyCharacters();
                InitialDungeonUI();
            }
            else
            {
                Debug.LogWarning("씬 내에 'DungeonCanvas'라는 이름의 Canvas를 찾을 수 없습니다.");
            }
        }
    }

    // PartyInfo 내의 Players 리스트를 순회하여 캐릭터 UI를 생성하는 메서드
    private void InstantiatePartyCharacters()
    {
        if (InDungeonPartyInfo == null || InDungeonPartyInfo.Players == null)
        {
            Debug.LogWarning("파티 정보 또는 플레이어 리스트가 없습니다.");
            return;
        }

        foreach (var playerStatus in InDungeonPartyInfo.Players)
        {
            // partyLeaderId와 playerStatus.playerId 비교하여 리더인지 확인
            GameObject prefabToInstantiate = (playerStatus.PlayerId == InDungeonPartyInfo.PartyLeaderId)
                ? PartyLeaderCharPrefab
                : PartyMemberCharPrefab;

            // partyUI의 자식으로 캐릭터 UI Prefab을 Instantiate합니다.
            GameObject partyChar = Instantiate(prefabToInstantiate, partyUI.transform);

            // 필요에 따라 위치나 크기 조정을 추가할 수 있습니다.

            // 인스턴스화된 객체의 자식에서 TextMeshProUGUI 컴포넌트를 찾아 playerName을 설정합니다.
            TextMeshProUGUI nameText = partyChar.GetComponentInChildren<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = playerStatus.PlayerName;
            }
            else
            {
                Debug.LogWarning("인스턴스에서 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }

    void InitialDungeonUI()
    {
        for(int i = 0; i < InDungeonPartyInfo.Players.Count; i++)
        {
            if (DungeonManager.Instance.MyPlayer.PlayerId == InDungeonPartyInfo.Players[i].PlayerId)
            {
                DungeonUIManager.Instance.PlayerLevelText.text = new string("Lv. ") + InDungeonPartyInfo.Players[i].PlayerLevel;
                DungeonUIManager.Instance.PlayerHpText.text = InDungeonPartyInfo.Players[i].PlayerCurHp + " / " + InDungeonPartyInfo.Players[i].PlayerFullHp;
                DungeonUIManager.Instance.PlayerMpText.text = InDungeonPartyInfo.Players[i].PlayerCurMp + " / " + InDungeonPartyInfo.Players[i].PlayerFullMp;
                DungeonUIManager.Instance.PlayerIcon[InDungeonPartyInfo.Players[i].PlayerClass - 1].SetActive(true);
                DungeonUIManager.Instance.PlayerSkillIcon[InDungeonPartyInfo.Players[i].PlayerClass - 1].SetActive(true);
                DungeonUIManager.Instance.PlayerDodgeIcon[InDungeonPartyInfo.Players[i].PlayerClass - 1].SetActive(true);
                break;
            }
        }
    }
}
