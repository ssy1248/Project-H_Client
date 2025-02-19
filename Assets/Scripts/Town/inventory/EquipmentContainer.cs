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
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitializeSlot(InventorySlot slot)
    {
        slot.onPointerEnterAction += ShowItemInfoPanel;
        slot.onPointerExitAction += HideItemInfoPanel;
        slot.onRightClickAction += Unequip;
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

    public void Unequip(InventorySlot slot)
    {
        var item = slot.data;
        slot.Clear();
        inventory.AddItem(item);
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
