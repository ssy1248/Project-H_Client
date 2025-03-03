using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageContainer : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public EquipmentContainer equipmentContainer;
    public InventoryContainer inventoryContainer;
    public CanvasGroup canvasGroup;
    public Transform itemSlotParent;
    public Button btn_close;
    public ItemInfoPanel itemInfoPanel;

    private List<InventorySlot> itemSlots = new List<InventorySlot>();
    private bool isShowing = false;
    private Transform originalParent;
    private Vector2 offset;

    // Start is called before the first frame update
    private void Start()
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
                slot.Init(i++, InventorySlot.SlotType.STORAGE);
            }
        }
        // 패킷 핸들러 이벤트 구독
        PacketHandler.S_InventoryEvent += S_UpdateInventoryHandler;
        PacketHandler.S_MoveItemEvent += S_MoveItemHandler;
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
        if (index < 0 || !itemSlots[index].isEmpty)
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
    protected void ClearSlots()
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
    protected virtual void OnRightClickHandler(PointerEventData eventData, InventorySlot slot)
    {
        // 슬롯이 비어있다면 아무것도 하지 않는다
        if (slot.isEmpty) return;

        // 그냥 우클릭시 인벤토리로 이동
        C_MoveItemRequest(slot, -1, (int)InventorySlot.SlotType.INVENTORY);
    }

    private void OnBeginDragHandler(PointerEventData eventData, InventorySlot slot)
    {
        itemInfoPanel.gameObject.SetActive(false);
        originalParent = slot.itemImage.transform.parent;
        slot.itemImage.transform.SetParent(canvasGroup.transform, true);
    }

    private void OnDragHandler(PointerEventData eventData, InventorySlot slot)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasGroup.transform as RectTransform, eventData.position, null, out Vector2 localPoint);
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
                switch (other.type)
                {
                    case InventorySlot.SlotType.INVENTORY:
                        // 인벤토리에 놓으면 아이템 이동 요청
                        C_MoveItemRequest(slot, other.index, (int)InventorySlot.SlotType.INVENTORY);
                        return;
                    case InventorySlot.SlotType.EQUIPMENT:
                        // 장비 창에 놓으면
                        var equipmentSlot = other as EquipmentSlot;

                        // 올바른 자리에 놓았는지 확인
                        if (equipmentSlot.index == slot.data.ItemType)
                        {
                            // 장비 요청
                            C_MoveItemRequest(slot, slot.data.ItemType, (int)InventorySlot.SlotType.EQUIPMENT);
                        }
                        return;
                    case InventorySlot.SlotType.STORAGE:
                        // 창고에 놓으면 이동 요청
                        C_MoveItemRequest(slot, other.index, (int)InventorySlot.SlotType.STORAGE);
                        return;
                }
            }
        }
    }

    protected virtual void S_UpdateInventoryHandler(S_InventoryResponse data)
    {
        // 인벤토리 갱신
        ClearSlots();

        // 슬롯에 아이템 정보 추가
        var _inventory = data.Inventory;
        for (var i = 0; i < _inventory.Count; i++)
        {
            if (_inventory[i].Equipped == (int)InventorySlot.SlotType.STORAGE)
                AddItem(_inventory[i], _inventory[i].Position);
        }
    }

    private void S_MoveItemHandler(S_MoveItemResponse data)
    {
        var itemId = data.ItemId;
        var position = data.Position;
        var storage = data.Storage;

        // 옮기려는 아이템이 있는지 확인
        var slot = FindItemSlot(itemId);
        if (slot == null)
        {
            Debug.LogError("Slot not found");
            return;
        }

        var item = slot.data;
        switch (storage)
        {
            // storage -> inventory
            case (int)InventorySlot.SlotType.INVENTORY:
                var temp = inventoryContainer.RemoveItem(position);
                inventoryContainer.AddItem(item, position);
                slot.SetItem(temp);
                break;
            // storage -> storage
            case (int)InventorySlot.SlotType.STORAGE:
                temp = RemoveItem(position);
                AddItem(item, position);
                slot.SetItem(temp);
                break;
            // storage -> equipment
            case (int)InventorySlot.SlotType.EQUIPMENT:
                temp = equipmentContainer.Disrobe(item.ItemType);
                equipmentContainer.Equip(item);
                slot.SetItem(temp);
                break;
        }
    }

    protected void C_MoveItemRequest(InventorySlot slot, int position, int storage)
    {
        C_MoveItemRequest moveRequest = new C_MoveItemRequest
        {
            ItemId = slot.data.Id,
            Position = position,
            Storage = storage,
        };
        GameManager.Network.Send(moveRequest);
    }

    private void C_MoveItemRequest(int inventoryId, int position, int storage)
    {
        C_MoveItemRequest moveRequest = new C_MoveItemRequest
        {
            ItemId = inventoryId,
            Position = position,
            Storage = storage,
        };
        GameManager.Network.Send(moveRequest);
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, null, out offset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var p = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (p.x < 0 || p.x > 1 || p.y < 0 || p.y > 1)
        {
            return;
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, null, out Vector2 localPosition);
        (transform as RectTransform).anchoredPosition += localPosition - offset;
    }
}
