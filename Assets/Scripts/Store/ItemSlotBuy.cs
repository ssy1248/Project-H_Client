using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotBuy : MonoBehaviour
{
    public TMP_Text itemTitleText;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemPriceText;
    public Image itemIconImage;
    public Button buyButton;

    private Item currentItem;

    public void SetItem(Item item)
    {
        currentItem = item;
        itemTitleText.text = item.itemTitle;
        itemDescriptionText.text = item.itemDescription;
        itemIconImage.sprite = item.itemIcon;
        itemPriceText.text = item.itemPrice.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(BuyItem);

    }

    private void BuyItem()
    {
        Debug.Log($"Bought {currentItem.itemTitle} for {currentItem.itemPrice} Gold!");
        // 실제 구매 로직 (예: 플레이어 소지금 차감 등) 추가 가능
    }
}
