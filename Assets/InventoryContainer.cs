using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using SRF;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryContainer : MonoBehaviour
{
    public Transform itemSlotParent;
    public Button btn_close;
    private List<InventorySlot> itemSlots = new List<InventorySlot>();

    // Start is called before the first frame update
    void Start()
    {
        btn_close.onClick.AddListener(Btn_Close);
        foreach (Transform child in itemSlotParent)
        {
            if (child.gameObject.TryGetComponent(out InventorySlot slot))
            {
                itemSlots.Add(slot);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show()
    {

    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void UpdateInventory(S_InventoryResponse data)
    {
        // 인벤토리 갱신
        ClearItems();

        var _inventory = data.Inventory;
        // 슬롯에 아이템 정보 추가
        for(var i = 0; i < _inventory.Count; i++){
            itemSlots[i].InitSlot(_inventory[i]);
        }
    }

    private void ClearItems(){
        foreach(var slot in itemSlots){
            // 슬롯 데이터 초기화
            
        }
    }

    private void Btn_Close()
    {
        gameObject.SetActive(false);
    }
}
