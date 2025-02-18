// 파티 슬롯 스크립트
using TMPro;
using UnityEngine;

public class PartySlot : MonoBehaviour
{
    public TMP_Text dungeonNameText;
    public TMP_Text partyNameText;
    public TMP_Text memberCountText;

    private Party currentParty;

    public void SetParty(Party party)
    {
        currentParty = party;
        dungeonNameText.text = party.dungeonName;
        partyNameText.text = party.partyName;
        memberCountText.text = $"{party.memberCount} / 4";
    }
}