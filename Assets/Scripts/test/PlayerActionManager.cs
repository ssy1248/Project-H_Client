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
            if (Input.GetMouseButtonDown(0)) // ��Ŭ�� ����
            {
                NormalAttackRequest();
            }
            // ��ų ����
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SkillAttackRequest();
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
            default:
                Debug.LogWarning("�� �� ���� �÷��̾� �׼��� �����߽��ϴ�.");
                break;
        }
    }

    // ȸ�� �׼��� ������ ó���� �ڵ鷯
    private void ProcessDodgeResult(DodgeResult result)
    {
        Debug.Log($"ȸ�� ���: ȸ�Ƿ� ������ ���ط�={result.EvadedDamage}, ���� ��Ÿ��={result.Cooldown}");
        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player.MPlayer != null && player.nickname == result.UseUserName)
            {
                if(player.IsMage())
                {
                    player.Teleport();
                }
                else
                {
                    player.Dodge();
                }
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
        Debug.Log($"��ų ���� ���: ��ųID={result.SkillId}, ���ID={result.TargetId}, ���ط�={result.DamageDealt}");
        // ���⼭ UI ������Ʈ�� ���� ������ �ݿ�
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

    void DodgeRequest()
    {
        // �ٶ󺸴� ���⵵ ��������ҵ�?
        // ȸ�� �Ÿ��� ������ ���� �ƴ� �ٶ󺸴� �������� ����ؾ��ҵ�
        DodgeAction dodgeAction = new DodgeAction
        {
            AttackerName = DungeonManager.Instance.MyPlayer.nickname,
            DodgeDistance = 5,
        };

        C_PlayerAction actionPacket = new C_PlayerAction
        {
            DodgeAction = dodgeAction,
        };

        GameManager.Network.Send(actionPacket);
    }

    void SkillAttackRequest()
    {
        int targetId = 1;

        // SkillAttack �޼��� ����
        SkillAttack skillAttack = new SkillAttack
        {
            AttackerName = DungeonManager.Instance.MyPlayer.nickname,
            SkillId = 1,
            TargetId = targetId,
            CurrentMp = 100,
        };

        // ����� mp�� ������ ���� ��� üũ�ؾ��ҵ�
        C_PlayerAction actionPacket = new C_PlayerAction
        {
            SkillAttack = skillAttack
        };

        GameManager.Network.Send(actionPacket);
    }

    void NormalAttackRequest()
    {
        // ����: ����ĳ��Ʈ�� ���� Ÿ���� ��ų�, Ÿ�� ID�� ���� ������ �� �ֽ��ϴ�.
        int targetId = 1;//GetTargetIdFromMouseClick();

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

    // ����: ���콺 Ŭ�� �� ����ĳ��Ʈ�� Ÿ�� ID�� ��� �Լ� (���� ������ ��Ȳ�� �°� ����)
    int GetTargetIdFromMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // hit.collider.gameObject�� �ش��ϴ� ������Ʈ���� Ÿ�� ID�� �����´ٰ� ����
            // ��: TargetIdentifier ��ũ��Ʈ�� �پ��ִٰ� ����
            TargetIdentifier identifier = hit.collider.GetComponent<TargetIdentifier>();
            if (identifier != null)
            {
                return identifier.targetId;
            }
        }
        // Ÿ���� ������ 0�̳� -1 ���� �⺻�� ��ȯ (�ʿ信 ���� ó��)
        return -1;
    }
}
