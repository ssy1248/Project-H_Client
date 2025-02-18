using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class InventoryContainer : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Transform itemSlotParent;
    public Button btn_close;
    private List<InventorySlot> itemSlots = new List<InventorySlot>();
    private bool isShowing = false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("inventory init");
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

    public void Toggle(){
        if(isShowing){
            Hide();
            isShowing = false;
        }else{
            Show();
            isShowing = true;
        }
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void UpdateInventory(S_InventoryResponse data)
    {
        Debug.Log(data.Inventory);

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
