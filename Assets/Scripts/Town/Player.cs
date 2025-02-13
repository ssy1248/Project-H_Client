using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using System;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private UINameChat uiNameChat;

    [Header("Movement Settings")]
    public float SmoothMoveSpeed = 10f; // 위치 보간 속도
    public float SmoothRotateSpeed = 10f; // 회전 보간 속도
    public float TeleportDistanceThreshold = 0.5f; // 순간 이동 거리 임계값

    public Avatar Avatar { get; private set; }
    public MyPlayer MPlayer { get; private set; }

    private string nickname;
    private UIChat uiChat;

    private Vector3 goalPos;
    private Quaternion goalRot;

    private Animator animator;

    public int PlayerId { get; private set; }
    public bool IsMine { get; private set; }
    private bool isInitialized = false;

    private Vector3 lastPos;

    private long EstimatedArrivalTime; 



    
    private void Start()
    {
        Avatar = GetComponent<Avatar>();
        animator = GetComponent<Animator>();
    }

    public void SetPlayerId(int playerId)
    {
        PlayerId = playerId;
    }

    public void SetNickname(string nickname)
    {
        this.nickname = nickname;
        uiNameChat.SetName(nickname);
    }

    public void SetIsMine(bool isMine)
    {
        IsMine = isMine;

        if (IsMine)
        {
            MPlayer = gameObject.AddComponent<MyPlayer>();
        }
        else
        {
            Destroy(GetComponent<NavMeshAgent>());
        }

        uiChat = TownManager.Instance.UiChat;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (!IsMine)
        {
            SmoothMoveAndRotate();
        }

        CheckMove();
    }

    private void SmoothMoveAndRotate()
    {   
        // 개선해야할점
        // 
        MoveSmoothly();
        RotateSmoothly();
    }

    private void MoveSmoothly()
{
    // 도착시간 계산.
    long now = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
    int deltaTime = (int)(EstimatedArrivalTime - now);

    if (deltaTime <= 0) 
    {
        // 이미 목표 시간 지났으면 즉시 위치 업데이트
        transform.localPosition = goalPos;
        return;
    }

    // 현재 트랜스폼 좌표랑 도착좌표 거리 계산.
    float distance = Vector3.Distance(transform.localPosition, goalPos);

    // 이동해야 할 시간 동안 예상 이동 속도 (거리 / 남은 시간)
    float timeToReachGoal = Mathf.Max(deltaTime / 1000f, 0.1f); // 1000f는 ms를 초로 변환
    float speed = distance / timeToReachGoal;

    if (distance > TeleportDistanceThreshold)
    {
        // 순간이동이면 바로 넣기. 
        transform.localPosition = goalPos; 
    }
    else
    {
        // 예측된 위치 계산 (이동 속도에 맞춰 보간)
        float step = speed * Time.deltaTime;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, goalPos, step);
    }
}

private void RotateSmoothly()
{
    if (goalRot != Quaternion.identity)
    {
        // 현재 시간과 예상 도착 시간의 차이 계산
        long now = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
        int deltaTime = (int)(EstimatedArrivalTime - now);

        if (deltaTime <= 0)
        {
            // 목표 시간 이미 지났으면 바로 회전
            transform.rotation = goalRot;
            return;
        }

        // 회전이 완료될 때까지 걸리는 시간 (초 단위)
        float timeToReachGoal = Mathf.Max(deltaTime / 1000f, 0.1f); // 1000f는 ms -> 초로 변환

        // 회전 속도 계산 (회전값 / 예상 시간)
        float rotationSpeed = Vector3.Angle(transform.forward, goalRot * Vector3.forward) / timeToReachGoal;

        // 회전 보간을 통해 목표 회전값으로 부드럽게 회전
        float step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, goalRot, step);
    }
}


    public void SendMessage(string msg)
    {
        if (!IsMine) 
            return;

        var chatPacket = new C_Chat
        {
            PlayerId = PlayerId,
            SenderName = nickname,
            ChatMsg = msg
        };

        GameManager.Network.Send(chatPacket);
    }

    public void RecvMessage(string msg)
    {
        uiNameChat.PushText(msg);
        uiChat.PushMessage(nickname, msg, IsMine);
    }

    public void Move(Vector3 move, Quaternion rot, long estimatedArrivalTime)
    {
        goalPos = move;
        goalRot = rot;
        EstimatedArrivalTime = estimatedArrivalTime;
    }

    public void PlayAnimation(int animCode)
    {
        animator?.SetTrigger("Anim"+animCode);
    }

    private void CheckMove()
    {
        float dist = Vector3.Distance(lastPos, transform.position);
        animator.SetFloat(Constants.TownPlayerMove, dist * 100);
        lastPos = transform.position;
    }
}
