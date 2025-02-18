using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image itemImage;

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
}
