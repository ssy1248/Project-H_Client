using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerActionManager : MonoBehaviour
{
    private static PlayerActionManager _instance;
    public static PlayerActionManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        // 현재 씬이 "Dungeon"으로 시작하는지 확인
        if (SceneManager.GetActiveScene().name.StartsWith("Dungeon"))
        {
            // 일반 공격
            if (Input.GetMouseButtonDown(0)) // 좌클릭 감지
            {
                NormalAttackRequest();
            }
            // 스킬 공격
            if (Input.GetKeyDown(KeyCode.Q))
            {

            }
            // 회피
            if (Input.GetKeyDown(KeyCode.Space))
            {

            }
        }
    }

    public void PlayerActionHandler(S_PlayerAction action)
    {
        if(action.Success)
        {

        }
    }

    void NormalAttackRequest()
    {
        // 예시: 레이캐스트를 통해 타겟을 얻거나, 타겟 ID를 직접 결정할 수 있습니다.
        int targetId = 1;//GetTargetIdFromMouseClick();

        // NormalAttack 메시지 생성
        NormalAttack normalAttack = new NormalAttack
        {
            AttackerName = DungeonManager.Instance.MyPlayer.nickname,
            TargetId = targetId,
            // 필요한 경우 추가 정보(예: 위치, 방향 등)도 설정
        };

        Debug.Log(normalAttack);

        // C_PlayerAction 메시지의 oneof 필드인 normalAttack에 값 할당
        C_PlayerAction actionPacket = new C_PlayerAction
        {
            NormalAttack = normalAttack
        };

        Debug.Log(actionPacket);

        // 서버로 메시지 전송
        GameManager.Network.Send(actionPacket);
    }

    // 예시: 마우스 클릭 시 레이캐스트로 타겟 ID를 얻는 함수 (실제 구현은 상황에 맞게 수정)
    int GetTargetIdFromMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // hit.collider.gameObject에 해당하는 오브젝트에서 타겟 ID를 가져온다고 가정
            // 예: TargetIdentifier 스크립트가 붙어있다고 가정
            TargetIdentifier identifier = hit.collider.GetComponent<TargetIdentifier>();
            if (identifier != null)
            {
                return identifier.targetId;
            }
        }
        // 타겟이 없으면 0이나 -1 같은 기본값 반환 (필요에 따라 처리)
        return -1;
    }
}
