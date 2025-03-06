using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerTest : MonoBehaviour
{
    #region Effects & Movement (Local)
    [Header("Effects")]
    public List<GameObject> teleportEffects = new List<GameObject>();       // �ڷ���Ʈ ����Ʈ Ǯ
    public List<GameObject> dodgeEffectsArcher = new List<GameObject>();      // �ü� ȸ�� ����Ʈ Ǯ
    public List<GameObject> dodgeEffectsRogue = new List<GameObject>();       // ���� ȸ�� ����Ʈ Ǯ
    private int effectIndex = 0;
    private int dodgeEffectArcherIndex = 0;
    private int dodgeEffectRogueIndex = 0;

    [Header("Local Movement Settings")]
    public float moveSpeed = 10f;         // �̵� �ӵ� (Ű �Է� ��)
    public float speed = 10f;             // �߰� �ӵ� ���� (ȸ�� �� ���)
    private Camera cam;
    protected NavMeshAgent nav;         // ���� �÷��̾�� �׺�޽�
    #endregion

    #region Health & Damage
    // �̺κ��� ���߿� �������� �����ִ� �κ����� �����ϴ� �κ��� �߰��ؾ���
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    bool isDamage = false;
    #endregion

    #region Input & Animation (Local)
    // �Է� ���� ����
    float hAxis, vAxis;
    protected bool isMove = false;
    protected bool isDodge = false;
    protected bool isFireReady = true;
    Vector3 moveVec;
    Vector3 dodgeVec;

    protected Animator anim;
    MeshRenderer[] meshs;
    protected Rigidbody rigid;
    Weapon equipWeapon;
    float fireDelay;
    #endregion

    #region Remote (Smoothing Movement)
    [Header("Remote Movement Settings")]
    public float SmoothMoveSpeed = 10f;
    public float SmoothRotateSpeed = 10f;
    public float TeleportDistanceThreshold = 10f;
    public Vector3 goalPos;
    public Quaternion goalRot;
    private float agentSpeed;
    private Vector3 lastPos;
    #endregion

    #region Identification & UI
    public int PlayerId { get; private set; }
    public string nickname;
    public bool IsMine { get; private set; }
    private bool isInitialized = false;
    public Avatar Avatar { get; private set; }
    public MyPlayer MPlayer { get; private set; }
    // UI ���� (�г���, ä�� ��)
    [SerializeField] private UINameChat uiNameChat;
    private UIChat uiChat;
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
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
        // �ڽĿ� Weapon�� ������ ����
        Weapon weapon = GetComponentInChildren<Weapon>();
        if (weapon != null)
            EquipWeapon(weapon);

        // UI ä�� ���� (TownManager�� �̱����̶�� ����)
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
            // ���� �÷��̾� �Է� ó��
            //GETInput();
            //if (IsMage())
            //    Teleport();
            //else
            //    Dodge();

            if (Input.GetMouseButton(1))
            {
                RaycastHit hit;
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    setDestination(hit.point);
                    //CheckMove();
                    Mousemove();
                }
            }
        }
        else
        {
            // ���� �÷��̾�� �ε巯�� ���� ó��
            SmoothMoveAndRotate();
        }
    }

    #region Local Player Methods

    public void setDestination(Vector3 dest)
    {
        if (nav != null)
            nav.SetDestination(dest);
        moveVec = dest;
        isMove = true;
        anim.SetBool("isRun", true);
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
        Debug.Log("���콺 ���� ����");
        if (isMove)
        {
            Vector3 currentPos = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 destination = new Vector3(moveVec.x, 0, moveVec.z);
            Vector3 dir = destination - currentPos;
            if (dir != Vector3.zero)
                transform.forward = dir.normalized;

            // ��ǥ ��ġ�� �̵�
            transform.position += dir.normalized * Time.deltaTime * moveSpeed;
            anim.SetBool("isRun", true);
            Debug.Log($"isRun : {anim.GetBool("isRun")}");
        }
        else
        {
            anim.SetBool("isRun", false);
        }

        if (Vector3.Distance(transform.position, moveVec) <= 0.5f)
        {
            isMove = false;
            anim.SetBool("isRun", false);
        }
    }

    bool IsMage()
    {
        return gameObject.CompareTag("Mage");
    }

    void Dodge()
    {
        if (!isDodge)
        {
            dodgeVec = isMove ? (moveVec - transform.position).normalized : transform.forward;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            if (gameObject.CompareTag("Archer"))
                ActivateDodgeEffect(dodgeEffectsArcher, ref dodgeEffectArcherIndex);
            else if (gameObject.CompareTag("Rogue"))
                ActivateDodgeEffect(dodgeEffectsRogue, ref dodgeEffectRogueIndex);

            Invoke("DodgeOut", 0.4f);
        }
    }

    void Teleport()
    {
        if (!isDodge)
        {
            isDodge = true;
            anim.SetTrigger("doTeleport");
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
                if (ps != null) ps.Play();
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

    void DodgeOut()
    {
        speed = 10f;
        isDodge = false;
    }

    public void Attack()
    {
        // �̺κ� ���
        if (equipWeapon == null)
            return;

        // �̺κ� ���

        if (!isDodge)
        {
            equipWeapon.Use();
            int attackIndex = Random.Range(0, 2);
            anim.SetInteger("attackIndex", attackIndex);
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 attackDirection = hit.point - transform.position;
                attackDirection.y = 0;
                if (attackDirection.magnitude > 0.1f)
                    transform.rotation = Quaternion.LookRotation(attackDirection.normalized);
            }
            isMove = false;
            anim.SetBool("isRun", false);
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

    // �̺κ��� �������� �÷��̾� �̵�
    private void CheckMove()
    {
        Debug.Log("üũ���� ����");
        float dist = Vector3.Distance(lastPos, transform.position);
        anim.SetBool("isRun", true);
        lastPos = transform.position;
    }
    #endregion

    #region Damage & Death
    void Die()
    {
        Debug.Log("ĳ���Ͱ� �׾����ϴ�!");
        anim.SetTrigger("doDie");
        if (nav != null)
            nav.isStopped = true;
        rigid.isKinematic = true;
        anim.SetBool("isRun", false);
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
                Debug.Log("���� ü��: " + currentHealth);
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
            mesh.material.color = Color.yellow;
        }
        yield return new WaitForSeconds(1f);
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
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

    public void RecvMessage(string msg, UIChat.ChatType type,int id)
    {
        if (uiNameChat != null)
            uiNameChat.PushText(msg);
        if (uiChat != null)
            uiChat.PushMessage(msg, IsMine, type, id);
    }

    public void PlayAnimation(int animCode)
    {
        anim?.SetTrigger("Anim" + animCode);
    }
    #endregion
}
