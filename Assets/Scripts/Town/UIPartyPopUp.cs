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
    [SerializeField] private TMP_InputField partyKickInputField;
    [SerializeField] public int selectPartyId;

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

    public void PartyKickBtnClick()
    {
        PartyKickRequest();
    }

    public void ParticipateBtnClick()
    {
        ParticipateRequest();
    }

    private void ParticipateRequest()
    {
        C_PartyJoinRequest partyJoinPacket = new C_PartyJoinRequest { PartyId = selectPartyId, UserId = TownManager.Instance.MyPlayer.PlayerId };
        GameManager.Network.Send(partyJoinPacket);
    }

    private void PartyKickRequest()
    {
        int kickUserId = TownManager.Instance.GetPlayerByNickname(partyKickInputField.text).PlayerId;
        C_PartyKickRequest partyKickPacket = new C_PartyKickRequest { RequesterUserId = TownManager.Instance.MyPlayer.PlayerId, KickUserUserId = kickUserId };
        GameManager.Network.Send(partyKickPacket);
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
        //partySearchInputField.text = "";
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
        //partyNameInputField.text = "";
    }

    private void PartyInviteRequest()
    {
        C_PartyInviteRequest partyInviteRequestPacket = new C_PartyInviteRequest { RequesterUserNickname = TownManager.Instance.MyPlayer.nickname, ParticipaterUserNickname = partyInviteInputField.text };
        GameManager.Network.Send(partyInviteRequestPacket);
        //partyInviteInputField.text = "";
    }
}