using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private Transform originalParent;

    // Start is called before the first frame update
    void Start()
    {
        // 버튼에 이벤트 핸들러 부착
        btn_close.onClick.AddListener(Hide);
        var i = 0;
        foreach (Transform child in itemSlotParent)
        {
            if (child.gameObject.TryGetComponent(out InventorySlot slot))
            {
                itemSlots.Add(slot);
                slot.onPointerEnterAction += ShowItemInfoPanel;
                slot.onPointerExitAction += HideItemInfoPanel;
                slot.onRightClickAction += OnRightClickHandler;
                slot.onBeginDragAction += OnBeginDragHandler;
                slot.onDragAction += OnDragHandler;
                slot.onEndDragAction += OnEndDragHandler;
                slot.Init(i++);
            }
        }
        // 패킷 핸들러 이벤트 구독
        PacketHandler.S_InventoryEvent += UpdateInventory;
        PacketHandler.S_EquipItemEvent += EquipItem;
        PacketHandler.S_MoveItemEvent += MoveItemHandler;
    }

    public void AddItem(ItemInfo item)
    {
        // 빈 슬롯 찾기
        foreach (var slot in itemSlots)
        {
            if (slot.isEmpty)
            {
                slot.SetItem(item);
                return;
            }
        }
        // 빈 슬롯이 없는 경우

    }

    public void AddItem(ItemInfo item, int index)
    {
        if (!itemSlots[index].isEmpty)
        {
            // 아이템 슬롯이 비어있지 않으면
            AddItem(item);
            return;
        }
        itemSlots[index].SetItem(item);
    }

    public ItemInfo RemoveItem(InventorySlot slot)
    {
        // 아이템 제거: 아이템을 해당 슬롯에서 제거
        var item = slot.data;
        slot.ClearItem();
        return item;
    }

    public void DestroyItem(int index, ItemInfo item)
    {
        // 아이템 파괴: 복구 불가능, 아이템이 영구히 제거됨
    }

    private void UpdateInventory(S_InventoryResponse data)
    {
        // 인벤토리 갱신
        ClearSlots();

        // 슬롯에 아이템 정보 추가
        var _inventory = data.Inventory;
        for (var i = 0; i < _inventory.Count; i++)
        {
            Debug.Log(_inventory[i]);
            if (_inventory[i].Equiped)
            {
                equipmentContainer.Equip(_inventory[i]);
                continue;
            }
            AddItem(_inventory[i], _inventory[i].Position);
        }
    }


    private InventorySlot FindItemSlot(int itemId)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.isEmpty) continue;
            if (slot.data.Id == itemId) return slot;
        }
        return null;
    }

    private void ClearSlots()
    {
        foreach (var slot in itemSlots)
        {
            // 슬롯 데이터 초기화
            slot.ClearItem();
        }
    }

    private void ShowItemInfoPanel(PointerEventData eventData, InventorySlot slot)
    {
        itemInfoPanel.Init(slot.data);
        itemInfoPanel.Show();
    }

    private void HideItemInfoPanel(PointerEventData eventData)
    {
        itemInfoPanel.Hide();
    }

    private void OnRightClickHandler(PointerEventData eventData, InventorySlot slot)
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

    private void OnPointerUpHandler(InventorySlot slot)
    {

    }

    private void OnBeginDragHandler(PointerEventData eventData, InventorySlot slot)
    {
        itemInfoPanel.gameObject.SetActive(false);
        originalParent = slot.itemImage.transform.parent;
        slot.itemImage.transform.SetParent(canvasGroup.transform, true);
    }

    private void OnDragHandler(PointerEventData eventData, InventorySlot slot)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasGroup.transform as RectTransform, Input.mousePosition, null, out Vector2 localPoint);
        slot.itemImage.rectTransform.localPosition = localPoint;
    }

    private void OnEndDragHandler(PointerEventData eventData, InventorySlot slot)
    {
        slot.itemImage.transform.SetParent(originalParent, true);
        slot.itemImage.rectTransform.localPosition = Vector3.zero;
        itemInfoPanel.gameObject.SetActive(true);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var hit in results)
        {
            if (hit.gameObject.TryGetComponent<InventorySlot>(out var other))
            {
                // swap
                if (other != null && slot != other)
                {
                    // 서버에 아이템 이동 메세지 전송
                    C_MoveItemRequest moveRequest = new C_MoveItemRequest
                    {
                        ItemId = slot.data.Id,
                        Position = other.index,
                    };

                    GameManager.Network.Send(moveRequest);
                    break;
                }
            }
        }
        return;
    }

    private void EquipItem(S_EquipItemResponse data)
    {
        if (data.Success)
        {
            var slot = FindItemSlot(data.ItemId);
            if (slot == null)
            {
                Debug.LogError("slot not found");
                return;
            }
            equipmentContainer.Equip(slot.data);
            RemoveItem(slot);
        }
    }

    private void MoveItemHandler(S_MoveItemResponse data)
    {
        // 옮기려는 아이템
        var slot = FindItemSlot(data.ItemId);
        if(slot == null){
            Debug.LogError("SLot not found");
            return;
        }

        var item = slot.data;
        var targetSlot = itemSlots[data.Position];
        // 옮기려는 위치에 다른 아이템이 있으면
        if (!targetSlot.isEmpty)
        {
            // 스왑
            var other = targetSlot.data;
            targetSlot.SetItem(item);
            slot.SetItem(other);
        }
        else
        {
            // 아이템 옮기기
            targetSlot.SetItem(item);
            slot.ClearItem();
        }
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
