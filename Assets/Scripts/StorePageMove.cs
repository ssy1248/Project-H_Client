using UnityEngine;

public class PageManager : MonoBehaviour
{
    public GameObject[] consumablePages; // �Ҹ�ǰ ��������
    public GameObject[] equipmentPages;  // ��� ��������
    private GameObject[] currentPages;   // ���� Ȱ��ȭ�� ������ ����Ʈ
    private int currentPageIndex = 0;

    private void Start()
    {
        // �⺻������ �Ҹ�ǰ ������ Ȱ��ȭ
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
        // ��� ������ ��Ȱ��ȭ �� ���� ������ Ȱ��ȭ
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
