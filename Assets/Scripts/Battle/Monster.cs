using Cinemachine.Utility;
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

    // ������ ID, HP, ��ǥ
    public string MonsterId { get; private set; }
    public float MonsterHp { get; private set; }
    private Vector3 monsterPosition;

    // Ÿ�� ��ǥ
    private Vector3 targetPosition;
    private Quaternion targetRot;
    private float moveSpeed = 4f; // ���� �̵� �ӵ�
    private bool isDestinationReached = true;

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
        navMeshAgent.avoidancePriority = 0;
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        navMeshAgent.acceleration = 100f; // ���ӵ� ���� -> ��� ����
        navMeshAgent.stoppingDistance = 0.1f; // ���ߴ� �Ÿ� ����
        navMeshAgent.autoBraking = true; // �ڵ� �극��ũ Ȱ��ȭ
        navMeshAgent.angularSpeed = 1000f; // ȸ�� �ӵ� ����



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
        //MoveTowardsTarget();
        //Debug.Log($"���� ���� ��ǥ {targetRot}");
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
            // ���� �Ÿ��� ���� �Ÿ����� ũ�� �̵�
            //if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            //{
            //    navMeshAgent.SetDestination(targetPosition);

            //}

            navMeshAgent.SetDestination(targetPosition);
            isDestinationReached = false;
            // ���� ȸ�� ����.
            // .. 

            // NavMeshAgent�� ȸ���� �ڵ����� ���� �ʵ��� ����
            navMeshAgent.updateRotation = false;

           

        }
    }

    // ���� ���� ������Ʈ (�� ������ ȣ��)
    private void Update()
    {
        if (!isDestinationReached &&  navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && navMeshAgent.velocity.sqrMagnitude == 0)
        {
           

            // ������ �� ���� ����
            isDestinationReached = true;
            //Debug.Log($"���̵�: {MonsterId} / ��ǥ: {transform.position}");

            SendMovePacket(MonsterId, transform.position);
        }


        if (IsVector3NaN(targetPosition))
        {
            return;
        }
        if (targetPosition != null )
        {
            // ���� ��ġ�� ��ǥ ��ġ ���� ���� ���� ���
            Vector3 directionToTarget2 = targetPosition - transform.position;
            directionToTarget2.y = 0;  // ���� �̵��� ����

            // ��ǥ ��ġ���� �̵��� �Ÿ� ���
            float distanceToTarget = directionToTarget2.magnitude;

            // ���� ��ǥ ��ġ������ �Ÿ��� ���� ���� ���϶�� �̵� ����
            if (distanceToTarget < 0.1f)
            {
                transform.position = targetPosition;  // ��ǥ ��ġ�� ��Ȯ�� �̵�
                return;
            }

            // �̵� �ӵ� ���� (��: 4 units per second)
            float moveSpeed = 4f;
            Vector3 moveDirection = directionToTarget2.normalized;  // �̵� ���� ���͸� ����ȭ

            // ���� �̵�: ���� ��ġ���� ��ǥ ��ġ�� �ε巴�� �̵�
            Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

            // ���ο� ��ġ�� �̵�
            transform.position = newPosition;

            
        }
        // NaN  체크용도
        bool IsVector3NaN(Vector3 vec)
        {
            return float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z);
        }


        // ȸ�� ��ǥ�� ����Ǿ��� ���� ȸ���� �����ϵ��� ó��
        if (targetRot != null)  // targetRot�� null�� �ƴ��� Ȯ��
        {
            // 1. Ÿ�� ��ġ�κ��� ���� ���͸� ���
            Vector3 directionToTarget = targetPosition - transform.position;  // Ÿ�� ���� ����
            directionToTarget.y = 0;  // Y�����δ� ȸ������ �ʵ��� ���� (���� ȸ���� �ϵ���)

            // ���� ���Ͱ� (0, 0, 0)�� ��� ȸ������ �ʵ��� ó��
            if (directionToTarget.sqrMagnitude > Mathf.Epsilon)  // ���Ͱ� (0, 0, 0)�� �ƴ� ��쿡�� ó��
            {
                // 2. ���� ���͸� �������Ѽ� ���Ͱ� ������ �ѵ��� ����
                directionToTarget = -directionToTarget;

                // 3. ������ �������� ȸ�� �� ���
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);  // ������ ������ ���� ȸ�� �� ���

                // 4. �ε巴�� ȸ�� ����
                if (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)  // ȸ�� ���� ���̰� 0.5�� �̻��� ��쿡�� ȸ��
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);  // �ε巴�� ȸ��
                }
            }

            // NavMeshAgent�� ȸ���� �������� ó�������Ƿ�, �ڵ� ȸ���� ����
            navMeshAgent.updateRotation = false;
        }
         

        // ���÷�, HP�� 0 ���ϰ� �ƴϸ� ��� Ÿ���� �����ϵ��� ����
        if (MonsterHp <= 0)
        {
            Die(); // HP�� 0 ������ �� ���� ����
        }

    }


    private void SendMovePacket(string id, Vector3 position)
    {
        // ���콺 Ŭ�� ��ǥ�� ��ǥ �������� ���
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
            //Debug.Log($"공격 좌표 : {transform.position}");

            animator.ResetTrigger(Constants.MonsterAttack1);
            animator.SetTrigger(Constants.MonsterAttack1);
        } else if(id == "Die")
        {
            animator.SetTrigger(Constants.MonsterDie);
            StartCoroutine(WaitForAnimationEnd("Die"));
        } else if (id == "Hit")
        {
            //Debug.Log($"들어왔어요.!  id : {id}");
            animator.ResetTrigger(Constants.MonsterHit);
            animator.SetTrigger(Constants.MonsterHit);
        }
        //Debug.Log($"id : {id}");
    }

    private IEnumerator WaitForAnimationEnd(string stateName)
    {
        yield return new WaitForEndOfFrame(); // �� ������ ��� (�ִϸ��̼��� ���۵ǵ���)

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //// �ִϸ��̼��� ���� ������ ���
        //while (!(stateInfo.IsName(stateName) && (stateInfo.normalizedTime % 1) >= 0.90f))
        //{
        //    //Debug.Log($" ���� �ִϸ��̼�: {stateInfo.fullPathHash}, ��ǥ: {stateName}, ���൵: {stateInfo.normalizedTime}");
        //    yield return null;
        //    stateInfo = animator.GetCurrentAnimatorStateInfo(0); // ���� ���� ������Ʈ
        //}

        yield return new WaitForSeconds(2f); // �ִϸ��̼��� ������ ����� �� ��� ���

        // �ִϸ��̼� ���� �� ������ ����
        OnMonsterDeath();
    }

    private void OnMonsterDeath()
    {
        //if (this == null) return; // �̹� ������ ��� ���� �� ��
        Destroy(gameObject);
    }
}
