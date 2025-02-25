using Google.Protobuf.Protocol;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardAuction : MonoBehaviour
{
    [SerializeField] GameObject auctionObject;
    [SerializeField] GameObject itemStateObject;
    [SerializeField] GameObject getRewardObject;

    [SerializeField] Image rewardImg;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI buyText;
    [SerializeField] TMP_InputField goldInput;
    [SerializeField] TextMeshProUGUI stateText;
    [SerializeField] TextMeshProUGUI maxRewardText;
    [SerializeField] TextMeshProUGUI GetRewardText;

    string id = "";
    public bool isEnd = false;
    int nowTime = 0;
    int nowGold = 0;
    string nowName = "";
    ItemData nowitem ;
    int maxRewardItems = 0;

    public void StartAuction(S_SetAuctionData data)
    {
        Debug.Log(data);
        id = data.Id;
        nowitem = ItemManager.instance.GetBuyId(data.Itemid);
        nowitem.Rarity = data.Rarity;
        nowGold = nowitem.Price;
        nowTime = data.Time;
        maxRewardItems = data.MaxRewardItems;
        maxRewardText.SetText("남은 물품 : "+ maxRewardItems);
        buyText.SetText("현재 구매자 : \n" + nowName + "\n현재 가격 : \n" + nowGold);
        auctionObject.SetActive(true);
        StartCoroutine("TimeCheck");
    }
    public void WaitAuction(S_WaitAuction data)
    {
        auctionObject.SetActive(false);
        StopCoroutine("TimeCheck");
    }
    public void EndAuction()
    {
        isEnd = true;
        auctionObject.SetActive(false);
        StopCoroutine("TimeCheck");
    }
    public void ChangeGold(S_EnterAuctionBid data)
    {
        nowTime = data.Time;
        nowGold = data.Gold;
        nowName = data.Name;
        buyText.SetText("현재 구매자 : \n"+ nowName+ "\n현재 가격 : \n" + nowGold);
    }
    public void AuctionBid()
    {
        int gold = int.Parse(goldInput.text);
        if (gold > nowGold)
        {
            DungeonManager.Instance.EnterAuctionBid(gold, id);
        }
    }
    // 데이터 보여주기 용도 

    public void GetReward(string name, int data , bool isItem)
    {
        if (isItem)
        {
            GetRewardText.SetText(name + " 님 \n" +ItemManager.instance.GetBuyId(data).Name +" X 1 \n아이템 획득 ");
        }
        else
        {
            GetRewardText.SetText(name + " 님 \n" + data + "G  \n골드 획득 ");
        }
        getRewardObject.SetActive(true);
        
    }

    public void SetItemState()
    {
        if (itemStateObject.activeSelf)
        {
            itemStateObject.SetActive(false);
        }
        else
        {
            itemStateObject.SetActive(true);
            stateText.SetText("이름 : \n" + nowitem.Name + "\n레어도 : \n" + nowitem.Rarity + "\n종류 : \n" + nowitem.ItemType);
        }
    }
    public void SetReward()
    {
        if (getRewardObject.activeSelf)
        {
            getRewardObject.SetActive(false);
        }
        else
        {
            getRewardObject.SetActive(true);
        }
    }
    IEnumerator TimeCheck()
    {
        while (!isEnd)
        {
            nowTime--;
            timeText.text = nowTime.ToString();
            yield return new WaitForSeconds(1f);
        }
    }
}
