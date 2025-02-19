using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Protobuf.Protocol;

public class ItemSlotBuy : MonoBehaviour
{
    public TMP_Text itemTitleText;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemPriceText;
    public Image itemIconImage;
    public Button buyButton;

    private ItemInfo currentItem;

    public void SetItem(ItemInfo item)
    {
        currentItem = item; 
        
        currentItem = item;
        itemTitleText.text = item.Name;
        itemDescriptionText.text = item.ItemType.ToString();
        itemPriceText.text = item.Price.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(BuyItem);
        
    }

    private void BuyItem()
    {
        TownManager.Instance.BuyItemRequest(currentItem.Name, currentItem.Price);
        // 실제 구매 로직 (예: 플레이어 소지금 차감 등) 추가 가능
    }
}
