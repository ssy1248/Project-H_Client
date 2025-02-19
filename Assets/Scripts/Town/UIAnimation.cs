using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf.Protocol;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button btnMatch;
    [SerializeField] private Button[] btnList;

    [SerializeField] private MyPlayer mPlayer;
    [SerializeField] private InventoryContainer inventory;
    [SerializeField] GameObject marketplace;
    [SerializeField] private GameObject party;

    [SerializeField] int inventoryPage = 1;
    [SerializeField] int slotInPage = 10;
    [SerializeField] int slotDistance = 100;
    [SerializeField] GameObject slotObject;
    [SerializeField] Dictionary<int, GameObject> slots = new Dictionary<int, GameObject>();
    void Start()
    {
        
    }

    public void Init()
    {
        mPlayer = TownManager.Instance.MyPlayer?.MPlayer;
        //InitiallzeSlots();
        if (mPlayer == null)
        {
            Debug.LogError("MyPlayer instance is missing or not initialized.");
            return;
        }
        InitializeButtons();
        //marketplace.SetActive(false);
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < btnList.Length; i++)
        {
            int idx = i;
            btnList[i].onClick.AddListener(() => PlayAnimation(idx));
        }
    }

    private void PlayAnimation(int idx)
    {
        if (mPlayer == null)
        {
            Debug.LogWarning("Cannot play animation. MyPlayer instance is null.");
            return;
        }

        mPlayer.ExecuteAnimation(idx);
    }

    void InitiallzeSlots()
    {
        for (int i = 0; i < slotInPage; i++)
        {
            GameObject slotTemp = Instantiate(slotObject, inventory.transform);
            slotTemp.SetActive(false);
            RectTransform slotTr = slotTemp.GetComponent<RectTransform>();
            slotTr.localPosition = new Vector3(slotTr.localPosition.x, slotTr.localPosition.y - i * slotDistance, slotTr.localPosition.z);
            slots.Add(i, slotTemp);
        }
    }

    private void Update()
    {
        if (mPlayer == null)
        {
            return;
        }
        switch (true)
        {
            // �κ��丮 Ű 
            case var _ when Input.GetKeyDown(KeyCode.I):
                inventory.Toggle();
                break;
            case var _ when Input.GetKeyDown(KeyCode.M):
                if (marketplace.activeSelf)
                {
                    marketplace.SetActive(false);
                }
                else
                {
                    marketplace.SetActive(true);
                }
                break;
        }
    }

    public void UpdateInventory(S_InventoryResponse data)
    {
        // 인벤토리 갱신
        inventory.UpdateInventory(data);
    }

    public void MatchRequest()
    {
        // 1) 내 플레이어가 누군지 확인
        Player myPlayer = TownManager.Instance.MyPlayer;
        if (myPlayer == null)
        {
            Debug.LogWarning("내 플레이어가 존재하지 않아 매칭 요청을 보낼 수 없습니다.");
            return;
        }

        // 2) TownManager에서 “내 플레이어가 속한 파티” 가져오기
        PartyInfo myPartyInfo = TownManager.Instance.GetPartyInfoByPlayerId(myPlayer.PlayerId);
        Debug.Log($"에러전 파티 인포 : {myPartyInfo}");
        if (myPartyInfo == null)
        {
            Debug.Log("파티를 생성하고 매칭 신청을 해주세요");
            return;
        }

        Debug.Log($"찾은 파티 인포 : {myPartyInfo}");

        //if (myPartyInfo.PartyLeaderId != myPlayer.PlayerId)
        //{
        //    Debug.Log("매칭 요청은 파티장만 신청 가능합니다.");
        //    // 또는 경고 메시지를 띄운 후에도 요청을 보낼 수 있습니다.
        //    // return;
        //}

        // 3) MatchRequest 패킷 생성해서 파티 정보 넣기
        C_MatchRequest matchRequestPacket = new C_MatchRequest
        {
            Party = myPartyInfo
        };

        // 4) 서버로 전송
        GameManager.Network.Send(matchRequestPacket);
        Debug.Log("매칭 요청 패킷 전송 완료!");
    }

    public void MatchStopRequest()
    {
        /*
        // 1) 내 플레이어가 누군지 확인
        Player myPlayer = TownManager.Instance.MyPlayer;
        if (myPlayer == null)
        {
            Debug.LogWarning("내 플레이어가 존재하지 않아 매칭 요청을 보낼 수 없습니다.");
            return;
        }

        // 2) TownManager에서 “내 플레이어가 속한 파티” 가져오기
        PartyInfo myPartyInfo = TownManager.Instance.GetPartyInfoByPlayerId(myPlayer.PlayerId);
        C_MatchStopRequest matchStopRequestPacket = new C_MatchStopRequest { Party = myPartyInfo };
        GameManager.Network.Send(matchStopRequestPacket);
        */
    }

    public void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}