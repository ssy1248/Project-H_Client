using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.AI;
using Google.Protobuf.Protocol;
using UnityEngine.XR;

public class MageController : PlayerController
{
    public GameObject meteorEffectPrefab; // 메테오 이펙트 프리팹
    public float meteorDelay = 1f; // 메테오 시전 시간
    public float meteorSpeed = 15f; // 메테오의 떨어지는 속도

    private bool isMeteorCasting = false; // 메테오 시전 상태

    Animator anim;
    MeshRenderer[] meshs;
    Rigidbody rigid;
    private Camera camera;
    private NavMeshAgent nav;

    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); // 자식이 아닌 부모 오브젝트에서 가져오기
        meshs = GetComponentsInChildren<MeshRenderer>();
        camera = Camera.main;
        nav = GetComponent<NavMeshAgent>();
        nav.updateRotation = false;
    }

    void Update()
    {
        base.Update(); // 부모 클래스의 Update 호출 (기본적인 플레이어 움직임 처리)

        if (Input.GetKeyDown(KeyCode.Q) && !isMeteorCasting)
        {
            CastMeteor();
        }
    }

    // 메테오 시전 함수
    void CastMeteor()
    {
        isMeteorCasting = true;
        anim.SetTrigger("doMeteorCast"); // 메테오 시전 애니메이션 트리거

        // 메테오 이펙트 생성
        StartCoroutine(MeteorFallRoutine());
    }

    // 메테오가 하늘에서 떨어지는 코루틴
    IEnumerator MeteorFallRoutine()
    {
        // 메테오를 일정 높이에서 떨어지게 하기 위해 초기 위치를 설정
        Vector3 spawnPosition = new Vector3(transform.position.x + Random.Range(-5f, 5f), 10f, transform.position.z + Random.Range(-5f, 5f));
        GameObject meteor = Instantiate(meteorEffectPrefab, spawnPosition, Quaternion.identity);

        // 메테오 이펙트 실행
        ParticleSystem ps = meteor.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        // 메테오가 목표 위치로 떨어지도록 계산
        Vector3 targetPosition = transform.position;
        float fallTime = 0;

        while (fallTime < meteorDelay)
        {
            meteor.transform.position = Vector3.Lerp(spawnPosition, targetPosition, fallTime / meteorDelay);
            fallTime += Time.deltaTime;
            yield return null;
        }

        meteor.transform.position = targetPosition; // 메테오 최종 목표 위치에 배치

        // 메테오가 떨어지며 데미지를 주는 처리
        Collider[] hitEnemies = Physics.OverlapSphere(targetPosition, 3f, LayerMask.GetMask("Enemy"));
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                // Enemy 스크립트에서 데미지 처리 방식에 맞게 호출
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null && !enemyScript.isDead)
                {
                    int damage = 100; // 예시로 100 데미지, 필요시 다른 값으로 설정 가능
                    enemyScript.curHealth -= damage;
                    DamageManager.Instance.SpawnDamageText(damage, enemy.transform.position, isPlayerHit: true);

                    // 데미지 받은 후 상태 처리
                    if (enemyScript.curHealth <= 0)
                    {
                        enemyScript.isDead = true;
                        enemyScript.anim.SetTrigger("doDie");
                        Destroy(enemy.gameObject, 4f); // 적이 죽은 후 4초 뒤에 제거
                    }
                }
            }
        }

        // 메테오 이펙트 비활성화
        Destroy(meteor);
        isMeteorCasting = false;
    }
}
