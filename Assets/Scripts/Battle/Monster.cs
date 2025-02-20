using System;
using System.Collections;
using System.Collections.Generic;
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
            navMeshAgent.SetDestination(targetPosition);
            // 남은 거리가 멈출 거리보다 크면 이동
            //if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            //{
            //    navMeshAgent.SetDestination(targetPosition);

            //}
        }
    }

    // 몬스터 상태 업데이트 (매 프레임 호출)
    private void Update()
    {


        // 예시로, HP가 0 이하가 아니면 계속 타겟을 추적하도록 설정
        if (MonsterHp <= 0)
        {
            Die(); // HP가 0 이하일 때 몬스터 죽음
        }
    }
}
