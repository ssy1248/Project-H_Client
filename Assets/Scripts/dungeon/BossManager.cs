using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    private static BossManager _instance = null;
    public static BossManager Instance => _instance;

    public GameObject demonKingPrefab;  // 보스 프리팹
    private GameObject currentBoss;
    private BossController boss;

    public GameObject bladePoolPrefab;  // 블레이드 프리팹
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


        // 블레이드 생성
        if (bladePoolPrefab != null)
        {
            currentBladePool = Instantiate(bladePoolPrefab, new Vector3(2, 0, 0), Quaternion.identity);
        }

        // 보스 몬스터 생성
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
        Debug.Log("여긴 오셔야합니다.");
        Vector3 targetPosition = new Vector3(bossMovePacket.TargetPosition.X, bossMovePacket.TargetPosition.Y, bossMovePacket.TargetPosition.Z);
        Debug.Log($"서버에서받은 보스 목표 패킷 : {targetPosition}");
        if (boss != null)
        {
            boss.handleBossMove(targetPosition);
        }
        else
        {
            Debug.LogWarning("BossController가 없습니다.");
        }

    }

    public void OnBossDeath()
    {
        boss.BossDie();

        // 코루틴 실행
        StartCoroutine(DisableBossAfterDelay(1.0f));

    }


    private IEnumerator DisableBossAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentBoss.SetActive(false);
        currentBladePool.SetActive(false);
    }

    void TakeDamage()
    {

    }

    public void ReceiveSkillPacket(S_BossSkillStart bossSkillStartPacket)
    {
        Debug.Log($"보스 스킬은 : {bossSkillStartPacket.Type}");
        if (boss != null)
        {
            boss.handleBossSkill(bossSkillStartPacket.Type);
        }
        else
        {
            Debug.LogWarning("BossController가 없습니다.");
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
