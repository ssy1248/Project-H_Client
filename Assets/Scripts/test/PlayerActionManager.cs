using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerActionManager : MonoBehaviour
{
    void Update()
    {
        // ���� ���� "Dungeon"���� �����ϴ��� Ȯ��
        if (SceneManager.GetActiveScene().name.StartsWith("Dungeon"))
        {
            if (Input.GetMouseButtonDown(0)) // ��Ŭ�� ����
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

                Debug.Log(normalAttack);

                // C_PlayerAction �޽����� oneof �ʵ��� normalAttack�� �� �Ҵ�
                C_PlayerAction actionPacket = new C_PlayerAction
                {
                    NormalAttack = normalAttack
                };

                Debug.Log(actionPacket);

                // ������ �޽��� ����
                GameManager.Network.Send(actionPacket);
            }
        }
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
