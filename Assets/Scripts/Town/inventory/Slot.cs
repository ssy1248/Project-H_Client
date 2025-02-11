using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] int id = 0;
    [SerializeField] bool isEquip = false;
    ItemInfo itemData;

    void InitSlot(ItemInfo data)
    {
        itemData = data;
        ToggleEquipEffect();
        gameObject.GetComponent<Button>().onClick.AddListener(ToggleEquip);
        gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
    }
    void ToggleEquip()
    {
        if (!isEquip)
        {
            TownManager.Instance.EquipItemRequest(itemData.ItemId);
        }
        else
        {
            TownManager.Instance.DisrobeItemRequest(itemData.ItemId);
        }
    }

    void ToggleEquipEffect()
    {
        if (isEquip)
        {
            gameObject.GetComponent<Image>().color = UnityEngine.Color.gray;
        }
        else
        {
            gameObject.GetComponent<Image>().color = UnityEngine.Color.white;
        }     
    }
}
