using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : InventorySlot
{
    // ItemType
        // 0: 소모성 아이템
        // 1: 비소모성 아이템
        // 2: 머리
        // 3: 상의
        // 4: 하의
        // 5: 신발
        // 6: 무기
    private int _itemType;
    public int itemType
    {
        get { return _itemType; }
    }

    public void Init(int index, SlotType slotType, int itemType)
    {
        _index = index;
        _type = slotType;
        _itemType = itemType;
    }
}
