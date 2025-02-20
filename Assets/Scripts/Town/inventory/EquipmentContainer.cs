using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentContainer : MonoBehaviour
{
    public InventoryContainer inventory;
    public Button btn_close;
    public CanvasGroup canvasGroup;
    public ItemInfoPanel itemInfoPanel;

    public InventorySlot headSlot, shirtSlot, pantsSlot, righthandSlot, lefthandSlot, footSlot;
    private bool isShowing = false;

    // Start is called before the first frame update
    void Start()
    {
        btn_close.onClick.AddListener(Hide);
        InitializeSlot(headSlot);
        InitializeSlot(shirtSlot);
        InitializeSlot(pantsSlot);
        InitializeSlot(righthandSlot);
        InitializeSlot(lefthandSlot);
        InitializeSlot(footSlot);
        PacketHandler.S_DisrobeItemEvent += S_DisrobeItemResponseHandler;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitializeSlot(InventorySlot slot)
    {
        slot.onPointerEnterAction += ShowItemInfoPanel;
        slot.onPointerExitAction += HideItemInfoPanel;
        slot.onRightClickAction += OnRightClickHandler;
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

    public void Equip(ItemInfo item)
    {
        switch (item.ItemType)
        {
            case 2:
                // 머리
                if (!headSlot.isEmpty)
                {
                    Unequip(headSlot);
                }
                headSlot.Init(item);
                break;
            case 3:
                // 상의
                if (!shirtSlot.isEmpty)
                {
                    Unequip(shirtSlot);
                }
                shirtSlot.Init(item);
                break;
            case 4:
                // 하의
                if (!pantsSlot.isEmpty)
                {
                    Unequip(pantsSlot);
                }
                pantsSlot.Init(item);
                break;
            case 5:
                // 신발
                if (!footSlot.isEmpty)
                {
                    Unequip(footSlot);
                }
                footSlot.Init(item);
                break;
            case 6:
                // 무기
                if (!righthandSlot.isEmpty)
                {
                    Unequip(righthandSlot);
                }
                righthandSlot.Init(item);
                break;
            default:
                break;
        }
    }

    private void OnRightClickHandler(InventorySlot slot)
    {
        if (slot.isEmpty) return;
        C_DisrobeItemRequest disrobeRequest = new C_DisrobeItemRequest
        {
            ItemId = slot.data.Id,
        };
        GameManager.Network.Send(disrobeRequest);
    }

    private void S_DisrobeItemResponseHandler(S_DisrobeItemResponse data)
    {
        if (data.Success)
        {
            var slot = FindItemSlot(data.ItemId);
            if (slot == null)
            {
                Debug.LogError("slot not found");
                return;
            }
            Unequip(slot);
        }
    }

    public void Unequip(InventorySlot slot)
    {
        var item = slot.data;
        inventory.AddItem(item);
        slot.Clear();
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
