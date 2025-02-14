using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPartyPopUp : MonoBehaviour
{
    [SerializeField] private TMP_InputField partyNameInputField;
    [SerializeField] private TMP_InputField partySearchInputField;
    [SerializeField] private TMP_InputField partyInviteInputField;

    void Update()
    {

    }

    public void PartySearchBtnClick()
    {
        PartySearch();
    }

    public void PartyCreateBtnClick()
    {
        PartyCreateRequest();
    }

    public void PartyBtnClick()
    {
        RequestPartyList();
    }

    public void PartyInviteBtnClick()
    {
        PartyInviteRequest();
    }

    public void PartyExitBtnClick()
    {
        PartyExitRequest();
    }

    private void PartyExitRequest()
    {
        C_PartyExitRequest partyExitPacket = new C_PartyExitRequest { UserId = TownManager.Instance.MyPlayer.PlayerId };
        GameManager.Network.Send(partyExitPacket);
    }

    private void PartySearch()
    {
        C_SearchPartyRequest partySearchPacket = new C_SearchPartyRequest { PartyName = partySearchInputField.text };
        GameManager.Network.Send(partySearchPacket);
    }

    private void RequestPartyList()
    {
        C_PartyListRequest partyListPacket = new C_PartyListRequest { };
        GameManager.Network.Send(partyListPacket);
    }

    private void PartyCreateRequest()
    {
        C_PartyRequest partyRequestPacket = new C_PartyRequest { UserId = TownManager.Instance.MyPlayer.PlayerId, PartyName = partyNameInputField.text };
        GameManager.Network.Send(partyRequestPacket);
    }

    private void PartyInviteRequest()
    {
        C_PartyInviteRequest partyInviteRequestPacket = new C_PartyInviteRequest { RequesterUserNickname = TownManager.Instance.MyPlayer.nickname, ParticipaterUserNickname = partyInviteInputField.text };
        GameManager.Network.Send(partyInviteRequestPacket);
    }
}