using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PartyInfoClickHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("클릭이벤트 실행");
        // 태그가 "PartyInfo"인지 확인
        if (gameObject.CompareTag("PartyInfo"))
        {
            // 모든 자식(TextMeshProUGUI 컴포넌트 포함, 비활성화된 것도 포함)
            TextMeshProUGUI[] texts = gameObject.GetComponentsInChildren<TextMeshProUGUI>(true);
            TextMeshProUGUI partyIdText = null;

            // "PartyId"라는 이름의 TextMeshProUGUI를 찾는다
            foreach (var txt in texts)
            {
                if (txt.gameObject.name.Equals("PartyId"))
                {
                    partyIdText = txt;
                    break;
                }
            }

            if (partyIdText != null)
            {
                // 문자열 형태로 PartyId를 가져온다
                string partyIdString = partyIdText.text;

                // UIPartyPopUp 찾아서 selectPartyId(문자열) 할당
                UIPartyPopUp popup = FindObjectOfType<UIPartyPopUp>();
                if (popup != null)
                {
                    popup.selectPartyId = partyIdString;
                    Debug.Log("Selected party id set to: " + partyIdString);
                }
                else
                {
                    Debug.LogWarning("UIPartyPopUp 인스턴스를 찾을 수 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("자식 오브젝트 'PartyId'를 찾을 수 없습니다.");
            }
        }
    }
}
