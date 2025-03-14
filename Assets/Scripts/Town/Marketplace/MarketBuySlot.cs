using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarkeBuySlot : MonoBehaviour
{
    Marketplace marketplaceTemp;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] Image itemImg;

    public marketItemInfo marketData;

    public void InitSlot(Marketplace temp) 
    {
        marketplaceTemp = temp;
    }
    public void SetData(marketItemInfo data)
    {
        marketData = data;
        itemImg.sprite = ItemManager.instance.GetItemImg(data.ItemId);
        nameText.SetText(data.Name);
        priceText.SetText(data.Price.ToString());
        itemText.SetText(data.ItemId.ToString());
        gameObject.SetActive(true);
    }
    public void Check()
    {
        marketplaceTemp.CheckBuy(BuyInMarket);
    }
    public void BuyInMarket()
    {
        TownManager.Instance.BuyInMarketRequest(marketData.MarketId);
    }
}
