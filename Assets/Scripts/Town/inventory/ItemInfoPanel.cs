using Google.Protobuf.Protocol;
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

            // 화면 경계를 확인하고 위치 조정
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            Vector3 minScreenBounds = new Vector3(0, 0, 0);
            Vector3 maxScreenBounds = new Vector3(Screen.width, Screen.height, 0);

            // 좌우 경계 확인 및 조정
            if (corners[2].x > maxScreenBounds.x) // 패널의 오른쪽이 화면을 넘어가면
            {
                localPoint.x -= rectTransform.rect.width;
            }

            // 상하 경계 확인 및 조정
            if (corners[2].y > maxScreenBounds.y) // 패널의 위쪽이 화면을 넘어가면
            {
                localPoint.y -= corners[2].y - maxScreenBounds.y;
            }
            if (corners[0].y < minScreenBounds.y) // 패널의 아래쪽이 화면을 넘어가면
            {
                localPoint.y += minScreenBounds.y - corners[0].y;
            }

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
