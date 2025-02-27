using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PartyStatusMemberClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public GameObject contextMenu;       // ��Ŭ�� �� ���� ���ؽ�Ʈ �޴�
    private bool isContextMenuOpen = false;

    void Update()
    {
        // ���ؽ�Ʈ �޴��� ���� �ִ� ���¿���, ���콺 Ŭ��(��/��)�ϸ� �ݱ�
        if (isContextMenuOpen && !gameObject.CompareTag("Button"))
        {
            // ���� ��ư Ŭ��(0) �Ǵ� ������ ��ư Ŭ��(1)
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                // ������ Ŭ���� ������Ʈ�� "Button" �±����� Ȯ��
                if (!IsPointerOverUIWithTag("Button"))
                {
                    CloseContextMenu();
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // �±װ� "PartyMember"���� Ȯ��
        if (CompareTag("PartyMember"))
        {
            // ��Ŭ���� ���� �޴� ����
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                OpenContextMenu();
            }
        }
    }

    private void OpenContextMenu()
    {
        contextMenu.SetActive(true);
        isContextMenuOpen = true;
        UIPartyPopUp party = FindObjectOfType<UIPartyPopUp>();
        // Ŭ���� ������Ʈ�� �ڽĿ� �ִ� TextMeshProUGUI ������Ʈ�� ã�� �ؽ�Ʈ�� Nickname�� ����
        TextMeshProUGUI textComponent = GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            party.MemberClickNickname = textComponent.text;
        }
        else
        {
            Debug.LogWarning("�ڽĿ� TextMeshProUGUI ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    public void CloseContextMenu()
    {
        contextMenu.SetActive(false);
        isContextMenuOpen = false;
    }

    private bool IsPointerOverUIWithTag(string targetTag)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            // ���� Ŭ���� UI �� �ϳ��� ���� �±׸� ���� �ִٸ� true
            if (result.gameObject.CompareTag(targetTag))
                return true;
        }

        return false;
    }
}
