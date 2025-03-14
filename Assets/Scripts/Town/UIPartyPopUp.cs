using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPartyPopUp : MonoBehaviour
{
    [SerializeField] private TMP_InputField partyNameInputField;
    [SerializeField] public TMP_InputField partySearchInputField;
    [SerializeField] private TMP_InputField partyInviteInputField;
    [SerializeField] private TMP_InputField partyKickInputField;
    [SerializeField] public string selectPartyId;
    [SerializeField] public int dungeonIndex;
    [SerializeField] public GameObject RemoveBtnObj;
    [SerializeField] public string MemberClickNickname;

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
    public void PartyKickBtnRequest()
    {
        int kickUserId = TownManager.Instance.GetPlayerByNickname(MemberClickNickname).PlayerId;
        C_PartyKickRequest partyKickPacket = new C_PartyKickRequest { RequesterUserId = TownManager.Instance.MyPlayer.PlayerId, KickUserUserId = kickUserId };
        GameManager.Network.Send(partyKickPacket);

        // 컨텍스트 메뉴 닫기
        PartyStatusMemberClick memberClick = FindObjectOfType<PartyStatusMemberClick>();
        if (memberClick != null)
        {
            memberClick.CloseContextMenu();
        }
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
        C_PartyRequest partyRequestPacket = new C_PartyRequest { UserId = TownManager.Instance.MyPlayer.PlayerId, PartyName = partyNameInputField.text, DungeonIndex = dungeonIndex };
        GameManager.Network.Send(partyRequestPacket);
        //partyNameInputField.text = "";
    }

    private void PartyInviteRequest()
    {
        C_PartyInviteRequest partyInviteRequestPacket = new C_PartyInviteRequest { RequesterUserNickname = TownManager.Instance.MyPlayer.nickname, ParticipaterUserNickname = partyInviteInputField.text };
        GameManager.Network.Send(partyInviteRequestPacket);
        //partyInviteInputField.text = "";
    }

    public void PartyLeaderChangeRequest()
    {
        int ChangeUser = TownManager.Instance.GetPlayerByNickname(MemberClickNickname).PlayerId;
        C_PartyLeaderChangeRequest partyLeaderChangeRequest = new C_PartyLeaderChangeRequest { RequesterId = TownManager.Instance.MyPlayer.PlayerId, ChangeUserId = ChangeUser };
        GameManager.Network.Send(partyLeaderChangeRequest);

        // 컨텍스트 메뉴 닫기
        PartyStatusMemberClick memberClick = FindObjectOfType<PartyStatusMemberClick>();
        if (memberClick != null)
        {
            memberClick.CloseContextMenu();
        }
    }
}