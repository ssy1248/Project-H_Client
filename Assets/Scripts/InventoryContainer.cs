using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class InventoryContainer : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Transform itemSlotParent;
    public Button btn_close;
    public ItemInfoPanel itemInfoPanel;
    public EquipmentContainer equipmentContainer;

    private List<InventorySlot> itemSlots = new List<InventorySlot>();
    private bool isShowing = false;

    // Start is called before the first frame update
    void Start()
    {
        // 버튼에 이벤트 핸들러 부착
        btn_close.onClick.AddListener(Hide);
        foreach (Transform child in itemSlotParent)
        {
            if (child.gameObject.TryGetComponent(out InventorySlot slot))
            {
                itemSlots.Add(slot);
                slot.onPointerEnterAction += ShowItemInfoPanel;
                slot.onPointerExitAction += HideItemInfoPanel;
                slot.onRightClickAction += OnRightClickHandler;
            }
        }

        // 패킷 핸들러 이벤트 구독
        PacketHandler.S_InventoryEvent += UpdateInventory;
        PacketHandler.S_EquipItemEvent += EquipItem;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddItem(ItemInfo item)
    {
        // 빈 슬롯 찾기
        foreach (var slot in itemSlots)
        {
            if (slot.isEmpty)
            {
                slot.Init(item);
                return;
            }
        }
        // 빈 슬롯이 없는 경우

    }

    public void AddItem(ItemInfo item, int index)
    {
        if (!itemSlots[index].isEmpty)
        {
            AddItem(item);
        }
        itemSlots[index].Init(item);
    }

    public ItemInfo RemoveItem(InventorySlot slot)
    {
        // 아이템 제거: 아이템을 해당 슬롯에서 제거
        var item = slot.data;
        slot.Clear();
        return item;
    }

    public void DestroyItem(int index, ItemInfo item)
    {
        // 아이템 파괴: 복구 불가능, 아이템이 영구히 제거됨
    }

    public void UpdateInventory(S_InventoryResponse data)
    {
        // 인벤토리 갱신
        ClearSlots();

        // 슬롯에 아이템 정보 추가
        var _inventory = data.Inventory;
        for (var i = 0; i < _inventory.Count; i++)
        {
            if (_inventory[i].Equiped)
            {
                equipmentContainer.Equip(_inventory[i]);
                continue;
            }
            AddItem(_inventory[i]);
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in itemSlots)
        {
            // 슬롯 데이터 초기화
            slot.Clear();
        }
    }

    private void ShowItemInfoPanel(InventorySlot slot)
    {
        itemInfoPanel.Init(slot.data);
        itemInfoPanel.Show();
    }

    private void HideItemInfoPanel()
    {
        itemInfoPanel.Hide();
    }

    private void OnRightClickHandler(InventorySlot slot)
    {
        if (slot.isEmpty) return;
        // ItemType
        // 0: 소모성 아이템
        // 1: 비소모성 아이템
        // 2: 머리
        // 3: 상의
        // 4: 하의
        // 5: 신발
        // 6: 무기
        var item = slot.data;
        switch (item.ItemType)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                // 서버에 아이템 장착 메세지 전송
                C_EquipItemRequest equipRequest = new C_EquipItemRequest
                {
                    ItemId = item.Id,
                };
                Debug.Log(equipRequest);
                GameManager.Network.Send(equipRequest);
                break;
        }
    }

    public void EquipItem(S_EquipItemResponse data)
    {
        Debug.Log(data);
        // // TODO : S_EquipItemResponse 핸들러에서 실행하도록 수정
        // equipmentContainer.Equip(item);
        // RemoveItem(slot); // TODO : ItemInfo.index 추가
    }

    public void Toggle()
    {
        if (isShowing)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        isShowing = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        isShowing = false;
    }
}
