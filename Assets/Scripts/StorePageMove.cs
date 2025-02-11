using UnityEngine;
using TMPro; // TextMeshPro ���� ���ӽ����̽� �߰�

public class PageManager : MonoBehaviour
{
    public GameObject[] consumablePages; // �Ҹ�ǰ ��������
    public GameObject[] equipmentPages;  // ��� ��������
    public GameObject[] sellPages;  // ��� ��������
    private GameObject[] currentPages;   // ���� Ȱ��ȭ�� ������ ����Ʈ
    private int currentPageIndex = 0;

    public TextMeshProUGUI pageNumberText; // ���� ������ ��ȣ�� ǥ���� TextMeshProUGUI UI

    private void Start()
    {
        // �⺻������ �Ҹ�ǰ ������ Ȱ��ȭ
        currentPages = consumablePages;
        UpdatePageVisibility();
    }

    public void ShowNextPage()
    {
        Debug.Log("Next Page Button Clicked!"); // ��ư Ŭ�� �� ��µ� �α�
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
        // ��� ������ ��Ȱ��ȭ �� ���� ������ Ȱ��ȭ
        DisableAllPages(consumablePages);
        DisableAllPages(equipmentPages);
        DisableAllPages(sellPages);

        if (currentPages.Length > 0 && currentPages[currentPageIndex] != null)
        {
            currentPages[currentPageIndex].SetActive(true);
        }

        // ������ ��ȣ ������Ʈ
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
        // ���� ������ ��ȣ�� �ؽ�Ʈ�� ǥ�� (TextMeshPro ���)
        if (pageNumberText != null)
        {
            pageNumberText.text = $"{currentPageIndex + 1} / {currentPages.Length}";
        }
    }
}
