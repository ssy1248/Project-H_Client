using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DropdownMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Button headerButton;        // 드롭다운을 여닫는 버튼
    public RectTransform scrollView;   // 확장될 스크롤 뷰
    public RectTransform content;      // 아이템들이 들어갈 Content
    public CanvasGroup canvasGroup;    // 알파 값을 조절할 CanvasGroup
    public GameObject buttonPrefab;    // 버튼 프리팹 (여기서 데이터 기반으로 버튼을 생성)
    public float itemHeight = 50f;     // 개별 아이템 높이
    public float maxExpandedHeight = 300f; // 최대 확장 높이
    public float collapsedHeight = 10f; // 최소 높이
    public float animationSpeed = 5f;   // 애니메이션 속도
    public ScrollRect scrollRect;      // ScrollRect 컴포넌트

    private bool isExpanded = false;

    void Start()
    {
        // CanvasGroup 기본 설정 (처음엔 닫힌 상태)
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        scrollView.sizeDelta = new Vector2(scrollView.sizeDelta.x, collapsedHeight);

        // 헤더 버튼 클릭 이벤트 연결
        headerButton.onClick.AddListener(ToggleDropdown);

        // 던전 데이터에 맞게 버튼 생성
        AddDungeonButtons(new string[] { "Dungeon 1", "Dungeon 2", "Dungeon 3" });

        // 스크롤뷰 내부의 버튼 클릭 이벤트 연결
        UpdateButtonListeners();
    }

    void Update()
    {
        // 마우스 휠로 스크롤 이동
        HandleMouseWheelScroll();
    }

    void HandleMouseWheelScroll()
    {
        if (scrollRect != null)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0)
            {
                // ScrollRect의 verticalNormalizedPosition을 조정하여 스크롤을 처리
                float newPos = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + scrollInput);  // 부호 반전
                scrollRect.verticalNormalizedPosition = newPos;
            }
        }
    }

    void ToggleDropdown()
    {
        isExpanded = !isExpanded;
        StopAllCoroutines();

        // 버튼 개수에 따라 확장 높이 결정
        float totalHeight = content.childCount * itemHeight;
        float targetHeight = Mathf.Min(maxExpandedHeight, totalHeight);
        if (!isExpanded) targetHeight = collapsedHeight;

        float targetAlpha = isExpanded ? 1 : 0;
        bool interactable = isExpanded;

        StartCoroutine(AnimateDropdown(targetHeight, targetAlpha, interactable));
    }

    void CloseDropdown()
    {
        if (isExpanded)
        {
            isExpanded = false;
            StopAllCoroutines();
            StartCoroutine(AnimateDropdown(collapsedHeight, 0, false));
        }
    }

    IEnumerator AnimateDropdown(float targetHeight, float targetAlpha, bool interactable)
    {
        float currentHeight = scrollView.sizeDelta.y;
        float currentAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * animationSpeed;
            float newHeight = Mathf.Lerp(currentHeight, targetHeight, elapsedTime);
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, elapsedTime);

            scrollView.sizeDelta = new Vector2(scrollView.sizeDelta.x, newHeight);
            canvasGroup.alpha = newAlpha;
            yield return null;
        }

        scrollView.sizeDelta = new Vector2(scrollView.sizeDelta.x, targetHeight);
        canvasGroup.alpha = targetAlpha;
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }

    // 던전 버튼들을 추가하는 함수
    public void AddDungeonButtons(string[] dungeonNames)
    {
        foreach (var dungeon in dungeonNames)
        {
            // 버튼 생성
            GameObject button = Instantiate(buttonPrefab, content);
            button.SetActive(true);  // 프리팹을 활성화
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();  // 버튼의 텍스트 컴포넌트
            buttonText.text = dungeon;  // 텍스트 설정

            Button btnComponent = button.GetComponent<Button>();
            btnComponent.onClick.AddListener(() => OnDungeonSelected(dungeon));  // 던전 선택 시 이벤트
        }

        UpdateButtonListeners();
    }

    // 던전 선택 시 호출되는 함수 (여기서는 던전 선택 후 특별한 행동은 없지만 필요시 추가 가능)
    void OnDungeonSelected(string dungeonName)
    {
        Debug.Log($"Selected Dungeon: {dungeonName}");
        // 던전 선택 후 로직을 추가하세요 (예: 던전 입장)
    }

    // ⚡ 버튼 개수가 바뀌면 자동으로 업데이트하는 함수
    public void UpdateButtonListeners()
    {
        Button[] itemButtons = content.GetComponentsInChildren<Button>();
        foreach (Button btn in itemButtons)
        {
            btn.onClick.AddListener(CloseDropdown);
        }
    }
}