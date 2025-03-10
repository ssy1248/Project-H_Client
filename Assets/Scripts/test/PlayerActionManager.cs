using Google.Protobuf.Protocol;
using System;
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
            if (Input.GetMouseButtonDown(0) && DungeonManager.Instance.MyPlayer.equipWeapon.type == Weapon.Type.Melee) // 좌클릭 감지
            {
                NormalAttackRequest();
            }
            if(Input.GetMouseButtonDown(0) && DungeonManager.Instance.MyPlayer.equipWeapon.type == Weapon.Type.Range)
            {
                RangeNormalAttackRequest();
            }
            // 스킬 공격
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var myPlayer = DungeonManager.Instance.MyPlayer;
                if (myPlayer != null)
                {
                    myPlayer.Skill();
                }
            }
            // 회피
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DodgeRequest();
            }
        }
    }

    public void PlayerActionHandler(S_PlayerAction action)
    {
        // 전체적인 성공 여부와 메시지를 먼저 처리
        if (!action.Success)
        {
            Debug.Log("플레이어 액션 실패: " + action.Message);
            return;
        }


        // oneof 케이스에 따라 분기 처리
        switch (action.ActionCase)
        {
            case S_PlayerAction.ActionOneofCase.NormalAttackResult:
                ProcessNormalAttackResult(action.NormalAttackResult);
                break;
            case S_PlayerAction.ActionOneofCase.SkillAttackResult:
                ProcessSkillAttackResult(action.SkillAttackResult);
                break;
            case S_PlayerAction.ActionOneofCase.DodgeResult:
                ProcessDodgeResult(action.DodgeResult);
                break;
            case S_PlayerAction.ActionOneofCase.HitResult:
                ProcessHitResult(action.HitResult);
                break;
            case S_PlayerAction.ActionOneofCase.RangeNormalAttackResult:
                ProcessRangeNormalAttackActionResult(action.RangeNormalAttackResult);
                break;
            default:
                Debug.LogWarning("알 수 없는 플레이어 액션이 도착했습니다.");
                break;
        }
    }

    // 회피 액션이 들어오면 처리할 핸들러
    private void ProcessDodgeResult(DodgeResult result)
    {
        Debug.Log($"회피 결과: 회피로 감소한 피해량={result.EvadedDamage}, " +
             $"이동 거리={result.DodgeDistance}, " +
             $"최종 위치=({result.FinalPosition.X}, {result.FinalPosition.Y}, {result.FinalPosition.Z})");

        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player.MPlayer != null && player.nickname == result.UseUserName)
            {
                //player.Dodge();
                // 서버 계산 좌표를 부드럽게 적용합니다.
                player.InterpolateToPosition(new Vector3(
                    result.FinalPosition.X,
                    result.FinalPosition.Y,
                    result.FinalPosition.Z));
                //player.SetPosition(new Vector3(
                //    result.FinalPosition.X,
                //    result.FinalPosition.Y,
                //    result.FinalPosition.Z)
                //    );
                player.TriggerDodgeAnimation();
                break;
            }
        }
        // 여기서 UI 업데이트나 게임 로직에 반영
    }

    // 피격 액션이 들어오면 처리할 핸들러
    private void ProcessHitResult(HitResult result)
    {
        Debug.Log($"피격 결과: 받은 피해량={result.DamageReceived}, 남은 HP={result.CurrentHp}");
        // 여기서 UI 업데이트나 게임 로직에 반영
    }

    // 스킬 액션이 들어오면 처리할 핸들러
    private void ProcessSkillAttackResult(SkillAttackResult result)
    {
        Debug.Log($"스킬 공격 결과: 스킬ID={result.SkillId}");
        // 여기서 UI 업데이트나 게임 로직에 반영
        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player.MPlayer != null && player.nickname == result.UseUserName)
            {
                player.Skill();
                break;
            }
        }
    }

    // 일반 공격 액션이 들어오면 처리할 핸들러
    private void ProcessNormalAttackResult(NormalAttackResult result)
    {
        Debug.Log($"일반 공격 결과: 대상ID={result.TargetId}, 피해량={result.DamageDealt}");
        // 여기서 UI 업데이트나 게임 로직에 반영
        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if(player.MPlayer != null && player.nickname == result.UseUserName)
            {
                player.Attack();
                break;
            }
        }
    }

    private void ProcessRangeNormalAttackActionResult(RangeNormalAttackResult result)
    {
        Debug.Log($"생성된 화살 아이디 = {result.ArrowId}, 생성 완료 메세지 = {result.Message}");
        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player.MPlayer != null)
            {
                player.RangeAttack(result.ArrowId);
                break;
            }
        }
    }

    void DodgeRequest()
    {
        // 클라이언트의 MyPlayer 인스턴스를 가져옵니다.
        var myPlayer = DungeonManager.Instance.MyPlayer;
            
        // 플레이어가 바라보는 방향을 구합니다.
        Vector3 playerForward = myPlayer.transform.forward.normalized;

        // 현재 위치
        Vector currentPositionProto = new Vector
        {
            X = transform.position.x,
            Y = transform.position.y,
            Z = transform.position.z
        };

        // 프로토버퍼 메시지 형식에 맞게 Vector3 객체로 변환합니다. -> 바라보는 방향
        Vector directionProto = new Vector
        {
            X = playerForward.x,
            Y = playerForward.y,
            Z = playerForward.z
        };

        DodgeAction dodgeAction = new DodgeAction
        {
            AttackerName = myPlayer.nickname,
            CurrentPosition = currentPositionProto,
            Direction = directionProto
        };

        C_PlayerAction actionPacket = new C_PlayerAction
        {
            DodgeAction = dodgeAction
        };

        GameManager.Network.Send(actionPacket);
    }

    void RangeNormalAttackRequest()
    {
        Debug.Log("원거리 공격 발사~~");

        // 클라이언트의 MyPlayer 인스턴스를 가져옵니다.
        var myPlayer = DungeonManager.Instance.MyPlayer;

        // 플레이어가 바라보는 방향을 구합니다.
        Vector3 playerForward = myPlayer.transform.forward.normalized;

        Vector currentPositionProto = new Vector
        {
            X = playerForward.x,
            Y = playerForward.y,
            Z = playerForward.z,
        };

        RangeNormalAttackAction rangeNormalAttackAction = new RangeNormalAttackAction
        {
            Direction = currentPositionProto,
        };

        C_PlayerAction actionPacket = new C_PlayerAction
        {
            RangeNormalAttackAction = rangeNormalAttackAction
        };

        GameManager.Network.Send(actionPacket);
    }

    void NormalAttackRequest()
    {
        // 레이캐스트를 통해 타겟을 얻거나, 타겟 ID를 직접 결정할 수 있습니다.
        string targetId = GetTargetIdFromMouseClick();
        Debug.Log("TargetId : " + targetId);

        // NormalAttack 메시지 생성
        NormalAttack normalAttack = new NormalAttack
        {
            AttackerName = DungeonManager.Instance.MyPlayer.nickname,
            TargetId = targetId,
            // 필요한 경우 추가 정보(예: 위치, 방향 등)도 설정
        };

        // C_PlayerAction 메시지의 oneof 필드인 normalAttack에 값 할당
        C_PlayerAction actionPacket = new C_PlayerAction
        {
            NormalAttack = normalAttack
        };

        // 서버로 메시지 전송
        GameManager.Network.Send(actionPacket);
    }

    // 마우스 클릭 시 레이캐스트로 타겟 ID를 얻는 함수 (실제 구현은 상황에 맞게 수정)
    public string GetTargetIdFromMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float radius = 0.3f;
        if (Physics.SphereCast(ray, radius, out RaycastHit hit))
        {
            Debug.Log("레이캐스트 들어옴");
            if(hit.collider.gameObject.CompareTag("Monster") == true)
            {
                string targetID = hit.transform.GetComponent<Monster>().MonsterId;
                Debug.Log($"클릭한 몬스터 ID : {targetID}");
                return targetID;
            } 
            else
            {
                Debug.Log("태그가 틀림?");
            }
        }
        // 타겟이 없으면 0이나 -1 같은 기본값 반환 (필요에 따라 처리)
        return "-1";
    }
}
