using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image itemImage;
    public UnityAction<PointerEventData, InventorySlot> onPointerEnterAction, onRightClickAction, onPointerUpAction, onBeginDragAction, onDragAction, onEndDragAction;
    public UnityAction<PointerEventData> onPointerExitAction;
    private int _index;
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(int index)
    {
        _index = index;
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
        // 슬롯에 아이콘 갱신
        itemImage.sprite = null;
        itemImage.color = new UnityEngine.Color(1, 1, 1, 1);
    }

    private void Equip(ItemInfo item)
    {
        // unequip

        // equip
        item.Equiped = true;
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
