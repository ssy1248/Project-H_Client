using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Transform partySlotParent;  // ��Ƽ ���Ե��� �� �θ� Transform
    public GameObject partySlotPrefab;  // ��Ƽ ���� ������
    public Button nextPageButton;  // ���� ������ ��ư
    public Button prevPageButton;  // ���� ������ ��ư
    public TMP_Text pageNumberText; //������ ��ȣ

    [Header("Party List")]
    public List<Party> partyList = new List<Party>();  // ��Ƽ ����Ʈ

    private int currentPage = 1;
    private const int partiesPerPage = 5;  // ������ �� ��Ƽ ��

    void Start()
    {
        // ���� ��Ƽ �߰� (���� ���ӿ����� DB�� �������� ������ �� ����)
        partyList.Add(new Party("Dungeon A", "Party 1", 4));
        partyList.Add(new Party("Dungeon B", "Party 2", 3));
        partyList.Add(new Party("Dungeon C", "Party 3", 5));
        partyList.Add(new Party("Dungeon D", "Party 4", 2));
        partyList.Add(new Party("Dungeon E", "Party 5", 4));
        partyList.Add(new Party("Dungeon F", "Party 6", 3));
        partyList.Add(new Party("Dungeon G", "Party 7", 1));

        // ��ư ������ ����
        nextPageButton.onClick.AddListener(ShowNextPage);
        prevPageButton.onClick.AddListener(ShowPreviousPage);

        // ù ������ �ε�
        LoadPartiesForPage();
        // ������ ��ȣ �ε�
        UpdatePageNumber();
    }

    private void UpdatePageNumber()
    {
        pageNumberText.text = $"{currentPage}";  // ���� ������ ��ȣ ǥ��
    }

    // ��Ƽ ������ �ε�
    private void LoadPartiesForPage()
    {
        foreach (Transform child in partySlotParent)
        {
            Destroy(child.gameObject);
        }

        int startIdx = ( currentPage - 1 ) * partiesPerPage;
        int endIdx = Mathf.Min(startIdx + partiesPerPage, partyList.Count);

        for (int i = startIdx; i < endIdx; i++)
        {
            GameObject slot = Instantiate(partySlotPrefab, partySlotParent);
            Party party = partyList[i];

            PartySlot partySlot = slot.GetComponent<PartySlot>();
            partySlot.SetParty(party);
        }
    }

    // ������ �̵�
    private void ShowNextPage() { if (currentPage  * partiesPerPage < partyList.Count) { currentPage++; LoadPartiesForPage(); } }
    private void ShowPreviousPage() { if (currentPage > 1) { currentPage--; LoadPartiesForPage(); } }
}
