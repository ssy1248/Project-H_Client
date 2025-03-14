using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketSellSlot : MonoBehaviour
{
    Marketplace marketplaceTemp;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TMP_TextInfo priceText;
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] Image itemImg;

    [SerializeField] ItemInfo iteminfoData;

    public void InitSlot(Marketplace temp)
    {
        marketplaceTemp = temp;
    }
    public void SetData(ItemInfo data)
    {
        iteminfoData = data;
        itemImg.sprite = ItemManager.instance.GetItemImg(data.Imgsrc);
        nameText.SetText(data.Name);
        itemText.SetText(data.ItemType.ToString());
        gameObject.SetActive(true);
    }
    public void Check()
    {
        marketplaceTemp.CheckSell(SellInMarket);
    }
    public void SellInMarket()
    {
        TownManager.Instance.SellInMarketRequest(iteminfoData.Id, iteminfoData.ItemType, Convert.ToInt32(marketplaceTemp.goldData.text));
    }
}
