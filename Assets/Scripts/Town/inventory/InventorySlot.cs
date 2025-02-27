using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum SlotType
    {
        NONE = 0,
        EQUIPMENT = 1,
        INVENTORY = 2,
        WAREHOUSE = 3,
    }
    public Image itemImage;
    public TMPro.TextMeshProUGUI quantityText;
    public UnityAction<PointerEventData, InventorySlot> onPointerEnterAction, onRightClickAction, onPointerUpAction, onBeginDragAction, onDragAction, onEndDragAction;
    public UnityAction<PointerEventData> onPointerExitAction;
    protected SlotType _type;
    public SlotType type
    {
        get { return _type; }
    }
    protected int _index;
    public int index
    {
        get { return _index; }
    }
    public bool isEmpty
    {
        get
        {
            if (_data == null) return true;
            return false;
        }
    }
    private ItemInfo _data;
    public ItemInfo data
    {
        get { return _data; }
    }

    public void Init(int index, SlotType type)
    {
        _index = index;
        _type = type;
    }

    public void ClearItem()
    {
        // 슬롯 데이터 삭제
        _data = null;
        itemImage.sprite = null;
        itemImage.color = new UnityEngine.Color(1, 1, 1, 0);
    }

    public void SetItem(ItemInfo data)
    {
        // 슬롯 데이터 초기화
        _data = data;
        _data.Position = index;
        if(!_data.Stackable){
            quantityText.text = "";
        }else{
            quantityText.text = _data.Quantity.ToString();
        }
        // 슬롯에 아이콘 갱신
        itemImage.sprite = ItemManager.instance.GetItemImg(_data.Imgsrc);
        itemImage.color = new UnityEngine.Color(1, 1, 1, 1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_data != null)
            onPointerEnterAction?.Invoke(eventData, this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExitAction?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 우클릭
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 부모 클래스에 맡겨버리기
            onRightClickAction?.Invoke(eventData, this);
            return;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && eventData.dragging)
        {
            onPointerUpAction?.Invoke(eventData, this);
            return;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onBeginDragAction?.Invoke(eventData, this);
            return;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onDragAction?.Invoke(eventData, this);
            return;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onEndDragAction?.Invoke(eventData, this);
        }
    }
}
