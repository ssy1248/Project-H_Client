using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Marketplace : MonoBehaviour
{
    public List<GameObject> checkObject;

    [SerializeField] GameObject[] slotObject;
    [SerializeField] List<GameObject> buttons = new List<GameObject>();
    [SerializeField] int marketPage = 1;
    [SerializeField] int selectPage = 1;
    int maxMarketPage = 1;
    [SerializeField] int inventoryPage = 1;
    int maxInventoryPage = 1;
    [SerializeField] int count = 5;
    [SerializeField] int slotGap = 200;
    [SerializeField] Dictionary<int, GameObject> buyslots = new Dictionary<int, GameObject>();
    [SerializeField] Dictionary<int, GameObject> Sellslots = new Dictionary<int, GameObject>();
    [SerializeField] TextMeshProUGUI pageText;
    [SerializeField] GameObject[] pageObject;
    [SerializeField] TMP_InputField selectData;
    [SerializeField] string selectName = "";

    private Action onConfirmAction;
    public void Awake()
    {
        InitSlot();
    }
    private void OnEnable()
    {
        ChangePage(0);
        
    }
    public void OnDisable()
    {
        marketPage = 1;
        inventoryPage = 1;
        maxMarketPage = 1;
        maxInventoryPage = 1;
    }
    // 페이지 조작 함수
    public void NextMarketPage()
    {
        marketPage++;
        MarketPageChangeRequest();
    }
    public void NextInventoryPage()
    {
        inventoryPage++;
        InventoryPageChangeRequest();
    }
    public void BeforeMarketPage()
    {
        marketPage--;
        MarketPageChangeRequest();
    }
    public void BeforeInventoryPage()
    {
        inventoryPage--;
        InventoryPageChangeRequest();
    }
    public void BeforSelectePage()
    {
        selectPage--;
        SelectBuyInMarket();
    }
    public void NextSelectPage()
    {
        selectPage++;
        SelectBuyInMarket();
    }
    //페이지 요청 함수 
    public void MarketPageChangeRequest()
    {
        TownManager.Instance.MarketListRequest(marketPage,count);
    }
    public void InventoryPageChangeRequest()
    {
        TownManager.Instance.MarketMyListRequest(inventoryPage, count);
    }
    public void SelectBuyInMarket()
    {
        TownManager.Instance.MarketSelectBuyNameRequest(selectPage,count, selectName);
    }
    //페이지 변경 함수
    public void ChangePage(int pageIndex)
    {
        // 페이지 전환
        for (int i = 0; i < pageObject.Length; i++)
        {
            pageObject[i].SetActive(i == pageIndex);
        }
        // 버튼 전환

        // 버튼 클릭 리스너 설정
        switch (pageIndex) 
        { 
            case 0:
                buttons[0].GetComponent<Button>().onClick.RemoveAllListeners();
                buttons[0].GetComponent<Button>().onClick.AddListener(NextMarketPage);
                buttons[1].GetComponent<Button>().onClick.RemoveAllListeners();
                buttons[1].GetComponent<Button>().onClick.AddListener(BeforeMarketPage);
                MarketPageChangeRequest();
                break;
            case 1:
                buttons[0].GetComponent<Button>().onClick.RemoveAllListeners();
                buttons[0].GetComponent<Button>().onClick.AddListener(NextInventoryPage);
                buttons[1].GetComponent<Button>().onClick.RemoveAllListeners();
                buttons[1].GetComponent<Button>().onClick.AddListener(BeforeInventoryPage);
                InventoryPageChangeRequest();
                break;
            case 2:
                pageObject[0].SetActive(true);
                buttons[0].GetComponent<Button>().onClick.RemoveAllListeners();
                buttons[0].GetComponent<Button>().onClick.AddListener(NextSelectPage);
                buttons[1].GetComponent<Button>().onClick.RemoveAllListeners();
                buttons[1].GetComponent<Button>().onClick.AddListener(BeforSelectePage);
                break;
        }
    }

    //데이터 넣어주기 인벤토리
    public void SetSellData(S_MarketMyList data)
    {
        for (int i = 0; i< count; i++)
        {
            if (i < data.Itemdata.Count)
            {
                Sellslots[i].GetComponent<MarketSellSlot>().SetData(data.Itemdata[i]);
            }
            else
            {
                Sellslots[i].SetActive(false);
            }
        }
        buttons[0].SetActive(inventoryPage > 0);
        buttons[1].SetActive(inventoryPage > 1);
        
    }
    // 데이터 넣어주기 마켓
    public void SetBuyData(S_MarketList data)
    {
        for (int i = 0; i < count; i++)
        {
            if (i < data.Itemdata.Count)
            {
                buyslots[i].GetComponent<MarkeBuySlot>().SetData(data.Itemdata[i]);
            }
            else
            {
                buyslots[i].SetActive(false);
            }
        }
        buttons[0].SetActive(marketPage > 0);
        buttons[1].SetActive(marketPage > 1);
    }
    public void SetSelectData(S_MarketSelectBuyName data)
    {
        for (int i = 0; i < count; i++)
        {
            if (i < data.Itemdata.Count)
            {
                //buyslots[i].GetComponent<MarkeBuySlot>().SetData(data.Itemdata[i]);
            }
            else
            {
                buyslots[i].SetActive(false);
            }
        }
        buttons[0].SetActive(marketPage > 0);
        buttons[1].SetActive(marketPage > 1);
    }
    // 클릭시 발동 
    public void CheckSelect()
    {
        checkObject[2].SetActive(true);
    }
    public void CheckBuy(Action onConfirm)
    {
        onConfirmAction = onConfirm;
        checkObject[0].SetActive(true);
    }
    public void CheckSell(Action onConfirm)
    {
        onConfirmAction = onConfirm;
        checkObject[1].SetActive(true);
    }
    // 수락
    public void AcceptSelect()
    {
        selectName = selectData.text;
        checkObject[2].SetActive(false);
        SelectBuyInMarket();
        ChangePage(2);
    }
    public void AcceptBuy()
    {
        checkObject[0].SetActive(false);
        onConfirmAction?.Invoke();
    }
    public void AcceptSell()
    {
        checkObject[1].SetActive(false);
        onConfirmAction?.Invoke();
    }
    // 캔슬 
    public void CancelSelect()
    {
        checkObject[2].SetActive(false);
    }
    public void CancelBuy()
    {
        checkObject[0].SetActive(false);
    }
    public void CancelSell()
    {
        checkObject[1].SetActive(false);
    }
    public void InitSlot()
    {
        // 구매 슬롯 생성
        for (int i = 0; i < count; i++)
        {
            GameObject temp = Instantiate(slotObject[0], pageObject[0].transform);
            temp.GetComponent<MarkeBuySlot>().InitSlot(this);
            RectTransform trTemp = temp.GetComponent<RectTransform>();
            Debug.Log(i * slotGap);
            Debug.Log(trTemp.anchoredPosition.x + i *slotGap);
            trTemp.anchoredPosition = new Vector2(trTemp.anchoredPosition.x + i * slotGap,0 );
            buyslots.Add(i, temp);
        }
        // 판매 슬롯 생성 
        for (int i = 0; i < count; i++)
        {
            GameObject temp = Instantiate(slotObject[1], pageObject[1].transform);
            temp.GetComponent<MarketSellSlot>().InitSlot(this);
            RectTransform trTemp = temp.GetComponent<RectTransform>();
            trTemp.anchoredPosition = new Vector2(trTemp.anchoredPosition.x + i * slotGap, trTemp.anchoredPosition.y );
            Sellslots.Add(i, temp);
        }
    }
}
