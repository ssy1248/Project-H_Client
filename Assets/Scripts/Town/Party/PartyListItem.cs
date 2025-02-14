using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyListItem : MonoBehaviour
{
    // ��Ƽ ������ ������ ���� (�������� ���� PartyInfo)
    public PartyInfo partyData;

    // ��ư ������Ʈ (�� ������Ʈ�� �پ��ְų� �ڽĿ� ���� �� ����)
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
        {
            _button.onClick.AddListener(OnPartyListItemClicked);
        }
    }

    // ��ư Ŭ�� �� ȣ��� �޼���
    private void OnPartyListItemClicked()
    {
        // TownManager�� ���ǵ� ��Ƽ�� UI ������Ʈ �Լ��� ȣ���մϴ�.
        // ��: �ش� ��Ƽ�� ������ PartyMemberSpawnPoint�� UI�� ǥ��.
        TownManager.Instance.UpdatePartyMembersUI(partyData);
    }

    // ���� ���
    public void ParticipateBtnClick()
    {
        ParticipateRequest();
    }

    private void ParticipateRequest()
    {
        C_PartyJoinRequest partyJoinPacket = new C_PartyJoinRequest { PartyId = partyData.PartyId , UserId = TownManager.Instance.MyPlayer.PlayerId};
        GameManager.Network.Send(partyJoinPacket);
    }
}
