using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : Enemy
{
    Vector3 lookVec;
    Vector3 tauntVec;
    Vector3 Attack1Vec;
    Vector3 Attack2Vec;

    private Projector projector;
    bool isLook;
    bool isPatternActive;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
    }

    void Start()
    {
        isLook = true;
        isPatternActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isPatternActive)
        {
            nav.isStopped = true;
            return;
        }

        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
    }

    IEnumerator Think()
    {
        while (!isDead)
        {
            if (!isPatternActive)  // 패턴이 활성화되지 않았을 때만 실행
            {
                // 플레이어와의 거리를 확인하고, 일정 거리 이내에 들어왔으면 멈추고 패턴 실행
                float distanceToPlayer = Vector3.Distance(transform.position, target.position);

                if (distanceToPlayer > 15f)  // 일정 거리 이상 멀리 있으면 이동
                {
                    if (!isPatternActive)  // 패턴이 활성화되지 않았을 때만 이동 시작
                    {
                        yield return StartCoroutine(Walk());  // 이동 후 패턴 실행
                    }
                }
                else  // 일정 거리 이내에 들어오면 멈추고 패턴 실행
                {
                    // 패턴 활성화
                    isPatternActive = true;
                    int randomAction = Random.Range(0, 5);  // 최대 3가지 패턴으로 랜덤 변경
                    switch (randomAction)
                    {
                        case 0:
                        case 1:
                            yield return StartCoroutine(Attack1());
                            break;
                        case 2:
                        case 3:
                            yield return StartCoroutine(Attack2());
                            break;
                        case 4:
                            yield return StartCoroutine(Taunt());
                            break;
                    }

                    yield return new WaitForSeconds(0.5f);  // 패턴 후 대기
                    isPatternActive = false;  // 패턴 종료 후 이동 가능 상태로 변경
                }
            }
            yield return null;
        }
    }


    IEnumerator Attack1()
    {
        Attack1Vec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        anim.SetTrigger("Attack1");

        yield return new WaitForSeconds(0f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
    }
    IEnumerator Attack2()
    {
        Attack2Vec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        anim.SetTrigger("Attack2");

        yield return new WaitForSeconds(0f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
    }
    // 보스가 타겟에게 도달하는 이동 패턴
    IEnumerator Walk()
{
    nav.isStopped = false;
    anim.SetBool("isWalk", true);  // 걷는 애니메이션 시작

        // stoppingDistance 값을 플레이어와의 거리보다 작게 설정하여, 도달 후 바로 멈추도록 설정
        nav.stoppingDistance = 15f;

        // acceleration과 deceleration 값 조정하여 더 자연스러운 멈춤 구현
        nav.acceleration = 8f;  // 보스의 가속도를 적당히 설정

        while (Vector3.Distance(transform.position, target.position) > 15f)  // 타겟과 일정 거리 이내에 도달할 때까지
    {
        // 타겟 위치를 실시간으로 받아서 목적지를 설정
        nav.SetDestination(target.position);  // 매 프레임마다 타겟의 위치를 계속 갱신

        // 보스의 시점을 플레이어 방향으로 회전
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0;  // 수평 방향만 바라보게 하기 위해 y값을 0으로 설정
        transform.rotation = Quaternion.LookRotation(lookDirection);

        yield return null;  // 매 프레임마다 반복
    }

    anim.SetBool("isWalk", false);  // 이동이 끝나면 애니메이션 종료
    nav.isStopped = true;  // 목적지 도달 후 멈추기
}

    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
    }
}
