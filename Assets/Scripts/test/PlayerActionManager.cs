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
        // ���� ���� "Dungeon"���� �����ϴ��� Ȯ��
        if (SceneManager.GetActiveScene().name.StartsWith("Dungeon"))
        {
            // �Ϲ� ����
            if (Input.GetMouseButtonDown(0) && DungeonManager.Instance.MyPlayer.equipWeapon.type == Weapon.Type.Melee) // ��Ŭ�� ����
            {
                NormalAttackRequest();
            }
            if(Input.GetMouseButtonDown(0) && DungeonManager.Instance.MyPlayer.equipWeapon.type == Weapon.Type.Range)
            {
                RangeNormalAttackRequest();
            }
            // ��ų ����
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var myPlayer = DungeonManager.Instance.MyPlayer;
                if (myPlayer != null)
                {
                    myPlayer.Skill();
                }
            }
            // ȸ��
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DodgeRequest();
            }
        }
    }

    public void PlayerActionHandler(S_PlayerAction action)
    {
        // ��ü���� ���� ���ο� �޽����� ���� ó��
        if (!action.Success)
        {
            Debug.Log("�÷��̾� �׼� ����: " + action.Message);
            return;
        }


        // oneof ���̽��� ���� �б� ó��
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
                Debug.LogWarning("�� �� ���� �÷��̾� �׼��� �����߽��ϴ�.");
                break;
        }
    }

    // ȸ�� �׼��� ������ ó���� �ڵ鷯
    private void ProcessDodgeResult(DodgeResult result)
    {
        Debug.Log($"ȸ�� ���: ȸ�Ƿ� ������ ���ط�={result.EvadedDamage}, " +
             $"�̵� �Ÿ�={result.DodgeDistance}, " +
             $"���� ��ġ=({result.FinalPosition.X}, {result.FinalPosition.Y}, {result.FinalPosition.Z})");

        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player.MPlayer != null && player.nickname == result.UseUserName)
            {
                //player.Dodge();
                // ���� ��� ��ǥ�� �ε巴�� �����մϴ�.
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
        // ���⼭ UI ������Ʈ�� ���� ������ �ݿ�
    }

    // �ǰ� �׼��� ������ ó���� �ڵ鷯
    private void ProcessHitResult(HitResult result)
    {
        Debug.Log($"�ǰ� ���: ���� ���ط�={result.DamageReceived}, ���� HP={result.CurrentHp}");
        // ���⼭ UI ������Ʈ�� ���� ������ �ݿ�
    }

    // ��ų �׼��� ������ ó���� �ڵ鷯
    private void ProcessSkillAttackResult(SkillAttackResult result)
    {
        Debug.Log($"��ų ���� ���: ��ųID={result.SkillId}");
        // ���⼭ UI ������Ʈ�� ���� ������ �ݿ�
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

    // �Ϲ� ���� �׼��� ������ ó���� �ڵ鷯
    private void ProcessNormalAttackResult(NormalAttackResult result)
    {
        Debug.Log($"�Ϲ� ���� ���: ���ID={result.TargetId}, ���ط�={result.DamageDealt}");
        // ���⼭ UI ������Ʈ�� ���� ������ �ݿ�
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
        Debug.Log($"������ ȭ�� ���̵� = {result.ArrowId}, ���� �Ϸ� �޼��� = {result.Message}");
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
        // Ŭ���̾�Ʈ�� MyPlayer �ν��Ͻ��� �����ɴϴ�.
        var myPlayer = DungeonManager.Instance.MyPlayer;
            
        // �÷��̾ �ٶ󺸴� ������ ���մϴ�.
        Vector3 playerForward = myPlayer.transform.forward.normalized;

        // ���� ��ġ
        Vector currentPositionProto = new Vector
        {
            X = transform.position.x,
            Y = transform.position.y,
            Z = transform.position.z
        };

        // ��������� �޽��� ���Ŀ� �°� Vector3 ��ü�� ��ȯ�մϴ�. -> �ٶ󺸴� ����
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
        Debug.Log("���Ÿ� ���� �߻�~~");

        // Ŭ���̾�Ʈ�� MyPlayer �ν��Ͻ��� �����ɴϴ�.
        var myPlayer = DungeonManager.Instance.MyPlayer;

        // �÷��̾ �ٶ󺸴� ������ ���մϴ�.
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
        // ����ĳ��Ʈ�� ���� Ÿ���� ��ų�, Ÿ�� ID�� ���� ������ �� �ֽ��ϴ�.
        string targetId = GetTargetIdFromMouseClick();
        Debug.Log("TargetId : " + targetId);

        // NormalAttack �޽��� ����
        NormalAttack normalAttack = new NormalAttack
        {
            AttackerName = DungeonManager.Instance.MyPlayer.nickname,
            TargetId = targetId,
            // �ʿ��� ��� �߰� ����(��: ��ġ, ���� ��)�� ����
        };

        // C_PlayerAction �޽����� oneof �ʵ��� normalAttack�� �� �Ҵ�
        C_PlayerAction actionPacket = new C_PlayerAction
        {
            NormalAttack = normalAttack
        };

        // ������ �޽��� ����
        GameManager.Network.Send(actionPacket);
    }

    // ���콺 Ŭ�� �� ����ĳ��Ʈ�� Ÿ�� ID�� ��� �Լ� (���� ������ ��Ȳ�� �°� ����)
    public string GetTargetIdFromMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float radius = 0.3f;
        if (Physics.SphereCast(ray, radius, out RaycastHit hit))
        {
            Debug.Log("����ĳ��Ʈ ����");
            if(hit.collider.gameObject.CompareTag("Monster") == true)
            {
                string targetID = hit.transform.GetComponent<Monster>().MonsterId;
                Debug.Log($"Ŭ���� ���� ID : {targetID}");
                return targetID;
            } 
            else
            {
                Debug.Log("�±װ� Ʋ��?");
            }
        }
        // Ÿ���� ������ 0�̳� -1 ���� �⺻�� ��ȯ (�ʿ信 ���� ó��)
        return "-1";
    }
}
