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
        nowGold = nowitem.Price;
        nowTime = data.Time;
        maxRewardItems = data.MaxRewardItems;
        maxRewardText.SetText("���� ��ǰ : "+ maxRewardItems);
        buyText.SetText("���� ������ : \n" + nowName + "\n���� ���� : \n" + nowGold);
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
        buyText.SetText("���� ������ : \n"+ nowName+ "\n���� ���� : \n" + nowGold);
    }
    public void AuctionBid()
    {
        int gold = int.Parse(goldInput.text);
        if (gold > nowGold)
        {
            DungeonManager.Instance.EnterAuctionBid(gold, id);
        }
    }
    // ������ �����ֱ� �뵵 

    public void GetReward(string name, int data , bool isItem)
    {
        if (isItem)
        {
            GetRewardText.SetText(name + " �� \n" +ItemManager.instance.GetBuyId(data).Name +" X 1 \n������ ȹ�� ");
        }
        else
        {
            GetRewardText.SetText(name + " �� \n" + data + "G  \n���? ȹ�� ");
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
            stateText.SetText("�̸� : \n" + nowitem.Name + "\n��� : \n" + "\n���� : \n" + nowitem.ItemType);
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
