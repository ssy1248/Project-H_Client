using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    static ItemManager _instance;
    public static ItemManager instance => _instance;

    [SerializeField] Dictionary<int,ItemData> itemData = new Dictionary<int,ItemData> ();
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void SetData(List<ItemData> data)
    {
        foreach (ItemData item in data)
        {
            itemData.Add(item.Id, item);
        }
    }
    public ItemData GetBuyId(int id)
    {
        return itemData[id];
    }

}
