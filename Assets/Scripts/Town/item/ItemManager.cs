using Google.Protobuf.Protocol;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Consumable = 0,
    NotConsumable = 1,
    Head = 2,
    Shirt = 3,
    Pants = 4,
    Foot = 5,
    Weapon = 6,
}

public class ItemManager : MonoBehaviour
{
    static ItemManager _instance;
    public static ItemManager instance => _instance;

    [SerializeField] Dictionary<int, ItemData> itemData = new Dictionary<int, ItemData>();
    private Dictionary<string, Sprite> itemImages = new Dictionary<string, Sprite>();
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
        LoadAssets();
        InitializeItemHandler();
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

    public Sprite GetItemImg(int id)
    {
        var route = itemData[id].Imgsrc;
        if (!itemImages.ContainsKey(route)) return null;
        Sprite itemSprite = itemImages[route];
        return itemSprite;
    }

    public Sprite GetItemImg(string imgsrc)
    {
        if (!itemImages.ContainsKey(imgsrc)) return null;
        Sprite itemSprite = itemImages[imgsrc];
        return itemSprite;
    }

    private void LoadAssets()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Items");
        foreach (var sprite in sprites)
        {
            string fileName = sprite.texture.name;
            itemImages.Add(fileName, sprite);
        }
    }

    public void ActiveItemHandler(int itemId, S_ActiveItemResponse data)
    {
        Debug.Log("ActiveItemHandler");
        var success = data.Success;

        if (success)
        {
            // 적절한 아이템 사용
            var handler = ItemHandlers[itemId];
            handler(data);
        }
    }

    private delegate void itemHandler(S_ActiveItemResponse data);
    private Dictionary<int, itemHandler> ItemHandlers;
    private void InitializeItemHandler()
    {
        ItemHandlers = new Dictionary<int, itemHandler>{
            {3, HealthPotion(100)},
        };
    }

    private itemHandler HealthPotion(int amount)
    {
        return (S_ActiveItemResponse data) =>
        {
            Debug.Log($"{data.UserId} : use item({data.Id})");
            Debug.Log($"hp potion({amount})");
        };
    }
}
