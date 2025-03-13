using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PartyManager : MonoBehaviour
{
    [Header("��Ƽ ����")]
    [SerializeField] private GameObject PartyListPrefab;
    [SerializeField] private GameObject PartyLeaderCharPrefab;
    [SerializeField] private GameObject PartyMemberCharPrefab;

    // ������ ���� ��Ƽ ������ ������ ��ü
    public PartyInfo InDungeonPartyInfo;

    // ������ UI ������Ʈ�� ������ �����մϴ�.
    private GameObject partyUI;

    private void Awake()
    {
        // �������� ���� �������� �̵���ų ������Ʈ
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {

    }

    void Update()
    {
        // �̹� UI�� �����Ǿ����� �� �̻� �������� ����
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
            // Ư�� �̸��� Canvas�� ã�ų�, �̹� Canvas�� Scene�� �ִ��� Ȯ��
            Canvas canvas = GameObject.Find("DungeonCanvas")?.GetComponent<Canvas>();
            if (canvas != null)
            {
                // Canvas�� �ڽ����� PartyListPrefab�� Instantiate�ϰ� ������ �����մϴ�.
                partyUI = Instantiate(PartyListPrefab, canvas.transform);

                // UI ����̹Ƿ� RectTransform�� ����մϴ�.
                RectTransform rt = partyUI.GetComponent<RectTransform>();
                if (rt != null)
                {
                    // ���� y, z ���� �״�� �ΰ� x ��ǥ�� -800���� �����մϴ�.
                    rt.anchoredPosition = new Vector2(-800f, rt.anchoredPosition.y);
                }
                else
                {
                    Debug.LogWarning("PartyListPrefab�� RectTransform ������Ʈ�� �����ϴ�.");
                }

                // �ν��Ͻ�ȭ�� ��ü���� TMP_InputField�� ������ �ؽ�Ʈ ����
                TMP_InputField partyNameField = partyUI.GetComponentInChildren<TMP_InputField>();
                if (partyNameField != null)
                {
                    partyNameField.text = InDungeonPartyInfo.PartyName;
                }
                else
                {
                    Debug.LogWarning("�ν��Ͻ����� TMP_InputField�� ã�� �� �����ϴ�.");
                }

                // ���� ��Ƽ ������ ������� ��Ƽ ĳ���� UI�� �����մϴ�.
                InstantiatePartyCharacters();
                InitialDungeonUI();
            }
            else
            {
                Debug.LogWarning("�� ���� 'DungeonCanvas'��� �̸��� Canvas�� ã�� �� �����ϴ�.");
            }
        }
    }

    // PartyInfo ���� Players ����Ʈ�� ��ȸ�Ͽ� ĳ���� UI�� �����ϴ� �޼���
    private void InstantiatePartyCharacters()
    {
        if (InDungeonPartyInfo == null || InDungeonPartyInfo.Players == null)
        {
            Debug.LogWarning("��Ƽ ���� �Ǵ� �÷��̾� ����Ʈ�� �����ϴ�.");
            return;
        }

        foreach (var playerStatus in InDungeonPartyInfo.Players)
        {
            // partyLeaderId�� playerStatus.playerId ���Ͽ� �������� Ȯ��
            GameObject prefabToInstantiate = (playerStatus.PlayerId == InDungeonPartyInfo.PartyLeaderId)
                ? PartyLeaderCharPrefab
                : PartyMemberCharPrefab;

            // partyUI�� �ڽ����� ĳ���� UI Prefab�� Instantiate�մϴ�.
            GameObject partyChar = Instantiate(prefabToInstantiate, partyUI.transform);

            // �ʿ信 ���� ��ġ�� ũ�� ������ �߰��� �� �ֽ��ϴ�.

            // �ν��Ͻ�ȭ�� ��ü�� �ڽĿ��� TextMeshProUGUI ������Ʈ�� ã�� playerName�� �����մϴ�.
            TextMeshProUGUI nameText = partyChar.GetComponentInChildren<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = playerStatus.PlayerName;
            }
            else
            {
                Debug.LogWarning("�ν��Ͻ����� TextMeshProUGUI ������Ʈ�� ã�� �� �����ϴ�.");
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
