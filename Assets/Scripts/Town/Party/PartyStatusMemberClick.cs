using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PartyStatusMemberClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public GameObject contextMenu;       // 우클릭 시 열릴 컨텍스트 메뉴
    private bool isContextMenuOpen = false;

    void Update()
    {
        // 컨텍스트 메뉴가 열려 있는 상태에서, 마우스 클릭(좌/우)하면 닫기
        if (isContextMenuOpen && !gameObject.CompareTag("Button"))
        {
            // 왼쪽 버튼 클릭(0) 또는 오른쪽 버튼 클릭(1)
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                // 실제로 클릭된 오브젝트가 "Button" 태그인지 확인
                if (!IsPointerOverUIWithTag("Button"))
                {
                    CloseContextMenu();
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 태그가 "PartyMember"인지 확인
        if (CompareTag("PartyMember"))
        {
            // 우클릭일 때만 메뉴 열기
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
        // 클릭한 오브젝트의 자식에 있는 TextMeshProUGUI 컴포넌트를 찾아 텍스트를 Nickname에 저장
        TextMeshProUGUI textComponent = GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            party.MemberClickNickname = textComponent.text;
        }
        else
        {
            Debug.LogWarning("자식에 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
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
            // 만약 클릭된 UI 중 하나라도 지정 태그를 갖고 있다면 true
            if (result.gameObject.CompareTag(targetTag))
                return true;
        }

        return false;
    }
}
