using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Protobuf.Protocol;
using System.Xml.Linq;

public class ShopUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Transform buySlotParent;  // ���� ������ ���Ե��� �� �θ� Transform
    public Transform sellSlotParent;  // �Ǹ� ������ ���Ե��� �� �θ� Transform
    public GameObject buySlotPrefab;  // ���� ���� ������
    public GameObject sellSlotPrefab;  // �Ǹ� ���� ������
    public Button nextBuyPageButton;  // ���� ������ ���� ������ ��ư
    public Button prevBuyPageButton;  // ���� ������ ���� ������ ��ư
    public Button nextSellPageButton;  // �Ǹ� ������ ���� ������ ��ư
    public Button prevSellPageButton;  // �Ǹ� ������ ���� ������ ��ư


    [Header("Item List")]
    public List<ItemInfo> buyItemList = new List<ItemInfo>();  // ���� ������ ������ ����Ʈ
    public List<ItemInfo> sellItemList = new List<ItemInfo>();  // �Ǹ� ������ ������ ����Ʈ

    private int currentBuyPage = 0;
    private int currentSellPage = 0;
    private const int itemsPerPage = 5;  // ������ �� ������ ��

    void Start()
    {
        /*
        buyItemList.Add(new ItemInfo("Sword", "A sharp blade.", null, 100));
        sellItemList.Add(new ItemInfo(1, 100, "Old Sword", "A worn-out sword.", null, 30));
        */
        // ��ư ������ ����
        nextBuyPageButton.onClick.AddListener(ShowNextBuyPage);
        prevBuyPageButton.onClick.AddListener(ShowPreviousBuyPage);
        nextSellPageButton.onClick.AddListener(ShowNextSellPage);
        prevSellPageButton.onClick.AddListener(ShowPreviousSellPage);

    }
    // ���� ������ �޾ƿ��� 
    public void GetBuyData(List<ItemInfo> data)
    {
        buyItemList = data;
        LoadItemsForSellPage();
    }
    // �κ��丮 ������ �޾ƿ���
    public void GetSellData()
    {

        LoadItemsForBuyPage();
    }
    // ���� ������ �ε�
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
            slot.SetActive(true);  // ���� �� Ȱ��ȭ
            ItemInfo item = buyItemList[i];

            ItemSlotBuy itemSlot = slot.GetComponent<ItemSlotBuy>();
            itemSlot.SetItem(item);
        }

    }

    // �Ǹ� ������ �ε�
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

    // ���� ������ �̵�
    private void ShowNextBuyPage() { if ((currentBuyPage + 1) * itemsPerPage < buyItemList.Count) { currentBuyPage++; LoadItemsForBuyPage(); } }
    private void ShowPreviousBuyPage() { if (currentBuyPage > 0) { currentBuyPage--; LoadItemsForBuyPage(); } }

    // �Ǹ� ������ �̵�
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
