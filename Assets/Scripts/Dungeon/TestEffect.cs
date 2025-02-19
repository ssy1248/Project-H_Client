using UnityEngine;

public class Attack : MonoBehaviour
{
    public Transform playerCube;  // �÷��̾� ť��
    public Transform monsterCube;  // ���� ť��
    public int playerDamage = 10;
    public int monsterDamage = 5;
    public float attackRange = 2f;  // ���� ����

    private void Update()
    {
        // 'A' Ű: �÷��̾� ����
        if (Input.GetKeyDown(KeyCode.A))  // A Ű�� �÷��̾� ����
        {
            AttackPlayer();
        }

        // �����̽���: ���� ����
        if (Input.GetKeyDown(KeyCode.Space))  // �����̽��ٷ� ���� ����
        {
            AttackMonster();
        }

        // �÷��̾�� ���Ͱ� ���� �ٶ󺸰�
        playerCube.LookAt(monsterCube);
        monsterCube.LookAt(playerCube);
    }

    private void AttackPlayer()
    {
        // �÷��̾ �����ϴ� ���
        // �÷��̾�� ������ �Ÿ� üũ
        float distance = Vector3.Distance(playerCube.position, monsterCube.position);

        // ���� ���� �ȿ� ���� ��� ����
        if (distance <= attackRange)
        {
            Vector3 hitPosition = monsterCube.position + new Vector3(0, 1, 0); // �Ӹ� ��
            DamageManager.Instance.SpawnDamageText(playerDamage, hitPosition, true);
        }
    }

    private void AttackMonster()
    {
        // ���Ͱ� �÷��̾ �����ϴ� ���
        float distance = Vector3.Distance(monsterCube.position, playerCube.position);

        // ���� ���� �ȿ� ���� ��� ����
        if (distance <= attackRange)
        {
            Vector3 hitPosition = playerCube.position + new Vector3(0, 1, 0); // �Ӹ� ��
            DamageManager.Instance.SpawnDamageText(monsterDamage, hitPosition, false);
        }
    }
}
