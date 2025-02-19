using Google.Protobuf.Protocol;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    public TMPro.TextMeshProUGUI text_itemName;
    public TMPro.TextMeshProUGUI text_itemType;
    public Image img_ItemImage;
    public TMPro.TextMeshProUGUI text_description;
    public TMPro.TextMeshProUGUI text_manual;

    public RectTransform parentTransform;
    private bool isShowing = false;

    private void Update()
    {
        if (isShowing)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, Input.mousePosition, null, out Vector2 localPoint);
            rectTransform.localPosition = localPoint;
        }
    }

    public void Init(ItemInfo info)
    {
        text_itemName.text = info.Name;
        string newString = $"{info.Rarity}\n{info.ItemType}"; // TODO: 희귀도랑 아이템 타입 매핑 필요
        // TODO : 아이템 이미지 매핑
        text_itemType.text = newString;
    }

    public void Toggle()
    {
        if (isShowing)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        isShowing = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        isShowing = false;
    }
}
