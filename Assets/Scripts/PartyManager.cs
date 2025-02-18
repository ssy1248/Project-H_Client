using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Transform partySlotParent;  // 파티 슬롯들이 들어갈 부모 Transform
    public GameObject partySlotPrefab;  // 파티 슬롯 프리팹
    public Button nextPageButton;  // 다음 페이지 버튼
    public Button prevPageButton;  // 이전 페이지 버튼
    public TMP_Text pageNumberText; //페이지 번호

    [Header("Party List")]
    public List<Party> partyList = new List<Party>();  // 파티 리스트

    private int currentPage = 1;
    private const int partiesPerPage = 5;  // 페이지 당 파티 수

    void Start()
    {
        // 예제 파티 추가 (실제 게임에서는 DB나 서버에서 가져올 수 있음)
        partyList.Add(new Party("Dungeon A", "Party 1", 4));
        partyList.Add(new Party("Dungeon B", "Party 2", 3));
        partyList.Add(new Party("Dungeon C", "Party 3", 5));
        partyList.Add(new Party("Dungeon D", "Party 4", 2));
        partyList.Add(new Party("Dungeon E", "Party 5", 4));
        partyList.Add(new Party("Dungeon F", "Party 6", 3));
        partyList.Add(new Party("Dungeon G", "Party 7", 1));

        // 버튼 리스너 설정
        nextPageButton.onClick.AddListener(ShowNextPage);
        prevPageButton.onClick.AddListener(ShowPreviousPage);

        // 첫 페이지 로드
        LoadPartiesForPage();
        // 페이지 번호 로드
        UpdatePageNumber();
    }

    private void UpdatePageNumber()
    {
        pageNumberText.text = $"{currentPage}";  // 현재 페이지 번호 표시
    }

    // 파티 페이지 로드
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

    // 페이지 이동
    private void ShowNextPage() { if (currentPage  * partiesPerPage < partyList.Count) { currentPage++; LoadPartiesForPage(); } }
    private void ShowPreviousPage() { if (currentPage > 1) { currentPage--; LoadPartiesForPage(); } }
}
