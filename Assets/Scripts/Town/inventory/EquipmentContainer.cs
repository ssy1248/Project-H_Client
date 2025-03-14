using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentContainer : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public InventoryContainer inventoryContainer;
    public StorageContainer storageContainer;
    public Button btn_close;
    public CanvasGroup canvasGroup;
    public ItemInfoPanel itemInfoPanel;
    public InventorySlot headSlot, shirtSlot, pantsSlot, righthandSlot, lefthandSlot, footSlot;
    private bool isShowing = false;
    private Transform originalParent;
    private Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        btn_close.onClick.AddListener(Hide);
        InitializeSlot(headSlot, (int)ItemType.Head); // TODO : 하드코딩. itemType 관리 필요
        InitializeSlot(shirtSlot, (int)ItemType.Shirt);
        InitializeSlot(pantsSlot, (int)ItemType.Pants);
        InitializeSlot(footSlot, (int)ItemType.Foot);
        InitializeSlot(righthandSlot, (int)ItemType.Weapon);
        InitializeSlot(lefthandSlot, (int)ItemType.Weapon);
        TownManager.Instance.S_MoveItemEvent += S_MoveItemHandler;
    }

    #region public
    /// <summary>
    /// 아이템을 장착하는 함수.
    /// </summary>
    /// <param name="item">장착할 아이템</param>
    public void Equip(ItemInfo item)
    {
        InventorySlot slot = null;
        switch (item.ItemType)
        {
            case (int)ItemType.Head:
                slot = headSlot;
                break;
            case (int)ItemType.Shirt:
                slot = shirtSlot;
                break;
            case (int)ItemType.Pants:
                slot = pantsSlot;
                break;
            case (int)ItemType.Foot:
                slot = footSlot;
                break;
            case (int)ItemType.Weapon:
                slot = righthandSlot;
                break;
        }
        if (slot == null) return;
        slot.SetItem(item);
    }

    public ItemInfo Disrobe(int itemType)
    {
        InventorySlot slot = null;
        switch (itemType)
        {
            case (int)ItemType.Head:
                slot = headSlot;
                break;
            case (int)ItemType.Shirt:
                slot = shirtSlot;
                break;
            case (int)ItemType.Pants:
                slot = pantsSlot;
                break;
            case (int)ItemType.Foot:
                slot = footSlot;
                break;
            case (int)ItemType.Weapon:
                slot = righthandSlot;
                break;
        }
        if (slot == null) return null;
        if (slot.isEmpty) return null;
        var item = slot.data;
        slot.ClearItem();
        return item;
    }

    public ItemInfo Disrobe(InventorySlot slot)
    {
        var item = slot.data;
        slot.ClearItem();
        return item;
    }

    public void SetItem(ItemInfo item)
    {
        var slot = FindTypeSlot(item);
        if (slot == null)
        {
            Debug.LogWarning("아이템 타입이 잘못되었습니다");
            return;
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
    private void InitializeSlot(InventorySlot slot, int itemType)
    {
        slot.onPointerEnterAction += ShowItemInfoPanel;
        slot.onPointerExitAction += HideItemInfoPanel;
        slot.onRightClickAction += OnRightClickHandler;
        slot.onBeginDragAction += OnDragBeginHandler;
        slot.onDragAction += OnDragHandler;
        slot.onEndDragAction += OnDragEndHandler;
        slot.Init(itemType, InventorySlot.SlotType.EQUIPMENT);
    }
    private InventorySlot FindItemSlot(int itemId)
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
    private InventorySlot FindItemSlot(ItemInfo item)
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

    private InventorySlot FindTypeSlot(ItemInfo item)
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
        // alt+우클릭 하면 창고로 이동
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            C_MoveItemRequest(slot, -1, (int)InventorySlot.SlotType.STORAGE);
            return;
        }
        // 그냥 우클릭 하면 소지품으로 이동
        C_MoveItemRequest(slot, -1, (int)InventorySlot.SlotType.INVENTORY);
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
                switch (other.type)
                {
                    case InventorySlot.SlotType.INVENTORY: // 인벤토리에 놓으면
                        C_MoveItemRequest(slot, other.index, (int)InventorySlot.SlotType.INVENTORY);
                        break;
                    case InventorySlot.SlotType.STORAGE: // 창고에 놓으면
                        C_MoveItemRequest(slot, other.index, (int)InventorySlot.SlotType.STORAGE);
                        break;
                }
            }
        }
        return;
    }

    private void S_MoveItemHandler(S_MoveItemResponse data)
    {
        // 아이템을 가지고 있는지 확인
        var slot = FindItemSlot(data.ItemId);
        if (slot == null) return;

        // equipment -> inventory
        if (data.Storage == (int)InventorySlot.SlotType.INVENTORY)
        {
            var disrobed = Disrobe(slot);
            inventoryContainer.AddItem(disrobed, data.Position);
        }
        // equipment -> storage
        else if (data.Storage == (int)InventorySlot.SlotType.STORAGE)
        {
            var disrobed = Disrobe(slot);
            storageContainer.AddItem(disrobed, data.Position);
        }
    }

    private void C_MoveItemRequest(InventorySlot slot, int position, int storage)
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
