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

    private int bossMaxhealth = 3000;


    // 부채꼴 범위 표시용 GameObject
    public GameObject fanShapeRange;  // 부채꼴 모양 범위 표시용 GameObject
    public GameObject squareShapeRange;  // 사각형 모양 범위 표시용 GameObject
    public GameObject circleShapeRange;
    public GameObject bladePrefab;
    private BoxCollider squareCollider;
    private SphereCollider fanCollider;


    void Awake()
    {
        maxHealth = bossMaxhealth;
        curHealth = maxHealth;
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

        // 부채꼴 범위 초기화
        if (fanShapeRange != null)
        {
            fanShapeRange.SetActive(false);  // 초기에는 비활성화
            fanCollider = fanShapeRange.AddComponent<SphereCollider>();
            fanCollider.isTrigger = true;
        }

        // 사각형 범위 초기화
        if (squareShapeRange != null)
        {
            squareShapeRange.SetActive(false);  // 초기에는 비활성화
            squareCollider = squareShapeRange.AddComponent<BoxCollider>();
            squareCollider.isTrigger = true;
        }
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
                    int randomAction = Random.Range(0, 10);  // 최대 3가지 패턴으로 랜덤 변경
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
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                            yield return StartCoroutine(Attack3());
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

        // 부채꼴 범위 표시 활성화
        if (squareShapeRange != null)
        {
            squareShapeRange.SetActive(true);

            // 부채꼴 범위의 위치를 보스 위치로 설정
            squareShapeRange.transform.position = transform.position + (transform.forward * 12f);
            // 부채꼴의 크기 조정
            squareShapeRange.transform.localScale = new Vector3(0.5f, 1f, 8f); // 적절한 크기로 조정

            Vector3 direction = transform.forward;  // 보스의 방향

            // 부채꼴이 보스 앞에 위치하고 범위가 펼쳐지도록 회전 조정
            squareShapeRange.transform.rotation = Quaternion.LookRotation(direction);

            squareCollider.size = squareShapeRange.transform.localScale;
        }

        yield return new WaitForSeconds(3f);

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;

        // 부채꼴 범위 표시 비활성화
        if (squareShapeRange != null)
        {
            squareShapeRange.SetActive(false);
        }


    }


    IEnumerator Attack2()
    {
        Attack2Vec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        anim.SetTrigger("Attack2");

        // 부채꼴 범위 표시 활성화
        if (fanShapeRange != null)
        {
            fanShapeRange.SetActive(true);

            // 부채꼴 범위의 위치를 보스 위치로 설정
            fanShapeRange.transform.position = transform.position;

            // 부채꼴의 크기 조정
            fanShapeRange.transform.localScale = new Vector3(4f, 4f, 4f); // 적절한 크기로 조정

            // 보스가 향하고 있는 방향을 기준으로 부채꼴의 회전 설정
            // 보스의 forward 방향 (현재 위치에서 향하는 방향)
            Vector3 direction = transform.forward;  // 보스의 방향을 가져옵니다.

            // 부채꼴의 회전을 보스가 향하는 방향으로 맞춤
            fanShapeRange.transform.rotation = Quaternion.LookRotation(direction);

            // 부채꼴이 바닥에 평평하게 있도록 회전 조정 (수평으로 위치)
            fanShapeRange.transform.Rotate(90f, -45f, 0f);  // X축을 90도 회전시켜 수평으로 맞춤

            fanCollider.radius = fanShapeRange.transform.localScale.x / 2f;
        }

        yield return new WaitForSeconds(3f);

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;

        // 부채꼴 범위 표시 비활성화
        if (fanShapeRange != null)
        {
            fanShapeRange.SetActive(false);
        }


    }

    IEnumerator Attack3()
    {
        isLook = false;
        nav.isStopped = false;
        anim.SetTrigger("Attack3"); // 보스 공격 애니메이션 실행

        int bladeCount = 50; // 떨어지는 칼 개수
        float radius = 50f; // 칼이 떨어지는 범위 반경
        float warningDuration = 1f; // 경고 범위 표시 시간

        List<GameObject> warningCircles = new List<GameObject>(); // 개별 원형 범위 저장 리스트

        // 각 칼이 떨어질 위치를 미리 생성하여 경고 표시
        Vector3[] attackPositions = new Vector3[bladeCount];
        for (int i = 0; i < bladeCount; i++)
        {
            // 보스 주변의 랜덤 각도와 거리로 위치 생성
            float angle = Random.Range(0f, 360f); // 0부터 360도 사이의 랜덤 각도
            float distance = Random.Range(0f, radius); // 0부터 지정된 반경 사이의 랜덤 거리
            Vector3 randomPos = target.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * distance, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * distance); // 원형 범위 계산

            attackPositions[i] = new Vector3(randomPos.x, 1.5f, randomPos.z); // Y값을 1.5f로 고정하여 칼이 떨어지게 설정

            // 원형 범위 경고 이펙트 생성 (지면에 배치)
            if (circleShapeRange != null)
            {
                GameObject warningCircle = Instantiate(circleShapeRange, attackPositions[i], Quaternion.identity);
                warningCircle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); // 크기 설정
                warningCircle.transform.rotation = Quaternion.Euler(-90f, 90f, 0); // 원형 범위가 올바르게 보이도록 회전

                // SphereCollider 설정
                SphereCollider collider = warningCircle.GetComponent<SphereCollider>();
                if (collider == null)
                {
                    collider = warningCircle.AddComponent<SphereCollider>(); // 없으면 추가
                }
                collider.radius = warningCircle.transform.localScale.x / 2f; // 크기에 맞게 설정
                collider.isTrigger = true; // 충돌 감지용

                warningCircle.SetActive(true);
                warningCircles.Add(warningCircle);
            }
        }

        yield return new WaitForSeconds(warningDuration); // 경고 표시 후 1초 대기

        // 칼이 순차적으로 떨어짐
        for (int i = 0; i < bladeCount; i++)
        {
            Vector3 spawnPosition = new Vector3(attackPositions[i].x, 12f, attackPositions[i].z); // Y축을 10f로 설정하여 위에서 떨어지도록 조정
            GameObject blade = Instantiate(bladePrefab, spawnPosition, Quaternion.identity);

            // 칼을 180도 회전시켜서 떨어지도록 적용
            blade.transform.rotation = Quaternion.Euler(180f, 0f, 0f); // X축을 기준으로 180도 회전

            blade.GetComponent<Rigidbody>().velocity = Vector3.down * 5f; // 아래로 떨어지는 속도 적용

        }

        yield return new WaitForSeconds(1f); // 칼이 다 떨어진 후

        // 경고 이펙트 제거
        foreach (var circle in warningCircles)
        {
            if (circle != null)
            {
                Destroy(circle);
            }
        }

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

        // 부채꼴 범위 표시 활성화
        if (fanShapeRange != null)
        {
            fanShapeRange.SetActive(true);

            // 부채꼴 범위의 위치를 보스 위치로 설정
            fanShapeRange.transform.position = transform.position;

            // 부채꼴의 크기 조정
            fanShapeRange.transform.localScale = new Vector3(5f, 4f, 5f); // 적절한 크기로 조정

            // 보스가 향하고 있는 방향을 기준으로 부채꼴의 회전 설정
            // 보스의 forward 방향 (현재 위치에서 향하는 방향)
            Vector3 direction = transform.forward;  // 보스의 방향을 가져옵니다.

            // 부채꼴의 회전을 보스가 향하는 방향으로 맞춤
            fanShapeRange.transform.rotation = Quaternion.LookRotation(direction);

            // 부채꼴이 바닥에 평평하게 있도록 회전 조정 (수평으로 위치)
            fanShapeRange.transform.Rotate(90f, -15f, 0f);  // X축을 90도 회전시켜 수평으로 맞춤

            fanCollider.radius = fanShapeRange.transform.localScale.x / 2f;
        }

        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(3f);

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;

        // 부채꼴 범위 표시 비활성화
        if (fanShapeRange != null)
        {
            fanShapeRange.SetActive(false);
        }
    }
}
