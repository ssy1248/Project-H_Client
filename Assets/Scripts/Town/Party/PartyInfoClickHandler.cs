using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PartyInfoClickHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Ŭ���̺�Ʈ ����");
        // �±װ� "PartyInfo"���� Ȯ��
        if (gameObject.CompareTag("PartyInfo"))
        {
            // ��� �ڽ�(TextMeshProUGUI ������Ʈ ����, ��Ȱ��ȭ�� �͵� ����)
            TextMeshProUGUI[] texts = gameObject.GetComponentsInChildren<TextMeshProUGUI>(true);
            TextMeshProUGUI partyIdText = null;

            // "PartyId"��� �̸��� TextMeshProUGUI�� ã�´�
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
                // ���ڿ� ���·� PartyId�� �����´�
                string partyIdString = partyIdText.text;

                // UIPartyPopUp ã�Ƽ� selectPartyId(���ڿ�) �Ҵ�
                UIPartyPopUp popup = FindObjectOfType<UIPartyPopUp>();
                if (popup != null)
                {
                    popup.selectPartyId = partyIdString;
                    Debug.Log("Selected party id set to: " + partyIdString);
                }
                else
                {
                    Debug.LogWarning("UIPartyPopUp �ν��Ͻ��� ã�� �� �����ϴ�.");
                }
            }
            else
            {
                Debug.LogWarning("�ڽ� ������Ʈ 'PartyId'�� ã�� �� �����ϴ�.");
            }
        }
    }
}
