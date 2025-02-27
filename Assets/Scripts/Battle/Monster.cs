using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [SerializeField] private UIMonsterInformation uiMonsterInfo;

    // 몬스터의 ID, HP, 좌표
    public string MonsterId { get; private set; }
    public float MonsterHp { get; private set; }
    private Vector3 monsterPosition;

    // 타겟 좌표
    private Vector3 targetPosition;
    private Quaternion targetRot;
    private float moveSpeed = 4f; // 몬스터 이동 속도
    private bool isDestinationReached = true;

    // 네브 메쉬 추가. 
    private NavMeshAgent navMeshAgent;

    private Animator animator;

    // 애니메이션 코드 리스트
    private int[] animCodeList = new[]
    {
        Constants.MonsterAttack1,
        Constants.MonsterAttack2,
        Constants.MonsterTaunting,
        Constants.MonsterVictory,
        Constants.MonsterDie,
        Constants.MonsterHit
    };

    public UIMonsterInformation UiMonsterInfo => uiMonsterInfo;

    private void Awake()
    {
        // 애니메이션
        animator = GetComponentInChildren<Animator>();

        // 네브메쉬 추가
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.avoidancePriority = 0;
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        navMeshAgent.acceleration = 100f; // 가속도 증가 -> 즉시 반응
        navMeshAgent.stoppingDistance = 0.1f; // 멈추는 거리 조절
        navMeshAgent.autoBraking = true; // 자동 브레이크 활성화
        navMeshAgent.angularSpeed = 1000f; // 회전 속도 증가



    }

    // 몬스터의 ID, HP, 좌표, 타겟 좌표를 설정하는 메서드
    public void Initialize(string id, string name, float hp, Vector3 spawnPosition, Quaternion rot)
    {
        MonsterId = id;
        MonsterHp = hp;
        monsterPosition = spawnPosition;
        targetPosition = spawnPosition;
        targetRot = rot;

        // UI 업데이트
        uiMonsterInfo.SetName("Monster " + name);
        uiMonsterInfo.SetFullHP(MonsterHp);
    }

    // 애니메이션 설정
    public void SetAnim(int idx)
    {
        if (idx < 0 || idx >= animCodeList.Length)
            return;

        var animCode = animCodeList[idx];
        animator.SetTrigger(animCode);
    }

    // 몬스터가 피해를 입었을 때 호출되는 메서드
    public void Hit(float damage)
    {
        MonsterHp -= damage;
        animator.SetTrigger(Constants.MonsterHit);

        // HP가 0 이하일 때 몬스터 삭제
        if (MonsterHp <= 0)
        {
            Die();
        }
    }

    // 몬스터 죽음 처리
    private void Die()
    {
        SetAnim(4); // Die 애니메이션 재생
        Destroy(gameObject, 2f); // 2초 뒤에 객체를 삭제
    }

    // 몬스터의 위치를 업데이트
    public void UpdateMonsterPosition(Vector3 move, Quaternion rot, float speed, float hp)
    {
        targetPosition = move;
        targetRot = rot;
        moveSpeed = speed;
        MonsterHp = hp;
        //이동
        MoveTowardsTarget();
        Debug.Log($"받은 몬스터 좌표 {targetRot}");
    }

    // 타겟 위치로 이동하는 로직 (몬스터가 타겟을 추적)
    private void MoveTowardsTarget()
    {
        if (navMeshAgent != null)
        {
            //// 몬스터 위치와 목표 위치를 가져옵니다.
            //Vector3 currentPosition = transform.position;
            //Vector3 targetPosition = this.targetPosition;

            //// 보간 이동: 현재 위치에서 목표 위치로 부드럽게 이동
            //float moveSpeed = 4f;  // 이동 속도 조절 가능
            //float step = moveSpeed * Time.deltaTime; // 프레임마다 이동하는 거리 계산

            //// 목표 위치로 이동
            //transform.position = Vector3.Lerp(currentPosition, targetPosition, step);
            //Debug.Log($"[몬스터 업데이트] 현재 포지션 {currentPosition} ..");
            //Debug.Log($"[몬스터 업데이트] 현재 포지션 {targetPosition} ..");

            //Debug.Log($"[몬스터 업데이트] 포지션 {targetPosition} 몬스터가 업데이트 되었습니다..");
            // 남은 거리가 멈출 거리보다 크면 이동
            //if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            //{
            //    navMeshAgent.SetDestination(targetPosition);

            //}

            navMeshAgent.SetDestination(targetPosition);
            isDestinationReached = false;
            // 받은 회전 적용.
            // .. 

            // NavMeshAgent가 회전을 자동으로 하지 않도록 설정
            navMeshAgent.updateRotation = false;

           

        }
    }

    // 몬스터 상태 업데이트 (매 프레임 호출)
    private void Update()
    {
        if (!isDestinationReached &&  navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && navMeshAgent.velocity.sqrMagnitude == 0)
        {
           

            // 도달한 후 상태 변경
            isDestinationReached = true;
            Debug.Log($"아이디: {MonsterId} / 좌표: {transform.position}");

            SendMovePacket(MonsterId, transform.position);
        }



        //if(targetPosition != null)
        //{
        //    // 현재 위치와 목표 위치 간의 방향 벡터 계산
        //    Vector3 directionToTarget2 = targetPosition - transform.position;
        //    directionToTarget2.y = 0;  // 수평 이동만 고려

        //    // 목표 위치까지 이동할 거리 계산
        //    float distanceToTarget = directionToTarget2.magnitude;

        //    // 만약 목표 위치까지의 거리가 일정 범위 이하라면 이동 멈춤
        //    if (distanceToTarget < 0.1f)
        //    {
        //        transform.position = targetPosition;  // 목표 위치로 정확히 이동
        //        return;
        //    }

        //    // 이동 속도 설정 (예: 4 units per second)
        //    float moveSpeed = 4f;
        //    Vector3 moveDirection = directionToTarget2.normalized;  // 이동 방향 벡터를 정규화

        //    // 보간 이동: 이전 위치에서 목표 위치로 부드럽게 이동
        //    Vector3 newPosition = Vector3.Lerp(lastPosition, targetPosition, Time.deltaTime * moveSpeed);

        //    // 새로운 위치로 이동
        //    transform.position = newPosition;

        //    // 이전 위치를 갱신
        //    lastPosition = transform.position;
        //}


        // 회전 목표가 변경되었을 때만 회전을 시작하도록 처리
        if (targetRot != null)  // targetRot이 null이 아닌지 확인
        {
            // 1. 타겟 위치로부터 방향 벡터를 계산
            Vector3 directionToTarget = targetPosition - transform.position;  // 타겟 방향 벡터
            directionToTarget.y = 0;  // Y축으로는 회전하지 않도록 설정 (수평 회전만 하도록)

            // 방향 벡터가 (0, 0, 0)일 경우 회전하지 않도록 처리
            if (directionToTarget.sqrMagnitude > Mathf.Epsilon)  // 벡터가 (0, 0, 0)이 아닌 경우에만 처리
            {
                // 2. 방향 벡터를 반전시켜서 몬스터가 등으로 쫓도록 설정
                directionToTarget = -directionToTarget;

                // 3. 반전된 방향으로 회전 값 계산
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);  // 반전된 방향을 향한 회전 값 계산

                // 4. 부드럽게 회전 적용
                if (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)  // 회전 각도 차이가 0.5도 이상일 경우에만 회전
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);  // 부드럽게 회전
                }
            }

            // NavMeshAgent의 회전은 수동으로 처리했으므로, 자동 회전은 끄기
            navMeshAgent.updateRotation = false;
        }
         

        // 예시로, HP가 0 이하가 아니면 계속 타겟을 추적하도록 설정
        if (MonsterHp <= 0)
        {
            Die(); // HP가 0 이하일 때 몬스터 죽음
        }

    }


    private void SendMovePacket(string id, Vector3 position)
    {
        // 마우스 클릭 좌표를 목표 지점으로 계산
        string monsterId = id;

      
        var tr = new TransformInfo
        {
            PosX = position.x,
            PosY = position.y,
            PosZ = position.z,
            Rot = 1
        };


        var monsterMovePacket = new C_MonsterMove
        {
            MonstId = monsterId, 
            TransformInfo = tr, 
        };

        GameManager.Network.Send(monsterMovePacket);

    }

    public void switchAnimation(string id)
    {
        if(id == "Attck")
        {
            animator.ResetTrigger(Constants.MonsterAttack1);
            animator.SetTrigger(Constants.MonsterAttack1);
        } else if(id == "Die")
        {
            animator.SetTrigger(Constants.MonsterDie);
        }

    }
}
