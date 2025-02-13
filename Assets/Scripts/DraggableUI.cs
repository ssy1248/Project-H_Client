using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform panelRectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalLocalPointerPosition;
    private Vector2 originalPanelLocalPosition;

    // �巡���� �г��� RectTransform�� �ܺο��� �Ҵ��� �� �ֵ��� ����
    public RectTransform draggablePanelRectTransform;

    void Start()
    {
        panelRectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (draggablePanelRectTransform == null)
        {
            // draggablePanelRectTransform�� �Ҵ���� �ʾҴٸ� �θ�� ����
            draggablePanelRectTransform = panelRectTransform.parent.GetComponent<RectTransform>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // �巡�� ���� �� �г��� �ٸ� UI ��� ���� ǥ�õǵ��� ����
        canvasGroup.blocksRaycasts = false;

        // ���콺�� �г��� ������� ��ġ ����
        RectTransformUtility.ScreenPointToLocalPointInRectangle(draggablePanelRectTransform, eventData.position, canvas.worldCamera, out originalLocalPointerPosition);
        originalPanelLocalPosition = draggablePanelRectTransform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ���콺 �����Ϳ� �г��� ��ġ�� ����Ͽ�, ����� ��ġ�� �г��� �̵�
        Vector2 currentLocalPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(draggablePanelRectTransform, eventData.position, canvas.worldCamera, out currentLocalPointerPosition);

        // �г��� ���ο� ��ġ ���
        Vector2 offset = currentLocalPointerPosition - originalLocalPointerPosition;
        draggablePanelRectTransform.localPosition = originalPanelLocalPosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�� ���� �� raycast ���� ����
        canvasGroup.blocksRaycasts = true;
    }
}