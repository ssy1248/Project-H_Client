using System;
using System.Collections.Generic;
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
                slot.Init(i++, InventorySlot.SlotType.INVENTORY);
            }
        }
        // 패킷 핸들러 이벤트 구독
        PacketHandler.S_InventoryEvent += S_UpdateInventoryHandler;
        PacketHandler.S_MoveItemEvent += S_MoveItemHandler;
        PacketHandler.S_ActiveItemEvent += S_ActiveItemResponseHandler;
    }

    #region public
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

    public void AddItem(ItemInfo item, InventorySlot slot)
    {
        if (!slot.isEmpty)
        {
            AddItem(item);
            return;
        }
        if (!itemSlots.Contains(slot))
        {
            AddItem(item);
            return;
        }
        slot.SetItem(item);
    }

    public ItemInfo RemoveItem(int index)
    {
        var item = itemSlots[index].data;
        itemSlots[index].ClearItem();
        return item;
    }

    public ItemInfo RemoveItem(InventorySlot slot)
    {
        // 아이템 제거: 아이템을 해당 슬롯에서 제거
        var item = slot.data;
        slot.ClearItem();
        return item;
    }

    public InventorySlot FindItemSlot(int itemId)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.isEmpty) continue;
            if (slot.data.Id == itemId) return slot;
        }
        return null;
    }

    public ItemInfo FindItem(int itemId)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.isEmpty) continue;
            if (slot.data.Id == itemId) return slot.data;
        }
        return null;
    }
    #endregion
    #region private
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

    /// <summary>
    /// 인벤토리 슬롯 클릭 핸들러. 아이템 타입에 따라 사용하거나 장착한다.
    /// </summary>
    /// <param name="eventData">포인터 이벤트 데이터</param>
    /// <param name="slot">이벤트가 호출된 슬롯</param>
    private void OnRightClickHandler(PointerEventData eventData, InventorySlot slot)
    {
        // 슬롯이 비어있다면 아무것도 하지 않는다
        if (slot.isEmpty) return;
        var item = slot.data;
        switch (item.ItemType)
        {
            case 0:
                // 아이템 사용
                C_ActiveItemRequest(slot.data.Id);
                break;
            case 1:
                break;
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                // 아이템 장착
                equipmentContainer.Equip(item);
                break;
        }
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

    /// <summary>
    /// 인벤토리 슬롯 드래그 핸들러. 드랍한 곳의 타입에 따라 아이템을 이동하거나 장비한다.
    /// </summary>
    /// <param name="eventData"></param>
    /// <param name="slot"></param>
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
                if (other.type == InventorySlot.SlotType.INVENTORY)
                {
                    // 인벤토리 안에서 놓으면
                    // swap
                    if (slot != other)
                    {
                        // 서버에 아이템 이동 메세지 전송
                        C_MoveItemRequest(slot, other.index);
                        break;
                    }
                }
                else if (other.type == InventorySlot.SlotType.EQUIPMENT)
                {
                    // 장비 창에 놓으면
                    var equipmentSlot = other as EquipmentSlot;

                    // 올바른 자리에 놓았는지 확인
                    if (equipmentSlot.itemType == slot.data.ItemType)
                    {
                        equipmentContainer.Equip(slot.data);
                    }
                }
            }
        }
        return;
    }

    private void S_UpdateInventoryHandler(S_InventoryResponse data)
    {
        // 인벤토리 갱신
        ClearSlots();

        // 슬롯에 아이템 정보 추가
        var _inventory = data.Inventory;
        for (var i = 0; i < _inventory.Count; i++)
        {
            if (_inventory[i].Equiped)
            {
                equipmentContainer.SetItem(_inventory[i]);
            }
            else
            {
                AddItem(_inventory[i], _inventory[i].Position);
            }
        }
    }

    private void S_MoveItemHandler(S_MoveItemResponse data)
    {
        // 옮기려는 아이템
        var slot = FindItemSlot(data.ItemId);
        if (slot == null)
        {
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

    private void S_ActiveItemResponseHandler(S_ActiveItemResponse data)
    {
        Debug.Log("S_ActiveItemResponseHandler");
        Debug.Log(data.UserId);
        Debug.Log(data.Id);
        // 아이템을 가지고 있는지 확인
        var slot = FindItemSlot(data.Id);
        if (!slot)
        {
            return;
        }
        var item = slot.data;

        // 아이템 효과 적용
        ItemManager.instance.ActiveItemHandler(item.ItemId, data);
        // 아이템 제거
        if (item.Quantity > 1)
        {
            item.Quantity -= 1;
            slot.SetItem(item);
        }
        else
        {
            slot.ClearItem();
        }
    }

    private void C_MoveItemRequest(InventorySlot slot, int to)
    {
        C_MoveItemRequest moveRequest = new C_MoveItemRequest
        {
            ItemId = slot.data.Id,
            Position = to,
        };
        GameManager.Network.Send(moveRequest);
    }

    private void C_ActiveItemRequest(int inventoryId)
    {
        C_ActiveItemRequest itemRequest = new C_ActiveItemRequest
        {
            Id = inventoryId,
            Timestamp = DateTime.UtcNow.Ticks,
        };
        GameManager.Network.Send(itemRequest);
    }
    #endregion

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
