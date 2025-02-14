using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyListItem : MonoBehaviour
{
    // 파티 정보를 저장할 변수 (서버에서 받은 PartyInfo)
    public PartyInfo partyData;

    // 버튼 컴포넌트 (이 오브젝트에 붙어있거나 자식에 있을 수 있음)
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
        {
            _button.onClick.AddListener(OnPartyListItemClicked);
        }
    }

    // 버튼 클릭 시 호출될 메서드
    private void OnPartyListItemClicked()
    {
        // TownManager에 정의된 파티원 UI 업데이트 함수를 호출합니다.
        // 예: 해당 파티의 정보를 PartyMemberSpawnPoint에 UI로 표시.
        TownManager.Instance.UpdatePartyMembersUI(partyData);
    }

    // 좀더 고민
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
