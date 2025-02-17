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
                int parsedId;
                if (int.TryParse(partyIdText.text, out parsedId))
                {
                    UIPartyPopUp popup = FindObjectOfType<UIPartyPopUp>();
                    if (popup != null)
                    {
                        popup.selectPartyId = parsedId;
                        Debug.Log("Selected party id set to: " + parsedId);
                    }
                    else
                    {
                        Debug.LogWarning("UIPartyPopUp �ν��Ͻ��� ã�� �� �����ϴ�.");
                    }
                }
                else
                {
                    Debug.LogWarning("PartyId �ؽ�Ʈ�� int�� ��ȯ�� �� �����ϴ�: " + partyIdText.text);
                }
            }
            else
            {
                Debug.LogWarning("�ڽ� ������Ʈ 'PartyId'�� ã�� �� �����ϴ�.");
            }
        }
    }
}
