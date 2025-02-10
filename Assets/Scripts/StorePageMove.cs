using UnityEngine;

public class PageManager : MonoBehaviour
{
    public GameObject[] consumablePages; // 소모품 페이지들
    public GameObject[] equipmentPages;  // 장비 페이지들
    private GameObject[] currentPages;   // 현재 활성화된 페이지 리스트
    private int currentPageIndex = 0;

    private void Start()
    {
        // 기본적으로 소모품 페이지 활성화
        currentPages = consumablePages;
        UpdatePageVisibility();
    }

    public void ShowNextPage()
    {
        if (currentPageIndex < currentPages.Length - 1)
        {
            currentPageIndex++;
            UpdatePageVisibility();
        }
    }

    public void ShowPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageVisibility();
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

    private void UpdatePageVisibility()
    {
        // 모든 페이지 비활성화 후 현재 페이지 활성화
        DisableAllPages(consumablePages);
        DisableAllPages(equipmentPages);

        if (currentPages.Length > 0)
        {
            currentPages[currentPageIndex].SetActive(true);
        }
    }

    private void DisableAllPages(GameObject[] pages)
    {
        foreach (var page in pages)
        {
            page.SetActive(false);
        }
    }
}
