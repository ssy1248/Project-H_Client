
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MyPlayer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private RaycastHit rayHit;
    private EventSystem eSystem;
    private Animator animator;
    private Vector3 lastPos;

    private readonly List<int> animHash = new List<int>();

    // 100ms마다 이동 패킷 전송 
    private float sendMovePacketInterval = 1f; // 100ms
    // 마지막으로 패킷을 전송한 시간
    private float lastSendTime = 0f;

    void Awake()
    {
        eSystem = TownManager.Instance.E_System;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        InitializeCamera();
        lastPos = transform.position;

        LoadAnimationHashes();
    }

    void Update()
    {
        HandleInput();
        CheckMove();
    }

    private void InitializeCamera()
    {
        var freeLook = TownManager.Instance.FreeLook;
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
        if (Input.GetMouseButtonDown(0) && !eSystem.IsPointerOverGameObject())
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit))
            {
                agent.SetDestination(rayHit.point);
            }
        }
    }

    public void ExecuteAnimation(int animIdx)
    {
        if (animIdx < 0 || animIdx >= animHash.Count) 
            return;

        int animKey = animHash[animIdx];
        agent.SetDestination(transform.position);

        TownManager.Instance.Animation(animKey);
    }


    private void CheckMove()
    {
        // 현재 위치와 마지막 위치 사이의 거리 계산
        float distanceMoved = Vector3.Distance(lastPos, transform.position);

        // 애니메이터? 관련된 설정?
        animator.SetFloat(Constants.TownPlayerMove, distanceMoved * 100);

        // 일정 거리 이상 이동했고, 마지막 전송 이후 지정된 시간이 지났다면 패킷 전송
        if (distanceMoved > 0.01f && Time.time - lastSendTime > sendMovePacketInterval)
        {
            // 이동 패킷 전송
            SendMovePacket();
            // 마지막 전송 시간 업데이트
            lastSendTime = Time.time; 
        }

        // 현재 위치를 마지막 위치로 저장
        lastPos = transform.position;
    }

    private void SendMovePacket()
    {
        var tr = new TransformInfo
        {
            PosX = transform.position.x,
            PosY = transform.position.y,
            PosZ = transform.position.z,
            Rot = transform.eulerAngles.y
        };

        var movePacket = new C_Move { Transform = tr };
        GameManager.Network.Send(movePacket);
    }
}