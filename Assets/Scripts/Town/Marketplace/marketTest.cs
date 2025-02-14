using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class marketTest : MonoBehaviour
{
    public void getList()
    {
        TownManager.Instance.MarketListRequest(0, 5);
    }
    public void getMyList()
    {
        TownManager.Instance.MarketMyListRequest(0,5);
    }
    public void buyInMarket(int id)
    {
        TownManager.Instance.BuyInMarketRequest(id);
    }
    public void sellInMarket(int id)
    {
        TownManager.Instance.SellInMarketRequest(id, 1);
    }
}
