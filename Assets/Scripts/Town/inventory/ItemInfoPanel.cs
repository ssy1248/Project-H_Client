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

    private RectTransform canvasRect;
    private bool isShowing = false;

    private void Awake()
    {
        Canvas canvas = gameObject.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("캔버스를 찾을 수 없습니다");
        }
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, Camera.main, out Vector2 localPoint);
        rectTransform.localPosition = localPoint;
    }

    public void Init(ItemInfo info)
    {
        text_itemName.text = info.Name;
        string newString = $"{info.Rarity}\n${info.ItemType}"; // TODO: 희귀도랑 아이템 타입 매핑 필요
        text_itemType.text = newString;
    }

    public void Toggle()
    {
        if (isShowing)
        {
            Hide();
            isShowing = false;
        }
        else
        {
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
}
