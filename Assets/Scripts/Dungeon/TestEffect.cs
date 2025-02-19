using UnityEngine;

public class Attack : MonoBehaviour
{
    public Transform playerCube;  // 플레이어 큐브
    public Transform monsterCube;  // 몬스터 큐브
    public int playerDamage = 10;
    public int monsterDamage = 5;
    public float attackRange = 2f;  // 공격 범위

    private void Update()
    {
        // 'A' 키: 플레이어 공격
        if (Input.GetKeyDown(KeyCode.A))  // A 키로 플레이어 공격
        {
            AttackPlayer();
        }

        // 스페이스바: 몬스터 공격
        if (Input.GetKeyDown(KeyCode.Space))  // 스페이스바로 몬스터 공격
        {
            AttackMonster();
        }

        // 플레이어와 몬스터가 서로 바라보게
        playerCube.LookAt(monsterCube);
        monsterCube.LookAt(playerCube);
    }

    private void AttackPlayer()
    {
        // 플레이어가 공격하는 방식
        // 플레이어와 몬스터의 거리 체크
        float distance = Vector3.Distance(playerCube.position, monsterCube.position);

        // 공격 범위 안에 있을 경우 공격
        if (distance <= attackRange)
        {
            Vector3 hitPosition = monsterCube.position + new Vector3(0, 1, 0); // 머리 위
            DamageManager.Instance.SpawnDamageText(playerDamage, hitPosition, true);
        }
    }

    private void AttackMonster()
    {
        // 몬스터가 플레이어를 공격하는 방식
        float distance = Vector3.Distance(monsterCube.position, playerCube.position);

        // 공격 범위 안에 있을 경우 공격
        if (distance <= attackRange)
        {
            Vector3 hitPosition = playerCube.position + new Vector3(0, 1, 0); // 머리 위
            DamageManager.Instance.SpawnDamageText(monsterDamage, hitPosition, false);
        }
    }
}
