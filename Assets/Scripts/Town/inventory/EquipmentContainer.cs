using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentContainer : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public InventoryContainer inventory;
    public Button btn_close;
    public CanvasGroup canvasGroup;
    public ItemInfoPanel itemInfoPanel;
    public EquipmentSlot headSlot, shirtSlot, pantsSlot, righthandSlot, lefthandSlot, footSlot;
    private bool isShowing = false;
    private Transform originalParent;
    private Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        btn_close.onClick.AddListener(Hide);
        InitializeSlot(headSlot, 2); // TODO : 하드코딩. itemType 관리 필요
        InitializeSlot(shirtSlot, 3);
        InitializeSlot(pantsSlot, 4);
        InitializeSlot(righthandSlot, 6);
        InitializeSlot(lefthandSlot, 6);
        InitializeSlot(footSlot, 5);
        PacketHandler.S_EquipItemEvent += S_EquipItemResponseHandler;
        PacketHandler.S_DisrobeItemEvent += S_DisrobeItemResponseHandler;
    }

    #region public
    /// <summary>
    /// 아이템을 장착하는 함수.
    /// </summary>
    /// <param name="item">장착할 아이템</param>
    public void Equip(ItemInfo item)
    {
        if (item == null) return;
        C_EquipItemRequest(item);
    }

    public void Disrobe(InventorySlot slot)
    {
        if (slot.isEmpty) return;
        C_DisrobeItemRequest(slot);
    }

    public void SetItem(ItemInfo item)
    {
        var slot = FindTypeSlot(item);
        if(slot == null){
            Debug.LogError("아이템 타입이 잘못되었습니다");
        }
        // 장비 슬롯에 장착
        slot.SetItem(item);
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
    #endregion
    #region private
    private void InitializeSlot(EquipmentSlot slot, int itemType)
    {
        slot.onPointerEnterAction += ShowItemInfoPanel;
        slot.onPointerExitAction += HideItemInfoPanel;
        slot.onRightClickAction += OnRightClickHandler;
        slot.onBeginDragAction += OnDragBeginHandler;
        slot.onDragAction += OnDragHandler;
        slot.onEndDragAction += OnDragEndHandler;
        slot.Init(0, InventorySlot.SlotType.EQUIPMENT, itemType);
    }
    private EquipmentSlot FindItemSlot(int itemId)
    {
        if (!headSlot.isEmpty)
            if (headSlot.data.Id == itemId)
                return headSlot;
        if (!shirtSlot.isEmpty)
            if (shirtSlot.data.Id == itemId)
                return shirtSlot;
        if (!pantsSlot.isEmpty)
            if (pantsSlot.data.Id == itemId)
                return pantsSlot;
        if (!footSlot.isEmpty)
            if (footSlot.data.Id == itemId)
                return footSlot;
        if (!righthandSlot.isEmpty)
            if (righthandSlot.data.Id == itemId)
                return righthandSlot;
        if (!lefthandSlot.isEmpty)
            if (lefthandSlot.data.Id == itemId)
                return lefthandSlot;
        return null;
    }
    private EquipmentSlot FindItemSlot(ItemInfo item)
    {
        var itemId = item.Id;
        if (!headSlot.isEmpty)
            if (headSlot.data.Id == itemId)
                return headSlot;
        if (!shirtSlot.isEmpty)
            if (shirtSlot.data.Id == itemId)
                return shirtSlot;
        if (!pantsSlot.isEmpty)
            if (pantsSlot.data.Id == itemId)
                return pantsSlot;
        if (!footSlot.isEmpty)
            if (footSlot.data.Id == itemId)
                return footSlot;
        if (!righthandSlot.isEmpty)
            if (righthandSlot.data.Id == itemId)
                return righthandSlot;
        if (!lefthandSlot.isEmpty)
            if (lefthandSlot.data.Id == itemId)
                return lefthandSlot;
        return null;
    }

    private EquipmentSlot FindTypeSlot(ItemInfo item)
    {
        switch (item.ItemType)
        {
            case 2:
                return headSlot;
            case 3:
                return shirtSlot;
            case 4:
                return pantsSlot;
            case 5:
                return footSlot;
            case 6:
                return righthandSlot;
        }
        return null;
    }

    private void OnRightClickHandler(PointerEventData eventData, InventorySlot slot)
    {
        if (slot.isEmpty) return;
        C_DisrobeItemRequest(slot);
    }

    private void OnDragBeginHandler(PointerEventData eventData, InventorySlot slot)
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

    private void OnDragEndHandler(PointerEventData eventData, InventorySlot slot)
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
                    // 인벤토리에 놓으면
                    if (other.isEmpty)
                    {
                        // 빈칸에 놓으면 장비 해제
                        C_DisrobeItemRequest(slot);
                        break;
                    }
                    else
                    {
                        // 빈칸이 아니면
                        // 놓은 칸의 장비 타입이 같은지 확인
                        if (other.data.ItemType == slot.data.ItemType)
                        {
                            // 장비 타입이 같다면
                            // 장비 교체
                            C_EquipItemRequest(other.data);
                        }
                    }
                }
            }
        }
        return;
    }

    private void S_EquipItemResponseHandler(S_EquipItemResponse data)
    {
        if (data.Success)
        {
            // 인벤토리에서 itemId로 아이템 검색
            var inventorySlot = inventory.FindItemSlot(data.ItemId);
            if (inventorySlot == null)
            {
                Debug.LogError("아이템을 찾을 수 없습니다");
                return;
            }
            var item = inventory.RemoveItem(inventorySlot);
            EquipmentSlot slot = FindTypeSlot(item);
            // 장비 슬롯이 비어있는지 확인
            if (slot == null)
            {
                Debug.LogError("아이템 타입이 올바르지 않습니다");
                return;
            }
            // 장비 슬롯 비우기
            if (!slot.isEmpty)
            {
                var disrobed = slot.data;
                slot.ClearItem();
                inventory.AddItem(disrobed);
            }
            // 장비 슬롯에 장착
            slot.SetItem(item);
        }
    }

    private void S_DisrobeItemResponseHandler(S_DisrobeItemResponse data)
    {
        if (data.Success)
        {
            var slot = FindItemSlot(data.ItemId);
            if (slot == null)
            {
                Debug.LogError("장비하지 않은 아이템입니다");
                return;
            }
            var disrobed = slot.data;
            slot.ClearItem();
            inventory.AddItem(disrobed);
        }
    }

    private void C_EquipItemRequest(ItemInfo item)
    {
        C_EquipItemRequest equipRequest = new C_EquipItemRequest
        {
            ItemId = item.Id,
        };
        GameManager.Network.Send(equipRequest);
    }

    private void C_DisrobeItemRequest(InventorySlot slot)
    {
        C_DisrobeItemRequest disrobeRequest = new C_DisrobeItemRequest
        {
            ItemId = slot.data.Id,
        };
        GameManager.Network.Send(disrobeRequest);
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
    #endregion
}
