using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemTitle;
    public string itemDescription;
    public int itemPrice;
    public Sprite itemIcon;

    public Item(string title, string description, Sprite icon, int price)
    {
        itemTitle = title;
        itemDescription = description;
        itemIcon = icon;
        itemPrice = price;
    }
}