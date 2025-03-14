using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    private static BossManager _instance = null;
    public static BossManager Instance => _instance;

    public GameObject demonKingPrefab;  // ���� ������
    private GameObject currentBoss;
    private BossController boss;

    public GameObject bladePoolPrefab;  // ���̵� ������
    private GameObject currentBladePool;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SpawnBoss(S_BossSpawn bossSpawnPacket)
    {
        Vector3 currentPosition;
        currentPosition.x = bossSpawnPacket.CurrentPosition.X;
        currentPosition.y = bossSpawnPacket.CurrentPosition.Y;
        currentPosition.z = bossSpawnPacket.CurrentPosition.Z;


        // ���̵� ����
        if (bladePoolPrefab != null)
        {
            currentBladePool = Instantiate(bladePoolPrefab, new Vector3(2, 0, 0), Quaternion.identity);
        }

        // ���� ���� ����
        if (demonKingPrefab != null)
        {
            currentBoss = Instantiate(demonKingPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            boss = currentBoss.GetComponent<BossController>();
            boss.SetId(bossSpawnPacket.BossId);

        }

       


        if (currentBoss != null )
        {
            currentBoss.transform.position = currentPosition;
            currentBoss.transform.localScale = new Vector3(2f, 2f, 2f);
        }
    }

    public void MoveBoss(S_BossMove bossMovePacket)
    {
        Debug.Log("���� ���ž��մϴ�.");
        Vector3 targetPosition = new Vector3(bossMovePacket.TargetPosition.X, bossMovePacket.TargetPosition.Y, bossMovePacket.TargetPosition.Z);
        Debug.Log($"������������ ���� ��ǥ ��Ŷ : {targetPosition}");
        if (boss != null)
        {
            boss.handleBossMove(targetPosition);
        }
        else
        {
            Debug.LogWarning("BossController�� �����ϴ�.");
        }

    }

    public void OnBossDeath()
    {
        boss.BossDie();

        // �ڷ�ƾ ����
        StartCoroutine(DisableBossAfterDelay(3.0f));

    }


    private IEnumerator DisableBossAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentBoss.SetActive(false);
        currentBladePool.SetActive(false);

        Destroy(currentBoss);        // ���� ���� ����
        Destroy(currentBladePool);   // ���̵� Ǯ ���� ����

        yield return new WaitForSeconds(0.1f); // ������ �Ϸ�� ������ ���

        if (currentBoss != null)
        {
            Debug.LogError($"currentBoss�� ���� ���� ���� ����: {currentBoss}");
        }
        if (currentBladePool != null)
        {
            Debug.LogError($"currentBladePool�� ���� ���� ���� ����: {currentBladePool}");
        }
    }

    public void TakeDamage(S_BossHit bossHitPacket)
    {
        int bossHp = boss.GetHp();
        bossHp -= bossHitPacket.Damage;
        boss.SetHp(bossHp);

    }

    public void ReceiveSkillPacket(S_BossSkillStart bossSkillStartPacket)
    {
        Debug.Log($"���� ��ų�� : {bossSkillStartPacket.Type}");
        if (boss != null)
        {
            boss.handleBossSkill(bossSkillStartPacket.Type);
        }
        else
        {
            Debug.LogWarning("BossController�� �����ϴ�.");
        }


    }

    void Update()
    {
        
    }
}
