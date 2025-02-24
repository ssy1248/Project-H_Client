using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : Enemy
{
    Vector3 lookVec;
    Vector3 tauntVec;
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
            yield return new WaitForSeconds(3f);  // 일정 시간 간격으로 행동을 바꿈

            // 패턴 활성화
            isPatternActive = true;
            int randomAction = Random.Range(0, 4);
            switch (randomAction)
            {
                case 0:
                case 1:
                case 2:
                    StartCoroutine(Taunt());
                    break;

                case 3:
                    StartCoroutine(Walk());
                    break;

                case 4:
                    break;
            }

            yield return new WaitForSeconds(2f);  // 패턴 후 대기
            isPatternActive = false;  // 패턴 종료 후 이동 가능 상태로 변경
        }
    }

    // 보스가 타겟에게 도달하는 이동 패턴
    IEnumerator Walk()
    {
        nav.isStopped = false;
        nav.SetDestination(target.position);  // 타겟 위치로 이동
        anim.SetBool("isWalk", true);  // 걷는 애니메이션 재생

        while (Vector3.Distance(transform.position, target.position) > 2f)  // 타겟과 가까워질 때까지 계속 이동
        {
            yield return null;
        }

        anim.SetBool("isWalk", false);  // 이동 완료 후 애니메이션 멈춤
    }

    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = false;
        boxCollider.enabled = true;
        StartCoroutine(Think());
    }
}
