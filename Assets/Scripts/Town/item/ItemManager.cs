using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
    public void SetData(List<ItemData> data)
    {
        foreach (ItemData item in data)
        {
            itemData.Add(item.Id, item);
            Debug.Log(item);
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
            Debug.Log(fileName);
        }
    }
}
