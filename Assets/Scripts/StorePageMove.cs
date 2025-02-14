using UnityEngine;
using TMPro; // TextMeshPro 관련 네임스페이스 추가

public class PageManager : MonoBehaviour
{
    public GameObject[] consumablePages; // 소모품 페이지들
    public GameObject[] equipmentPages;  // 장비 페이지들
    public GameObject[] sellPages;  // 장비 페이지들
    private GameObject[] currentPages;   // 현재 활성화된 페이지 리스트
    private int currentPageIndex = 0;

    public TextMeshProUGUI pageNumberText; // 현재 페이지 번호를 표시할 TextMeshProUGUI UI

    private void Start()
    {
        // 기본적으로 소모품 페이지 활성화
        currentPages = consumablePages;
        UpdatePageVisibility();
    }

    public void ShowNextPage()
    {
        Debug.Log("Next Page Button Clicked!"); // 버튼 클릭 시 출력될 로그
        if (currentPageIndex < currentPages.Length - 1)
        {
            currentPageIndex++;
            UpdatePageVisibility();
            Debug.Log("Showing next page: " + (currentPageIndex + 1));
        }
        else
        {
            Debug.Log("No more pages.");
        }
    }

    public void ShowPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageVisibility();
            Debug.Log("Showing previous page: " + (currentPageIndex + 1));
        }
        else
        {
            Debug.Log("Already at the first page.");
        }
    }

    public void SwitchToConsumablePages()
    {
        currentPages = consumablePages;
        currentPageIndex = 0;
        UpdatePageVisibility();
    }

    public void SwitchToEquipmentPages()
    {
        currentPages = equipmentPages;
        currentPageIndex = 0;
        UpdatePageVisibility();
    }
    public void SwitchToSellPages()
    {
        currentPages = sellPages;
        currentPageIndex = 0;
        UpdatePageVisibility();
    }

    private void UpdatePageVisibility()
    {
        // 모든 페이지 비활성화 후 현재 페이지 활성화
        DisableAllPages(consumablePages);
        DisableAllPages(equipmentPages);
        DisableAllPages(sellPages);

        if (currentPages.Length > 0 && currentPages[currentPageIndex] != null)
        {
            currentPages[currentPageIndex].SetActive(true);
        }

        // 페이지 번호 업데이트
        UpdatePageNumberText();
    }

    private void DisableAllPages(GameObject[] pages)
    {
        foreach (var page in pages)
        {
            if (page != null)
            {
                page.SetActive(false);
            }
        }
    }

    private void UpdatePageNumberText()
    {
        // 현재 페이지 번호를 텍스트로 표시 (TextMeshPro 사용)
        if (pageNumberText != null)
        {
            pageNumberText.text = $"{currentPageIndex + 1} / {currentPages.Length}";
        }
    }
}
