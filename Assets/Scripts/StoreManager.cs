using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public List<Item> buyItemList = new List<Item>();  // ���� ������ ������ ����Ʈ
    public List<Item> sellItemList = new List<Item>();  // �Ǹ� ������ ������ ����Ʈ

    private int currentBuyPage = 0;
    private int currentSellPage = 0;
    private const int itemsPerPage = 5;  // ������ �� ������ ��

    void Start()
    {

        // ���� ������ �߰� (���� ���ӿ����� DB�� �ٸ� ������� ������ ���� ����)
        buyItemList.Add(new Item("Sword", "A sharp blade.", null, 100));
        buyItemList.Add(new Item("Shield", "A sturdy shield.", null, 150));
        buyItemList.Add(new Item("Potion", "Restores health.", null, 50));
        buyItemList.Add(new Item("Sword", "A sharp blade.", null, 100));
        buyItemList.Add(new Item("Shield", "A sturdy shield.", null, 150));
        buyItemList.Add(new Item("Potion", "Restores health.", null, 50));
        buyItemList.Add(new Item("Sword", "A sharp blade.", null, 100));
        buyItemList.Add(new Item("Shield", "A sturdy shield.", null, 150));
        buyItemList.Add(new Item("Potion", "Restores health.", null, 50));

        sellItemList.Add(new Item("Old Sword", "A worn-out sword.", null, 30));
        sellItemList.Add(new Item("Used Potion", "Half-used potion.", null, 10));

        // ��ư ������ ����
        nextBuyPageButton.onClick.AddListener(ShowNextBuyPage);
        prevBuyPageButton.onClick.AddListener(ShowPreviousBuyPage);
        nextSellPageButton.onClick.AddListener(ShowNextSellPage);
        prevSellPageButton.onClick.AddListener(ShowPreviousSellPage);

        // ù ������ �ε�
        LoadItemsForBuyPage();
        LoadItemsForSellPage();
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
            Item item = buyItemList[i];

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
            Item item = sellItemList[i];

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
}
