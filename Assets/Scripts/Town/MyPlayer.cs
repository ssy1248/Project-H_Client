
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.AI.Navigation;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using Unity.Burst.CompilerServices;

public class MyPlayer : MonoBehaviour
{
    // 플레이어 정보.
    private Player player;

    [SerializeField] private NavMeshAgent agent;
    private RaycastHit rayHit;
    private EventSystem eSystem;
    private Animator animator;
    private Vector3 lastPos;

    private Vector3 targetPosition;
    private Quaternion targetRot;
    private float moveSpeed = 4f;
    private float smoothRotateSpeed = 3.0f;

    public Vector3 MousePos { get; set; }


    private readonly List<int> animHash = new List<int>();


    void Awake()
    {
        if (TownManager.Instance != null)
        {
            eSystem = TownManager.Instance.E_System;
        }
        if (DungeonManager.Instance != null)
        {
            eSystem = DungeonManager.Instance.E_System;
        }
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = 0;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.acceleration = 100f; // ���ӵ� ���� -> ��� ����
        agent.stoppingDistance = 0.1f; // ���ߴ� �Ÿ� ����
        agent.autoBraking = true; // �ڵ� �극��ũ Ȱ��ȭ
        agent.angularSpeed = 1000f; // ȸ�� �ӵ� ����
        agent.updateRotation = false;

        animator = GetComponent<Animator>();

        InitializeCamera();

        lastPos = transform.position;
        targetPosition = transform.position;
        targetRot = transform.rotation;

        LoadAnimationHashes();

        player = GetComponent<Player>(); // 같은 GameObject에 있는 Player 컴포넌트 가져오기
    }

    void Update()
    {
        HandleInput();
        test();
        //CheckMove();

    }

    // test
    public void UpdateUserPosition(Vector3 move, Quaternion rot, float speed)
    {
        targetPosition = move;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            targetPosition = new Vector3(targetPosition.x, hit.position.y, targetPosition.z);
        }

        targetRot = rot;
        moveSpeed = speed;

        //lastPos = targetPosition;
        //Debug.Log($" targetPosition : {targetPosition}");
    }

    //test
    private void test()
    {
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
            transform.position += dir * Time.deltaTime * moveSpeed;
            animator.SetBool("isRun", true);
        }
        else
        {
            // 목표에 도달했을 때 멈추는 로직
            transform.position = targetPosition; // 목표 위치에 정확히 도달
            animator.SetBool("isRun", false);
        }

        agent.updateRotation = false;

        
    }


    private void InitializeCamera()
    {
        CinemachineFreeLook freeLook =null;

        if (TownManager.Instance != null)
        {
             freeLook = TownManager.Instance.FreeLook;
        }
        if (DungeonManager.Instance != null)
        {
             freeLook = DungeonManager.Instance.FreeLook;
        }
        freeLook.Follow = transform;
        freeLook.LookAt = transform;
        freeLook.gameObject.SetActive(true);
    }

    private void LoadAnimationHashes()
    {
        animHash.Add(Constants.TownPlayerAnim1);
        animHash.Add(Constants.TownPlayerAnim2);
        animHash.Add(Constants.TownPlayerAnim3);
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(1) && !eSystem.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity); // 모든 충돌 감지

            Vector3 hitPosition = Vector3.zero;
            bool foundGround = false;

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("BossMonster")) continue; // 보스 몬스터 무시
                hitPosition = hit.point;
                foundGround = true;
                break; // 첫 번째로 감지된 땅만 사용
            }

            if (!foundGround) return; // 땅을 못 찾았으면 종료

            // 클릭한 지점이 NavMesh 위에 있는지 확인
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(hitPosition, out navHit, 0.5f, NavMesh.AllAreas))
            {
                MousePos = navHit.position;

                // 방향 + 속도 velocity 구하는 로직.
                Vector3 directionToGoal = (navHit.position - transform.position).normalized;
                float speed = agent.speed;
                Vector3 velocity = directionToGoal * speed;

                SendMovePacket(navHit.position, velocity, moveSpeed, moveSpeed);
            }
        }
    }

    public void ExecuteAnimation(int animIdx)
    {
        if (animIdx < 0 || animIdx >= animHash.Count) 
            return;

        int animKey = animHash[animIdx];
        agent.SetDestination(transform.position);

        TownManager.Instance.Animation(animIdx);
    }


    private void CheckMove()
    {
        // 현재 위치와 마지막 위치 사이의 거리 계산
        float distanceMoved = Vector3.Distance(lastPos, transform.position);

        // 애니메이터? 관련된 설정?
        animator.SetFloat(Constants.TownPlayerMove, distanceMoved * 100);


        // 현재 위치를 마지막 위치로 저장
        lastPos = transform.position;
    }

   

    private void SendMovePacket(Vector3 position, Vector3 velocity, float speed, float rotationSpeed)
    {
        // 마우스 클릭 좌표를 목표 지점으로 계산
        Vector3 targetPosition = position; // 마우스 클릭 좌표

        // 현재 오브젝트 위치에서 목표 지점까지의 방향을 계산
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0;  // 수평 방향으로만 회전하도록 y 값을 0으로 설정

        // 목표 지점 방향으로 회전 계산 (단위: 라디안 -> 도 단위 변환)
        float rotationY = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;

        // 회전 값이 NaN 또는 이상한 값이라면 초기화
        if (rotationY == 0 || rotationY == 0)
        {
            rotationY = 0.1f;  // 기본 회전 값
        }

        var tr = new TransformInfo
        {
            PosX = position.x,
            PosY = position.y,
            PosZ = position.z,
            Rot = rotationY
        };

        var vel = new Velocity
        {
            X = velocity.x,
            Y = velocity.y,
            Z = velocity.z
        };

        long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        var movePacket = new C_Move
        {
            PlayerId = player.PlayerId,
            Transform = tr,
            Timestamp = timestamp,
            IsMoving = true,
            Velocity = vel,
            Speed = agent.speed,
        };

        GameManager.Network.Send(movePacket);
    }
}