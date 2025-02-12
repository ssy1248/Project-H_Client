using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPartyPopUp : MonoBehaviour
{
    [SerializeField] private GameObject PartyCreateObject;
    [SerializeField] private TMP_InputField partyNameInputField;
    void Start()
    {
        PartyCreateObject.SetActive(false);
    }

    void Update()
    {

    }

    public void PartyCreateBtnClick()
    {

    }

    public void PartyBtnClick()
    {
        RequestPartyList();
    }

    private void RequestPartyList()
    {
        C_PartyListRequest partyListPacket = new C_PartyListRequest { };
        GameManager.Network.Send(partyListPacket);
    }

    private void PartyCreateRequest()
    {
        // inputField에서 받은 값 보내기
        C_PartyRequest partyRequestPacket = new C_PartyRequest { UserId = TownManager.Instance.MyPlayer.PlayerId, PartyName = partyNameInputField.text };
        GameManager.Network.Send(partyRequestPacket);
    }
}
