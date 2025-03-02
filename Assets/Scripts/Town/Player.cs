using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Google.Protobuf.Protocol;

public class Player : MonoBehaviour
{
    #region Identification & UI
    [Header("Player Settings")]
    [SerializeField] private UINameChat uiNameChat;


    private UIChat uiChat;

    public string nickname;
    public int PlayerId { get; private set; }
    public bool IsMine { get; private set; }
    public Avatar Avatar { get; private set; }
    public MyPlayer MPlayer { get; private set; }

    private bool isInitialized = false;
    #endregion

    #region Movement & Remote Smoothing
    [Header("Movement Settings")]
    public float SmoothMoveSpeed = 10f;       // 원격 보간 이동 속도
    public float SmoothRotateSpeed = 10f;     // 원격 보간 회전 속도
    public float TeleportDistanceThreshold = 10f; // 순간 이동 거리 임계값

    // 원격 이동용 변수
    public Vector3 goalPos;
    public Quaternion goalRot;
    private float agentSpeed;
    private Vector3 lastPos;
    #endregion

    #region Local Movement & Effects
    [Header("Local Movement Settings")]
    public float moveSpeed = 10f;     // 마우스 클릭 등으로 지정된 이동 속도
    public float speed = 10f;         // 회피 시 배속용 변수
    public float dodgeDistance = 5f;  // 회피 이동 시 적용할 거리 <- 이부분은 고민해야할듯
    private Camera cam;
    protected NavMeshAgent nav;

    [Header("Effects")]
    public List<GameObject> teleportEffects = new List<GameObject>();       // 텔레포트 이펙트 풀
    public List<GameObject> dodgeEffectsArcher = new List<GameObject>();      // 궁수 회피 이펙트 풀
    public List<GameObject> dodgeEffectsRogue = new List<GameObject>();       // 도적 회피 이펙트 풀
    private int effectIndex = 0;
    private int dodgeEffectArcherIndex = 0;
    private int dodgeEffectRogueIndex = 0;
    #endregion

    #region Health & Damage
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    private bool isDamage = false;
    #endregion

    #region Input, Animation & Combat
    // 입력 및 이동 관련 변수
    private float hAxis, vAxis;
    protected bool isMove = false;
    protected bool isDodge = false;
    protected bool isFireReady = true;
    private Vector3 moveVec;
    private Vector3 dodgeVec;

    // 무기 관련
    private Weapon equipWeapon;
    private float fireDelay;
    #endregion

