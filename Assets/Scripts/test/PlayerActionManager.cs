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

            }
        }
    }

    public void PlayerActionHandler(S_PlayerAction action)
    {
        if(action.Success)
        {

        }
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
