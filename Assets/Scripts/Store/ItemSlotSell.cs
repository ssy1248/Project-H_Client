using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Protobuf.Protocol;

public class ItemSlotSell : MonoBehaviour
{
    public TMP_Text itemTitleText;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemPriceText;
    public Image itemIconImage;
    public Button sellButton;

    private Item currentItem;

    public void SetItem(ItemInfo item)
    {
        /*
        currentItem = item;
        itemTitleText.text = item.itemTitle;
        itemDescriptionText.text = item.itemDescription;
        itemIconImage.sprite = item.itemIcon;
        itemPriceText.text = item.itemPrice.ToString();
        */

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(SellItem);

    }

    private void SellItem()
    {
        Debug.Log($"Sold {currentItem.itemTitle} for {currentItem.itemPrice} Gold!");
        // 실제 판매 로직 (예: 플레이어 소지금 증가 등) 추가 가능
    }
}
