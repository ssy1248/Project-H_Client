using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class BossController : Enemy
{
    Vector3 lookVec;
    Vector3 tauntVec;
    Vector3 AttackVec;
    

    private BladePool bladePool;
    bool isLook;
    bool isPatternActive;

    private int bossMaxhealth = 3000;

    string id;
    int hp;

    // 타겟 좌표.
    private Vector3 targetPosition = new Vector3(0f,0f,0f);
    bool isMove = false;
    bool isSkill = false;
    float moveSpeed = 4f;
    float smoothRotateSpeed = 4f;



    // 부채꼴 범위 표시용 GameObject
    public GameObject fanShapeRange;  // 부채꼴 모양 범위 표시용 GameObject
    public GameObject squareShapeRange;  // 사각형 모양 범위 표시용 GameObject
    public GameObject circleShapeRange;
    public GameObject bladePrefab;
    private BoxCollider squareCollider;
    private SphereCollider fanCollider;
    private SphereCollider circleCollider;

    // 보스 무기 Collider
    public GameObject bossWeapon;  // 보스 무기 오브젝트 (Collider 포함)
    public GameObject effectObject; //보스 무기 이펙트
    private Collider weaponCollider;

    private void Start()
    {
        //StartCoroutine(Think());
    }

    void Awake()
    {
        Debug.Log("[초기화 진행 - 보스몬스터]");
        maxHealth = bossMaxhealth;
        curHealth = maxHealth;
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        //nav.isStopped = true;
        //StartCoroutine(Think());

        //스킬 풀 찾기
        bladePool = FindObjectOfType<BladePool>();

        // 스킬 풀을 못찾으면..
        if (bladePool == null)
        {
            bladePool = GameObject.Find("BladePool(Clone)").GetComponent<BladePool>();
        }

        // 콜라이더 미리 초기화
        if (fanShapeRange != null)
        {
            fanCollider = fanShapeRange.GetComponent<SphereCollider>();
            if (fanCollider == null)
                fanCollider = fanShapeRange.AddComponent<SphereCollider>();

            fanCollider.isTrigger = true;
            fanShapeRange.SetActive(false);
        }

        if (squareShapeRange != null)
        {
            Debug.Log("squareShapeRange는 있음.");
            squareCollider = squareShapeRange.GetComponent<BoxCollider>();
            if (squareCollider == null)
            {
                squareCollider = squareShapeRange.AddComponent<BoxCollider>();
                Debug.Log("squareCollider가 없어서 새로만듬.");
            }

            Debug.Log("squareCollider는 있음");
            squareCollider.isTrigger = true;
            squareShapeRange.SetActive(false);
        }

        if (circleShapeRange != null)
        {
            circleCollider = circleShapeRange.GetComponent<SphereCollider>();
            if (circleCollider == null)
                circleCollider = circleShapeRange.AddComponent<SphereCollider>();

            circleCollider.isTrigger = true;
            circleShapeRange.SetActive(false);
        }

        // 보스 무기 Collider 가져오기
        if (bossWeapon != null)
        {
            weaponCollider = bossWeapon.GetComponent<Collider>();
            if (weaponCollider != null)
                weaponCollider.enabled = false; // 기본적으로 비활성화
        }

        // Effect 오브젝트 활성화
        if (effectObject != null)
        {
            effectObject.SetActive(false);
        }
    }

    public void SetId(string boosId)
    {
        id = boosId;
    }

    public void SetHp(int bossHp)
    {
        hp = bossHp;
    }

    void Update()
    {
        if (isDead)
        {
            // StopAllCoroutines();
            return;
        }


        if (isMove)
        {
            Move();
        }

        if (isPatternActive)
        {
            //nav.isStopped = true;
            return;
        }

        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(targetPosition + lookVec);
        }
    }

    public void handleBossMove(Vector3 pos)
    {
        targetPosition = pos;
        isMove = true;
        anim.SetBool("isWalk", true);  // 걷는 애니메이션 시작

        Debug.Log("보스 무브?");
    }

    private void Move()
    {
        Debug.Log($"보스 타겟 좌표 : {targetPosition}");

        // 이동 방향 벡터 구하기
        Vector3 dir = new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position;

        // 목표 위치와 현재 위치 간의 거리 계산
        float distance = dir.magnitude;

        // 목표 위치에 도달하면 이동을 멈추도록 설정
        if (distance > 0.1f)
        {
            // 회전 처리
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, smoothRotateSpeed);

            // 이동 처리
            transform.position += dir.normalized * Time.deltaTime * moveSpeed;
        }
        else
        {
            // 목표에 도달했을 때 멈추는 로직
            transform.position = targetPosition; // 목표 위치에 정확히 도달
            anim.SetBool("isWalk", false);  // 걷는 애니메이션 시작
        }
    }



    public void handleBossSkill(string type)
    {
        if(!isSkill)
        {
            isSkill = true;

            switch (type)
            {
                case "skill_01":
                    StartCoroutine(Skill_01());
                    break;
                case "skill_02":
                    StartCoroutine(Skill_02());
                    break;
                case "skill_03":
                    StartCoroutine(Skill_03());
                    break;
                case "skill_04":
                    StartCoroutine(Skill_04());
                    break;
                case "skill_05":
                    StartCoroutine(Skill_05());
                    break;
            }
        }
    }



    // 일정 시간이 지나면 블레이드를 풀에 반환하는 함수
    IEnumerator ReturnBladeToPool(GameObject blade, float delay)
    {
        yield return new WaitForSeconds(delay);
        bladePool.ReturnBlade(blade);
    }

    private Coroutine _stateCoroutine = null;
    private Coroutine StartAttack(int randomAction)
    {
        if (_stateCoroutine != null)
        {
            StopCoroutine(_stateCoroutine);
            _stateCoroutine = null;
        }

        switch (randomAction)
        {
            case 0:
                _stateCoroutine = StartCoroutine(Attack1());
                break;
            case 1:
                _stateCoroutine = StartCoroutine(Attack2());
                break;
            case 2:
                _stateCoroutine = StartCoroutine(Taunt());
                break;
            case 3:
                _stateCoroutine = StartCoroutine(Attack3());
                break;
            case 4:
                _stateCoroutine = StartCoroutine(Walk());
                break;
            case 5:
                _stateCoroutine= StartCoroutine(Attack4());
                break;
        }

        return _stateCoroutine;
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
                        yield return StartAttack(4);  // 이동 후 패턴 실행
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
                            yield return StartAttack(0);
                            break;
                        case 2:
                        case 3:
                            yield return StartAttack(1);
                            break;
                        case 4:
                            yield return StartAttack(2);
                            break;
                        case 5:
                            yield return StartAttack(3);
                            break;
                        case 6:
                            yield return StartAttack(5);
                            break;
                    }

                    yield return new WaitForSeconds(0.5f);  // 패턴 후 대기
                    isPatternActive = false;  // 패턴 종료 후 이동 가능 상태로 변경
                }
            }
            yield return null;
        }
    }

    IEnumerator Skill_01()
    {
        AttackVec = targetPosition + lookVec;


        isLook = false;
        // nav.isStopped = false;
        anim.SetTrigger("Attack1");

        yield return new WaitForSeconds(0.3f);
        SEManager.instance.PlaySE("BossHit");

        // 무기 충돌 활성화
        if (weaponCollider != null)
            weaponCollider.enabled = true;

        // 사각형 범위 표시 활성화
        if (squareShapeRange != null)
        {
            squareShapeRange.SetActive(true);

            // 부채꼴 범위의 위치를 보스 위치로 설정
            squareShapeRange.transform.position = transform.position + (transform.forward * 12f);
            // 부채꼴의 크기 조정
            squareShapeRange.transform.localScale = new Vector3(0.6f, 1f, 9f); // 적절한 크기로 조정

            Vector3 direction = transform.forward;  // 보스의 방향

            // 부채꼴이 보스 앞에 위치하고 범위가 펼쳐지도록 회전 조정
            squareShapeRange.transform.rotation = Quaternion.LookRotation(direction);

            squareCollider.size = squareShapeRange.transform.localScale;
        }

        yield return new WaitForSeconds(2f);

        // 여기서 클라 -> 서버 메시지
        Vector3 center = squareShapeRange.transform.position;
        Vector3 direction2 = transform.forward;
        Vector3 localScale = squareShapeRange.transform.localScale;
        Vector3 worldScale = squareShapeRange.transform.TransformVector(localScale);
        Skill_01_Send(center, direction2, worldScale.x, worldScale.y, worldScale.z);

        isLook = true;
        // nav.isStopped = true;

        // 부채꼴 범위 표시 비활성화
        if (squareShapeRange != null)
        {
            squareShapeRange.SetActive(false);
        }

        // 무기 충돌 비활성화
        if (weaponCollider != null)
            weaponCollider.enabled = false;

    }

    public void Skill_01_Send(Vector3 center, Vector3 direction, float width, float height, float length)
    {
        Vector protoVector = new Vector
        {
            X = transform.position.x,
            Y = transform.position.y,
            Z = transform.position.z
        };

        Vector centerVector = new Vector
        {
            X = center.x,
            Y = center.y,
            Z = center.z
        };

        Vector directionVector = new Vector
        {
            X = direction.x,
            Y = direction.y,
            Z = direction.z
        };

        var bossSkillPacket = new C_BossSkill
        {
            BossId = id,
            Type = "Skill_01",
            CurrentPosition = protoVector,

            Rectangle = new RectangleArea
            {
                Center = centerVector,
                Direction = directionVector,
                Width = width,
                Height = height,
                Length = length,

            }
        };

        GameManager.Network.Send(bossSkillPacket);
        isSkill = false;
    }


    IEnumerator Skill_02() 
    {
        AttackVec = targetPosition + lookVec;

        isLook = false;
        // nav.isStopped = false;
        anim.SetTrigger("Attack2");
        yield return new WaitForSeconds(0.3f);
        SEManager.instance.PlaySE("BossSwing");

        // Effect 오브젝트 활성화
        if (effectObject != null)
        {
            effectObject.SetActive(true);
        }

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

        yield return new WaitForSeconds(2f);

        // 여기에 로직 패킷 메세지 보낼거임
        Skill_02_Send(transform.position, transform.forward, fanCollider.radius, 45f);


        isLook = true;
        // nav.isStopped = true;

        // 부채꼴 범위 표시 비활성화
        if (fanShapeRange != null)
        {
            fanShapeRange.SetActive(false);
        }

        // Effect 오브젝트 비활성화
        if (effectObject != null)
        {
            effectObject.SetActive(false);
        }
    }

    public void Skill_02_Send(Vector3 center, Vector3 direction, float radius, float angle)
    {
        Vector protoVector = new Vector
        {
            X = transform.position.x,
            Y = transform.position.y,
            Z = transform.position.z
        };

        Vector centerVector = new Vector
        {
            X = center.x,
            Y = center.y,
            Z = center.z
        };

        Vector directionVector = new Vector
        {
            X = direction.x,
            Y = direction.y,
            Z = direction.z
        };

        var bossSkillPacket = new C_BossSkill
        {
            BossId = id,
            Type = "Skill_02",
            CurrentPosition = protoVector,

            Sector = new SectorArea
            {
                Center = centerVector,
                Direction = directionVector,
                Radius = radius,
                Angle = angle,
            }
        };

        GameManager.Network.Send(bossSkillPacket);
        isSkill = false;
    }

    IEnumerator Skill_03() 
    {
        isLook = false;
        // nav.isStopped = false;
        anim.SetTrigger("Attack3"); // 보스 공격 애니메이션 실행

        yield return new WaitForSeconds(0.2f);

        int bladeCount = 50; // 떨어지는 칼 개수
        float radius = 50f; // 칼이 떨어지는 범위 반경
        float warningDuration = 1f; // 경고 범위 표시 시간

        List<GameObject> warningCircles = new List<GameObject>(); // 개별 원형 범위 저장 리스트

        // 칼이 떨어질 위치를 미리 생성하여 경고 표시
        Vector3[] attackPositions = new Vector3[bladeCount];
        for (int i = 0; i < bladeCount; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(0f, radius);
            Vector3 randomPos = targetPosition + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * distance, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * distance);
            attackPositions[i] = new Vector3(randomPos.x, 1.5f, randomPos.z);


            // 원형 범위 경고 이펙트 생성
            if (circleShapeRange != null)
            {
                GameObject warningCircle = Instantiate(circleShapeRange, attackPositions[i], Quaternion.identity);
                warningCircle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                warningCircle.transform.rotation = Quaternion.Euler(-90f, 90f, 0);

                SphereCollider collider = warningCircle.GetComponent<SphereCollider>();
                if (collider == null)
                {
                    collider = warningCircle.AddComponent<SphereCollider>();
                }
                collider.radius = warningCircle.transform.localScale.x / 2f;
                collider.isTrigger = true;

                warningCircle.SetActive(true);
                warningCircles.Add(warningCircle);
            }
        }

        yield return new WaitForSeconds(warningDuration); // 경고 표시 후 1초 대기

        // 오브젝트 풀링 사용하여 칼 생성
        for (int i = 0; i < bladeCount; i++)
        {
            Vector3 spawnPosition = new Vector3(attackPositions[i].x, 12f, attackPositions[i].z);
            GameObject blade = bladePool.GetBlade(); // 풀에서 가져오기
            blade.transform.position = spawnPosition;
            blade.transform.rotation = Quaternion.Euler(180f, 0f, 0f); // X축 180도 회전

            Rigidbody rb = blade.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.down * 5f; // 아래로 떨어지는 속도 적용
            }
            // 칼이 3초 후에 풀로 돌아가도록 설정
            StartCoroutine(ReturnBladeToPool(blade, 3f));
        }

        SEManager.instance.PlaySE("BossFallBlade");

        yield return new WaitForSeconds(1f);



        // 경고 이펙트 제거
        foreach (var circle in warningCircles)
        {
            if (circle != null)
            {
                Destroy(circle);
            }
        }

        isLook = true;
        // nav.isStopped = true;

    }

    IEnumerator Skill_04() 
    {
        tauntVec = targetPosition + lookVec;

        isLook = false;
        // nav.isStopped = false;
        anim.SetTrigger("doTaunt");

        // 무기 충돌 활성화
        if (weaponCollider != null)
            weaponCollider.enabled = true;

        // 부채꼴 범위 표시 활성화
        if (fanShapeRange != null)
        {
            fanShapeRange.SetActive(true);

            yield return new WaitForSeconds(1.5f);
            SEManager.instance.PlaySE("BossHit");
            yield return new WaitForSeconds(0.3f);
            SEManager.instance.PlaySE("BossHit");
            yield return new WaitForSeconds(0.3f);
            SEManager.instance.PlaySE("BossHit");

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

        yield return new WaitForSeconds(3f);

        isLook = true;
        // nav.isStopped = true;

        // 부채꼴 범위 표시 비활성화
        if (fanShapeRange != null)
        {
            fanShapeRange.SetActive(false);
        }

        // 무기 충돌 비활성화
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }

    IEnumerator Skill_05() 
    {
        AttackVec = targetPosition + lookVec;

        isLook = false;
        // nav.isStopped = false;
        anim.SetTrigger("Attack4");

        yield return new WaitForSeconds(0.3f);
        SEManager.instance.PlaySE("BossSwingCircle");

        // Effect 오브젝트 활성화
        if (effectObject != null)
        {
            effectObject.SetActive(true);
        }


        // 사각형 범위 표시 활성화
        if (circleShapeRange != null)
        {

            Debug.Log("원 공격이다");
            circleShapeRange.SetActive(true);

            // 원형  범위의 위치를 보스 위치로 설정
            circleShapeRange.transform.position = transform.position + (transform.forward * 15f);
            // 범위 크기 조정
            circleShapeRange.transform.localScale = new Vector3(4f, 4f, 5f); // 적절한 크기로 조정

            Vector3 direction = transform.forward;  // 보스의 방향

            // 부채꼴이 보스 앞에 위치하고 범위가 펼쳐지도록 회전 조정
            circleShapeRange.transform.rotation = Quaternion.LookRotation(direction);

            circleShapeRange.transform.rotation = Quaternion.Euler(-90f, 90f, 0f); ;  // X축을 90도 회전시켜 수평으로 맞춤

            circleCollider.radius = circleShapeRange.transform.localScale.x / 2f;
        }

        yield return new WaitForSeconds(2f);

        isLook = true;
        // nav.isStopped = true;

        // 부채꼴 범위 표시 비활성화
        if (circleShapeRange != null)
        {
            circleShapeRange.SetActive(false);
        }

        // Effect 오브젝트 비활성화
        if (effectObject != null)
        {
            effectObject.SetActive(false);
        }
    }



    IEnumerator Attack1()
    {
        AttackVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        anim.SetTrigger("Attack1");

        yield return new WaitForSeconds(0.3f);
        SEManager.instance.PlaySE("BossHit");

        // 무기 충돌 활성화
        if (weaponCollider != null)
            weaponCollider.enabled = true;

        // 사각형 범위 표시 활성화
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

        yield return new WaitForSeconds(2f);

        isLook = true;
        nav.isStopped = true;

        // 부채꼴 범위 표시 비활성화
        if (squareShapeRange != null)
        {
            squareShapeRange.SetActive(false);
        }

        // 무기 충돌 비활성화
        if (weaponCollider != null)
            weaponCollider.enabled = false;


    }


    IEnumerator Attack2()
    {
        AttackVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        anim.SetTrigger("Attack2");
        yield return new WaitForSeconds(0.3f);
        SEManager.instance.PlaySE("BossSwing");

        // Effect 오브젝트 활성화
        if (effectObject != null)
        {
            effectObject.SetActive(true);
        }

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

        yield return new WaitForSeconds(2f);

        isLook = true;
        nav.isStopped = true;

        // 부채꼴 범위 표시 비활성화
        if (fanShapeRange != null)
        {
            fanShapeRange.SetActive(false);
        }

        // Effect 오브젝트 비활성화
        if (effectObject != null)
        {
            effectObject.SetActive(false);
        }
    }

    IEnumerator Attack3()
    {
        isLook = false;
        nav.isStopped = false;
        anim.SetTrigger("Attack3"); // 보스 공격 애니메이션 실행

        yield return new WaitForSeconds(0.2f);

        int bladeCount = 50; // 떨어지는 칼 개수
        float radius = 50f; // 칼이 떨어지는 범위 반경
        float warningDuration = 1f; // 경고 범위 표시 시간

        List<GameObject> warningCircles = new List<GameObject>(); // 개별 원형 범위 저장 리스트

        // 칼이 떨어질 위치를 미리 생성하여 경고 표시
        Vector3[] attackPositions = new Vector3[bladeCount];
        for (int i = 0; i < bladeCount; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(0f, radius);
            Vector3 randomPos = target.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * distance, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * distance);
            attackPositions[i] = new Vector3(randomPos.x, 1.5f, randomPos.z);


            // 원형 범위 경고 이펙트 생성
            if (circleShapeRange != null)
            {
                GameObject warningCircle = Instantiate(circleShapeRange, attackPositions[i], Quaternion.identity);
                warningCircle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                warningCircle.transform.rotation = Quaternion.Euler(-90f, 90f, 0);

                SphereCollider collider = warningCircle.GetComponent<SphereCollider>();
                if (collider == null)
                {
                    collider = warningCircle.AddComponent<SphereCollider>();
                }
                collider.radius = warningCircle.transform.localScale.x / 2f;
                collider.isTrigger = true;

                warningCircle.SetActive(true);
                warningCircles.Add(warningCircle);
            }
        }

        yield return new WaitForSeconds(warningDuration); // 경고 표시 후 1초 대기

        // 오브젝트 풀링 사용하여 칼 생성
        for (int i = 0; i < bladeCount; i++)
        {
            Vector3 spawnPosition = new Vector3(attackPositions[i].x, 12f, attackPositions[i].z);
            GameObject blade = bladePool.GetBlade(); // 풀에서 가져오기
            blade.transform.position = spawnPosition;
            blade.transform.rotation = Quaternion.Euler(180f, 0f, 0f); // X축 180도 회전

            Rigidbody rb = blade.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.down * 5f; // 아래로 떨어지는 속도 적용
            }
            // 칼이 3초 후에 풀로 돌아가도록 설정
            StartCoroutine(ReturnBladeToPool(blade, 3f));
        }

        SEManager.instance.PlaySE("BossFallBlade");

        yield return new WaitForSeconds(1f);

        

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
        LoopSEManager.instance.LoopPlaySE("BossWalk");
        // stoppingDistance 값을 플레이어와의 거리보다 작게 설정하여, 도달 후 바로 멈추도록 설정
        nav.stoppingDistance = 15f;

        // acceleration과 deceleration 값 조정하여 더 자연스러운 멈춤 구현
        nav.acceleration = 8f;  // 보스의 가속도를 적당히 설정

        if (Vector3.Distance(transform.position, target.position) > 15f)
        {
            anim.SetBool("isWalk", true);  // 걷는 애니메이션 시작
            //SEManager.instance.LoopPlaySE("BossWalk");
        }

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
        LoopSEManager.instance.StopSE("BossWalk");
        nav.isStopped = true;  // 목적지 도달 후 멈추기
    }

    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        anim.SetTrigger("doTaunt");

        // 무기 충돌 활성화
        if (weaponCollider != null)
            weaponCollider.enabled = true;

        // 부채꼴 범위 표시 활성화
        if (fanShapeRange != null)
        {
            fanShapeRange.SetActive(true);

            yield return new WaitForSeconds(1.5f);
            SEManager.instance.PlaySE("BossHit");
            yield return new WaitForSeconds(0.3f);
            SEManager.instance.PlaySE("BossHit");
            yield return new WaitForSeconds(0.3f);
            SEManager.instance.PlaySE("BossHit");

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

        yield return new WaitForSeconds(3f);

        isLook = true;
        nav.isStopped = true;

        // 부채꼴 범위 표시 비활성화
        if (fanShapeRange != null)
        {
            fanShapeRange.SetActive(false);
        }

        // 무기 충돌 비활성화
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }

    IEnumerator Attack4()
    {
        AttackVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        anim.SetTrigger("Attack4");

        yield return new WaitForSeconds(0.3f);
        SEManager.instance.PlaySE("BossSwingCircle");

        // Effect 오브젝트 활성화
        if (effectObject != null)
        {
            effectObject.SetActive(true);
        }


        // 사각형 범위 표시 활성화
        if (circleShapeRange != null)
        {

            Debug.Log("원 공격이다");
            circleShapeRange.SetActive(true);

            // 원형  범위의 위치를 보스 위치로 설정
            circleShapeRange.transform.position = transform.position + (transform.forward * 15f);
            // 범위 크기 조정
            circleShapeRange.transform.localScale = new Vector3(4f, 4f, 5f); // 적절한 크기로 조정

            Vector3 direction = transform.forward;  // 보스의 방향

            // 부채꼴이 보스 앞에 위치하고 범위가 펼쳐지도록 회전 조정
            circleShapeRange.transform.rotation = Quaternion.LookRotation(direction);

            circleShapeRange.transform.rotation = Quaternion.Euler(-90f, 90f, 0f); ;  // X축을 90도 회전시켜 수평으로 맞춤

            circleCollider.radius = circleShapeRange.transform.localScale.x / 2f;
        }

        yield return new WaitForSeconds(2f);

        isLook = true;
        nav.isStopped = true;

        // 부채꼴 범위 표시 비활성화
        if (circleShapeRange != null)
        {
            circleShapeRange.SetActive(false);
        }

        // Effect 오브젝트 비활성화
        if (effectObject != null)
        {
            effectObject.SetActive(false);
        }
    }
}
