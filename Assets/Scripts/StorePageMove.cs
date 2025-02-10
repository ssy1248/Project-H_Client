using UnityEngine;

public class PageManager : MonoBehaviour
{
    public GameObject[] pages;
    private int currentPageIndex = 0;

    void Start()
    {
        UpdatePageVisibility();
    }

    public void ShowNextPage()
    {
        if (currentPageIndex < pages.Length - 1)
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

    private void UpdatePageVisibility()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == currentPageIndex);
        }
    }
}
