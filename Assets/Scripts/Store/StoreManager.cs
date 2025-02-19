using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Protobuf.Protocol;
using System.Xml.Linq;

public class ShopUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Transform buySlotParent;  // 구매 페이지 슬롯들이 들어갈 부모 Transform
    public Transform sellSlotParent;  // 판매 페이지 슬롯들이 들어갈 부모 Transform
    public GameObject buySlotPrefab;  // 구매 슬롯 프리팹
    public GameObject sellSlotPrefab;  // 판매 슬롯 프리팹
    public Button nextBuyPageButton;  // 구매 페이지 다음 페이지 버튼
    public Button prevBuyPageButton;  // 구매 페이지 이전 페이지 버튼
    public Button nextSellPageButton;  // 판매 페이지 다음 페이지 버튼
    public Button prevSellPageButton;  // 판매 페이지 이전 페이지 버튼


    [Header("Item List")]
    public List<ItemInfo> buyItemList = new List<ItemInfo>();  // 구매 가능한 아이템 리스트
    public List<ItemInfo> sellItemList = new List<ItemInfo>();  // 판매 가능한 아이템 리스트

    private int currentBuyPage = 0;
    private int currentSellPage = 0;
    private const int itemsPerPage = 5;  // 페이지 당 아이템 수

    void Start()
    {
        /*
        buyItemList.Add(new ItemInfo("Sword", "A sharp blade.", null, 100));
        sellItemList.Add(new ItemInfo(1, 100, "Old Sword", "A worn-out sword.", null, 30));
        */
        // 버튼 리스너 설정
        nextBuyPageButton.onClick.AddListener(ShowNextBuyPage);
        prevBuyPageButton.onClick.AddListener(ShowPreviousBuyPage);
        nextSellPageButton.onClick.AddListener(ShowNextSellPage);
        prevSellPageButton.onClick.AddListener(ShowPreviousSellPage);

    }
    // 상점 데이터 받아오기 
    public void GetBuyData(List<ItemInfo> data)
    {
        buyItemList = data;
        LoadItemsForSellPage();
    }
    // 인벤토리 데이터 받아오기
    public void GetSellData()
    {

        LoadItemsForBuyPage();
    }
    // 구매 페이지 로드
    private void LoadItemsForBuyPage()
    {
        foreach (Transform child in buySlotParent)
        {
            Destroy(child.gameObject);
        }

        int startIdx = currentBuyPage * itemsPerPage;
        int endIdx = Mathf.Min(startIdx + itemsPerPage, buyItemList.Count);

        for (int i = startIdx; i < endIdx; i++)
        {
            GameObject slot = Instantiate(buySlotPrefab, buySlotParent);
            slot.SetActive(true);  // 생성 후 활성화
            ItemInfo item = buyItemList[i];

            ItemSlotBuy itemSlot = slot.GetComponent<ItemSlotBuy>();
            itemSlot.SetItem(item);
        }

    }

    // 판매 페이지 로드
    private void LoadItemsForSellPage()
    {
        foreach (Transform child in sellSlotParent)
        {
            Destroy(child.gameObject);
        }

        int startIdx = currentSellPage * itemsPerPage;
        int endIdx = Mathf.Min(startIdx + itemsPerPage, sellItemList.Count);

        for (int i = startIdx; i < endIdx; i++)
        {
            GameObject slot = Instantiate(sellSlotPrefab, sellSlotParent);
            ItemInfo item = sellItemList[i];

            ItemSlotSell itemSlot = slot.GetComponent<ItemSlotSell>();
            itemSlot.SetItem(item);
        }
    }

    // 구매 페이지 이동
    private void ShowNextBuyPage() { if ((currentBuyPage + 1) * itemsPerPage < buyItemList.Count) { currentBuyPage++; LoadItemsForBuyPage(); } }
    private void ShowPreviousBuyPage() { if (currentBuyPage > 0) { currentBuyPage--; LoadItemsForBuyPage(); } }

    // 판매 페이지 이동
    private void ShowNextSellPage() { if ((currentSellPage + 1) * itemsPerPage < sellItemList.Count) { currentSellPage++; LoadItemsForSellPage(); } }
    private void ShowPreviousSellPage() { if (currentSellPage > 0) { currentSellPage--; LoadItemsForSellPage(); } }

    public void OpenShop()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }else
            gameObject.SetActive(true);
    }
}
