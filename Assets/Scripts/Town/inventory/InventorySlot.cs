using Google.Protobuf.Protocol;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemImage;
    public UnityAction<ItemInfo> onPointerEnterAction;
    public UnityAction onPointerExitAction;

    private ItemInfo _data;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitSlot(ItemInfo data)
    {
        // 슬롯 데이터 초기화
        _data = data;
        // 슬롯에 아이콘 갱신
        itemImage.sprite = null;
        itemImage.color = new UnityEngine.Color(1, 1, 1, 1);
    }

    public void ClearSlot()
    {
        // 슬롯 데이터 삭제
        _data = null;
        itemImage.sprite = null;
        itemImage.color = new UnityEngine.Color(1, 1, 1, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_data != null)
            onPointerEnterAction?.Invoke(_data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExitAction?.Invoke();
    }
}
