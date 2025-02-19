using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image itemImage;
    public UnityAction<InventorySlot> onPointerEnterAction, onRightClickAction;
    public UnityAction onPointerExitAction;

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

    public void Init(ItemInfo data)
    {
        // 슬롯 데이터 초기화
        _data = data;
        // 슬롯에 아이콘 갱신
        itemImage.sprite = null;
        itemImage.color = new UnityEngine.Color(1, 1, 1, 1);
    }

    public void Clear()
    {
        // 슬롯 데이터 삭제
        _data = null;
        itemImage.sprite = null;
        itemImage.color = new UnityEngine.Color(1, 1, 1, 0);
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
            onPointerEnterAction?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExitAction?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 부모 클래스에 맡겨버리기
            onRightClickAction?.Invoke(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
