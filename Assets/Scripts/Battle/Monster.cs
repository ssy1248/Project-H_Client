using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [SerializeField] private UIMonsterInformation uiMonsterInfo;

    // ������ ID, HP, ��ǥ
    public string MonsterId { get; private set; }
    public float MonsterHp { get; private set; }
    private Vector3 monsterPosition;

    // Ÿ�� ��ǥ
    private Vector3 targetPosition;
    private Quaternion targetRot;
    private float moveSpeed = 4f; // ���� �̵� �ӵ�

    // �׺� �޽� �߰�. 
    private NavMeshAgent navMeshAgent;

    private Animator animator;

    // �ִϸ��̼� �ڵ� ����Ʈ
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
        // �ִϸ��̼�
        animator = GetComponentInChildren<Animator>();

        // �׺�޽� �߰�
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // ������ ID, HP, ��ǥ, Ÿ�� ��ǥ�� �����ϴ� �޼���
    public void Initialize(string id, string name, float hp, Vector3 spawnPosition, Quaternion rot)
    {
        MonsterId = id;
        MonsterHp = hp;
        monsterPosition = spawnPosition;
        targetPosition = spawnPosition;
        targetRot = rot;

        // UI ������Ʈ
        uiMonsterInfo.SetName("Monster " + name);
        uiMonsterInfo.SetFullHP(MonsterHp);
    }

    // �ִϸ��̼� ����
    public void SetAnim(int idx)
    {
        if (idx < 0 || idx >= animCodeList.Length)
            return;

        var animCode = animCodeList[idx];
        animator.SetTrigger(animCode);
    }

    // ���Ͱ� ���ظ� �Ծ��� �� ȣ��Ǵ� �޼���
    public void Hit(float damage)
    {
        MonsterHp -= damage;
        animator.SetTrigger(Constants.MonsterHit);

        // HP�� 0 ������ �� ���� ����
        if (MonsterHp <= 0)
        {
            Die();
        }
    }

    // ���� ���� ó��
    private void Die()
    {
        SetAnim(4); // Die �ִϸ��̼� ���
        Destroy(gameObject, 2f); // 2�� �ڿ� ��ü�� ����
    }

    // ������ ��ġ�� ������Ʈ
    public void UpdateMonsterPosition(Vector3 move, Quaternion rot, float speed, float hp)
    {
        targetPosition = move;
        targetRot = rot;
        moveSpeed = speed;
        MonsterHp = hp;
        //�̵�
        MoveTowardsTarget();
    }

    // Ÿ�� ��ġ�� �̵��ϴ� ���� (���Ͱ� Ÿ���� ����)
    private void MoveTowardsTarget()
    {
        if (navMeshAgent != null)
        {
            //// ���� ��ġ�� ��ǥ ��ġ�� �����ɴϴ�.
            //Vector3 currentPosition = transform.position;
            //Vector3 targetPosition = this.targetPosition;

            //// ���� �̵�: ���� ��ġ���� ��ǥ ��ġ�� �ε巴�� �̵�
            //float moveSpeed = 4f;  // �̵� �ӵ� ���� ����
            //float step = moveSpeed * Time.deltaTime; // �����Ӹ��� �̵��ϴ� �Ÿ� ���

            //// ��ǥ ��ġ�� �̵�
            //transform.position = Vector3.Lerp(currentPosition, targetPosition, step);
            //Debug.Log($"[���� ������Ʈ] ���� ������ {currentPosition} ..");
            //Debug.Log($"[���� ������Ʈ] ���� ������ {targetPosition} ..");

            //Debug.Log($"[���� ������Ʈ] ������ {targetPosition} ���Ͱ� ������Ʈ �Ǿ����ϴ�..");
            navMeshAgent.SetDestination(targetPosition);
            // ���� �Ÿ��� ���� �Ÿ����� ũ�� �̵�
            //if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            //{
            //    navMeshAgent.SetDestination(targetPosition);

            //}
        }
    }

    // ���� ���� ������Ʈ (�� ������ ȣ��)
    private void Update()
    {


        // ���÷�, HP�� 0 ���ϰ� �ƴϸ� ��� Ÿ���� �����ϵ��� ����
        if (MonsterHp <= 0)
        {
            Die(); // HP�� 0 ������ �� ���� ����
        }
    }
}
