using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;

public class PageNation : MonoBehaviour
{
    public PageManager pageManager;
    public GameObject slotPrefab;      // 슬롯 프리팹
    public GameObject pagePrefab;      // 페이지 프리팹
    public Transform pageContainer;    // 페이지를 담을 부모 오브젝트
    public int slotsPerPage = 6;       // 한 페이지당 슬롯 개수

    private List<GameObject> pages = new List<GameObject>();
    private int currentPageIndex = 0;

    public TextMeshProUGUI pageNumberText;

    private void Start()
    {
        // 첫 번째 페이지 생성
        CreateNewPage();
        UpdatePageVisibility();

        // 같은 오브젝트에 PageManager가 있을 경우
        if (pageManager == null)
        {
            pageManager = GetComponent<PageManager>();
        }

        if (pageManager == null)
        {
            Debug.LogError("PageManager not found!");
        }
    }

    public void AddSlotToConsumablePage()
    {
        // pageManager가 null인 경우 체크
        if (pageManager == null)
        {
            Debug.LogError("PageManager is not assigned!");
            return; // pageManager가 없으면 함수를 종료하여 추가 작업을 하지 않음
        }

        // 현재 페이지가 가득 찼는지 확인
        GameObject currentPage = pages[currentPageIndex];

        int currentSlotCount = currentPage.transform.childCount;

        // 한 페이지에 슬롯이 6개 이상이면 새로운 페이지 생성
        if (currentSlotCount >= slotsPerPage)
        {
            CreateNewPage();
            currentPageIndex++;
            currentPage = pages[currentPageIndex]; // 새로운 페이지로 할당
        }

        // 현재 페이지에 슬롯 추가
        GameObject newSlot = Instantiate(slotPrefab, currentPage.transform);
        newSlot.SetActive(true);

        UpdatePageVisibility();

    }


    private void CreateNewPage()
    {
        GameObject newPage = Instantiate(pagePrefab, pageContainer);
        newPage.SetActive(false);  // 🌟 처음엔 비활성화 🌟
        pages.Add(newPage);
    }

    private void UpdatePageVisibility()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == currentPageIndex);
        }

        UpdatePageNumberText();
    }

    private void UpdatePageNumberText()
    {
        if (pageNumberText != null)
        {
            pageNumberText.text = $"{currentPageIndex + 1} / {pages.Count}";
        }
    }
}
