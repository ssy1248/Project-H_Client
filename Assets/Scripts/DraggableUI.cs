using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform panelRectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalLocalPointerPosition;
    private Vector2 originalPanelLocalPosition;

    // 드래그할 패널의 RectTransform을 외부에서 할당할 수 있도록 변경
    public RectTransform draggablePanelRectTransform;

    void Start()
    {
        panelRectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (draggablePanelRectTransform == null)
        {
            // draggablePanelRectTransform이 할당되지 않았다면 부모로 설정
            draggablePanelRectTransform = panelRectTransform.parent.GetComponent<RectTransform>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 패널이 다른 UI 요소 위에 표시되도록 설정
        canvasGroup.blocksRaycasts = false;

        // 마우스와 패널의 상대적인 위치 저장
        RectTransformUtility.ScreenPointToLocalPointInRectangle(draggablePanelRectTransform, eventData.position, canvas.worldCamera, out originalLocalPointerPosition);
        originalPanelLocalPosition = draggablePanelRectTransform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 마우스 포인터와 패널의 위치를 계산하여, 상대적 위치로 패널을 이동
        Vector2 currentLocalPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(draggablePanelRectTransform, eventData.position, canvas.worldCamera, out currentLocalPointerPosition);

        // 패널의 새로운 위치 계산
        Vector2 offset = currentLocalPointerPosition - originalLocalPointerPosition;
        draggablePanelRectTransform.localPosition = originalPanelLocalPosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료 시 raycast 차단 복원
        canvasGroup.blocksRaycasts = true;
    }
}