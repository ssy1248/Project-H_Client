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
    public float TeleportDistanceThreshold = 10f; // 순간 이동 거리 임계값 (0.5)

    public Avatar Avatar { get; private set; }
    public MyPlayer MPlayer { get; private set; }

    public string nickname;
    private UIChat uiChat;

    private Vector3 goalPos;
    private Quaternion goalRot;

    private Animator animator;

    public int PlayerId { get; private set; }
    public bool IsMine { get; private set; }
    private bool isInitialized = false;

    private Vector3 lastPos;

    private float agentSpeed; 

    private void Start()
    {
        Avatar = GetComponent<Avatar>();
        animator = GetComponent<Animator>();
    }

    public void Despawn()
    {
        Destroy(gameObject);  // 게임 오브젝트 제거
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
        // 목표 위치까지 부드럽게 이동
        transform.position = Vector3.MoveTowards(transform.position, goalPos, agentSpeed * Time.deltaTime);
        Debug.Log(transform.position);
    }

    private void RotateSmoothly()
    {
        if (goalRot != Quaternion.identity)
        {
            float t = Mathf.Clamp(Time.deltaTime * SmoothRotateSpeed, 0, 0.99f);
            transform.rotation = Quaternion.Lerp(transform.rotation, goalRot, t);
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

    public void RecvMessage(string msg, UIChat.ChatType type)
    {
        uiNameChat.PushText(msg);
        uiChat.PushMessage(msg, IsMine, type);
    }

    public void Move(Vector3 move, Quaternion rot, float speed )
    {
        goalPos = move;
        goalRot = rot;
        agentSpeed = speed;
        Debug.Log(goalPos);
        animator.SetBool("isRun", true);
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