    #region Components
    private Animator animator;
    private MeshRenderer[] meshs;
    protected Rigidbody rigid;
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        cam = Camera.main;
        nav = GetComponent<NavMeshAgent>();
        if (nav != null)
            nav.updateRotation = false;
        currentHealth = maxHealth;
        lastPos = transform.position;
    }

    void Start()
    {
        Avatar = GetComponent<Avatar>();

        // 자식 오브젝트에 무기가 있다면 장착
        Weapon weapon = GetComponentInChildren<Weapon>();
        if (weapon != null)
            EquipWeapon(weapon);

        uiChat = TownManager.Instance?.UiChat;
        isInitialized = true;
    }
    #endregion

    void Update()
    {
        if (!isInitialized)
            return;

        if (currentHealth <= 0)
            return;

        if (IsMine)
        {
            // 로컬 플레이어: 입력 처리 및 이동/공격 등
           //GETInput();

            // 직업에 따라 순간이동(마법사) 혹은 회피(아처, 로그)
            //if (IsMage())
            //    Teleport();
            //else
            //    Dodge();

            // 마우스 우클릭으로 목적지 지정 후 이동 처리
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    setDestination(hit.point);
                }
            }
            // 이동 중이면 매 프레임 이동 처리 및 도착 여부 확인
            if (isMove)
            {
                Mousemove();
                CheckArrival();
            }

            // 마우스 좌클릭 공격
            //if (Input.GetMouseButtonDown(0))
            //{
            //    Attack();
            //}
        }
        else
        {
            // 원격 플레이어: 부드러운 보간 처리
            SmoothMoveAndRotate();
        }
    }

    #region Dodge Prediction Methods
    // 회피 보간 시간 (서버와의 좌표 차이 보정용)
    public float dodgeInterpolationTime = 0.3f;

    // 회피 입력 시 계산된 예측 좌표
    private Vector3 predictedDodgePos;

    /// <summary>
    /// 클라이언트가 회피 시 계산한 예측 좌표를 반환합니다.
    /// </summary>
    public Vector3 GetPredictedDodgePosition()
    {
        return predictedDodgePos;
    }

    /// <summary>
    /// 서버에서 전달받은 최종 좌표를 즉시 적용합니다.
    /// </summary>
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    /// <summary>
    /// 현재 위치에서 targetPos까지 일정 시간 동안 보간(interpolation)하여 이동합니다.
    /// (고무줄 효과 연출)
    /// </summary>
    public void InterpolateToPosition(Vector3 targetPos)
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPositionCoroutine(targetPos, dodgeInterpolationTime));
    }

    private IEnumerator MoveToPositionCoroutine(Vector3 targetPos, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }

    /// <summary>
    /// 회피 애니메이션을 실행합니다.
    /// Animator에 "doDodge" 트리거가 설정되어 있어야 합니다.
    /// </summary>
    public void TriggerDodgeAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("doDodge");
        }
        else
        {
            Debug.LogWarning("Animator가 할당되지 않았습니다. 회피 애니메이션을 실행할 수 없습니다.");
        }
    }
    #endregion

    #region Local Player Methods
    public void setDestination(Vector3 dest)
    {
        if (nav != null)
            nav.SetDestination(dest);
        moveVec = dest;
        isMove = true;
        animator.SetBool("isRun", true);
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        equipWeapon = newWeapon;
    }

    void GETInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
    }

    void Mousemove()
    {
        Debug.Log("마우스 무브 들어옴");
        Vector3 currentPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 destination = new Vector3(moveVec.x, 0, moveVec.z);
        Vector3 dir = destination - currentPos;

        if (dir != Vector3.zero)
            transform.forward = dir.normalized;

        // 목표 위치 방향으로 이동
        transform.position += dir.normalized * Time.deltaTime * moveSpeed;
        animator.SetBool("isRun", true);
    }

    public bool IsMage()
    {
        return gameObject.CompareTag("Mage");
    }

    // 도착 여부를 매 프레임 확인하는 함수
    void CheckArrival()
    {
        float distance = Vector3.Distance(transform.position, moveVec);
        if (distance <= 0.5f)
        {
            isMove = false;
            animator.SetBool("isRun", false);
        }
    }

    public void Dodge()
    {
        if (!isDodge)
        {
            // 회피 시 이동 방향 계산:
            // 이동 중이면 목적지와의 방향, 그렇지 않으면 현재 바라보는 방향 사용
            dodgeVec = isMove ? (moveVec - transform.position).normalized : transform.forward;
            // 회피 예측 좌표 계산
            predictedDodgePos = transform.position + dodgeVec * dodgeDistance;
            // 배속 적용 (필요에 따라 값 조정)
            speed *= 2;
            // 새로 추가한 함수로 회피 애니메이션 트리거
            TriggerDodgeAnimation();
            isDodge = true;

            if (gameObject.CompareTag("Archer"))
                ActivateDodgeEffect(dodgeEffectsArcher, ref dodgeEffectArcherIndex);
            else if (gameObject.CompareTag("Rogue"))
                ActivateDodgeEffect(dodgeEffectsRogue, ref dodgeEffectRogueIndex);

            Invoke("DodgeOut", 0.4f);
        }
    }
     
    void DodgeOut()
    {
        speed = 10f;
        isDodge = false;
    }

    public void Teleport()
    {
        if (!isDodge)
        {
            isDodge = true;
            animator.SetTrigger("doTeleport");
            Vector3 teleportDirection = isMove ? (moveVec - transform.position).normalized : transform.forward;
            Vector3 teleportPosition = transform.position + teleportDirection * 10f;
            if (Physics.Raycast(transform.position, teleportDirection, out RaycastHit hit, 10f))
                teleportPosition = hit.point;
            StartCoroutine(TeleportRoutine(teleportPosition));
        }
    }

    IEnumerator TeleportRoutine(Vector3 targetPosition)
    {
        ActivateEffect(teleportEffects, ref effectIndex, transform.position);
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.3f);
        transform.position = targetPosition;
        yield return new WaitForSeconds(0.3f);
        GetComponent<MeshRenderer>().enabled = true;
        isDodge = false;
    }

    void ActivateDodgeEffect(List<GameObject> effectList, ref int effectIndex)
    {
        ActivateEffect(effectList, ref effectIndex, transform.position, dodgeVec);
    }

    void ActivateEffect(List<GameObject> effectList, ref int effectIndex, Vector3 position, Vector3? direction = null)
    {
        if (effectList.Count > 0)
        {
            GameObject effect = effectList[effectIndex];
            if (!effect.activeSelf)
            {
                effect.transform.position = position;
                effect.transform.rotation = direction.HasValue ? Quaternion.LookRotation(direction.Value) : transform.rotation;
                effect.SetActive(true);
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                    ps.Play();
                StartCoroutine(DeactivateEffect(effect, 0.5f));
                effectIndex = (effectIndex + 1) % effectList.Count;
            }
        }
    }

    IEnumerator DeactivateEffect(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        effect.SetActive(false);
    }

    public void Attack()
    {
        if (equipWeapon == null)
            return;

        if (!isDodge)
        {
            equipWeapon.Use();
            int attackIndex = UnityEngine.Random.Range(0, 2);
            animator.SetInteger("attackIndex", attackIndex);
            animator.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 attackDirection = hit.point - transform.position;
                attackDirection.y = 0;
                if (attackDirection.magnitude > 0.1f)
                    transform.rotation = Quaternion.LookRotation(attackDirection.normalized);
            }
            isMove = false;
            animator.SetBool("isRun", false);
            fireDelay = 0;
        }
    }
    #endregion

    #region Remote Player Methods (Smoothing Movement)
    void SmoothMoveAndRotate()
    {
        MoveSmoothly();
        RotateSmoothly();
    }

    void MoveSmoothly()
    {
        transform.position = Vector3.MoveTowards(transform.position, goalPos, agentSpeed * Time.deltaTime);
    }

    void RotateSmoothly()
    {
        if (goalRot != Quaternion.identity)
        {
            float t = Mathf.Clamp(Time.deltaTime * SmoothRotateSpeed, 0, 0.99f);
            transform.rotation = Quaternion.Lerp(transform.rotation, goalRot, t);
        }
    }

    // 원격/네트워크 이동 시 애니메이션 혹은 파라미터 업데이트에 사용할 수 있음
    private void CheckMove()
    {
        Debug.Log("체크무브 들어옴");
        float dist = Vector3.Distance(lastPos, transform.position);
        animator.SetBool("isRun", true);
        lastPos = transform.position;
    }

    // 네트워크를 통해 수신한 이동 정보에 따라 목표 위치, 회전, 속도를 설정
    public void Move(Vector3 move, Quaternion rot, float speed)
    {
        goalPos = move;
        goalRot = rot;
        agentSpeed = speed;
        Debug.Log(goalPos);
    }
    #endregion

    #region Damage & Death
    void Die()
    {
        Debug.Log("캐릭터가 죽었습니다!");
        animator.SetTrigger("doDie");
        if (nav != null)
            nav.isStopped = true;
        rigid.isKinematic = true;
        animator.SetBool("isRun", false);
        isMove = false;
        isFireReady = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyArrow"))
        {
            if (!isDamage)
            {
                Arrow enemyArrow = other.GetComponent<Arrow>();
                currentHealth -= enemyArrow.damage;
                DamageManager.Instance.SpawnDamageText(enemyArrow.damage, transform.Find("Head"), true, 300f);
                Debug.Log("현재 체력: " + currentHealth);
                StartCoroutine(OnDamage());
            }
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public IEnumerator OnDamage()
    {
        isDamage = true;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = UnityEngine.Color.yellow;
        }
        yield return new WaitForSeconds(1f);
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = UnityEngine.Color.white;
        }
        yield return null;
    }
    #endregion

    #region Identification & UI Methods
    public void SetIsMine(bool isMine)
    {
        IsMine = isMine;
        if (IsMine)
        {
            MPlayer = gameObject.AddComponent<MyPlayer>();
        }
        else
        {
            if (nav != null)
                Destroy(nav);
        }
    }

    public void SetPlayerId(int id)
    {
        PlayerId = id;
    }

    public void SetNickname(string name)
    {
        nickname = name;
        if (uiNameChat != null)
            uiNameChat.SetName(name);
    }

    public void RecvMessage(string msg, UIChat.ChatType type)
    {
        if (uiNameChat != null)
            uiNameChat.PushText(msg);
        if (uiChat != null)
            uiChat.PushMessage(msg, IsMine, type);
    }

    public void PlayAnimation(int animCode)
    {
        animator?.SetTrigger("Anim" + animCode);
    }
    #endregion

    // 채팅 메시지를 전송하는 함수 (로컬 플레이어에서만 동작)
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

    // 게임 오브젝트 제거 (Despawn)
    /*
    public void Despawn()
    {
        uiNameChat.PushText(msg);
        uiChat.PushMessage(msg, IsMine, type);
    }
    */
}
