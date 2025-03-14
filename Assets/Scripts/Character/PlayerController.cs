﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public List<GameObject> teleportEffects = new List<GameObject>(); // 텔레포트 이펙트 풀
    public List<GameObject> dodgeEffectsArcher = new List<GameObject>(); // 궁수 Dodge 이펙트 풀
    public List<GameObject> dodgeEffectsRogue = new List<GameObject>(); // 도적 Dodge 이펙트 풀

    public PlayerHealthBar playerHealthBar;

    public int partyIndex;

    private int effectIndex = 0; // 텔레포트 이펙트 인덱스
    private int dodgeEffectArcherIndex = 0; // 궁수 Dodge 이펙트 인덱스
    private int dodgeEffectRogueIndex = 0; // 도적 Dodge 이펙트 인덱스
    public float moveSpeed = 4f;
    private bool isPlayingSound = false; // 소리가 재생 중인지 확인하는 변수

    private float DodgeCoolTime = 5f;

    private int comboIndex = 0; // 콤보 순서 저장 변수
    private float comboResetTime = 1.0f; // 콤보가 초기화되는 시간
    private float lastAttackTime = 0f; // 마지막 공격 시간 저장

    private Camera camera;
    protected NavMeshAgent nav;

    public int maxHealth = 200;
    public int currentHealth;
    public float AttackRate = 0.5f;

    float hAxis;
    float vAxis;
    bool DDown;
    bool FDown;

    public bool isMove = false;

    protected bool isDodge;
    private bool isOnCooldown = false;
    private float currentCooldown = 0f;

    protected bool isFireReady = true;
    bool isBorder1;
    bool isBorder2;
    bool isDamage;

    Vector3 moveVec;
    Vector3 dodgeVec;

    protected Animator anim;
    MeshRenderer[] meshs;
    protected Rigidbody rigid;

    Weapon equipWeapon;
    float fireDelay;

    // 
    MyPlayer testplayer;
    Player otherPlayer;


    public void Awake()
    {
        this.enabled = false; // 현재 스크립트 비활성화

        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); // 자식이 아닌 부모 오브젝트에서 가져오기
        meshs = GetComponentsInChildren<MeshRenderer>();
        camera = Camera.main;
        nav = GetComponent<NavMeshAgent>();
        nav.updateRotation = false;

        currentHealth = maxHealth;

        //임시 파티 인덱스 추후 서버에서 받아와야할듯
        partyIndex = 0;

        testplayer = GetComponent<MyPlayer>();

        otherPlayer = GetComponent<Player>();
    }

    public void setDestination(Vector3 dest)
    {
        //nav.SetDestination(dest);
        moveVec = dest;
        //moveVec = testplayer.MousePos;
        isMove = true;
        anim.SetBool("isRun", true);
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        equipWeapon = newWeapon;
    }

    void Start()
    {
        Weapon weapon = GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            EquipWeapon(weapon);
        }
    }

    // 체력 감소 함수

    // 캐릭터 죽음 처리
    void Die()
    {
        Debug.Log("캐릭터가 죽었습니다!");
        anim.SetTrigger("doDie");

        // 모든 이동, 공격 등의 기능을 비활성화
        nav.isStopped = true;  // NavMeshAgent 멈추기
        rigid.isKinematic = true;  // Rigidbody 비활성화
        anim.SetBool("isRun", false);  // 달리기 애니메이션 멈추기
        PlayerLoopSEManager.instance.StopSE("PlayerRun");
        isMove = false;
        isFireReady = false;

        // 추가적으로 UI나 게임 오버 화면을 띄우는 로직을 여기에 추가 가능
    }

    public void Update()
    {

        // 직업별로 Dodge 또는 Teleport 사용 가능하도록 설정
        //if (gameObject.CompareTag("Mage"))
        //    Teleport();
        //else if (gameObject.CompareTag("Archer") || gameObject.CompareTag("Rogue"))
        //    Dodge();

        //GETInput();
        //move();
        //Turn();

        //Attack();

        //if (IsMage()) // 마법사라면 Dodge 대신 Teleport 사용
        //    Teleport();
        //else
        //    Dodge();

        if(Input.GetMouseButton(1))
        {
            //  if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit)
            RaycastHit hit;
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if(testplayer == null)
                {
                    testplayer = GetComponent<MyPlayer>();
                } else
                {
                    setDestination(testplayer.MousePos);
                }

            }
        }

        if (currentHealth <= 0)
        {
            // 죽으면 더 이상 업데이트되지 않도록
            return;
        }

        // 쿨타임이 끝났다면 currentCooldown을 0으로 리셋
        if (isOnCooldown)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0f)
            {
                isOnCooldown = false;  // 쿨타임 끝
            }
        }

        // D키를 눌렀고, 구르기 중이 아니고 쿨타임이 끝났으면 구르기 실행
        if (DDown && !isDodge && !isOnCooldown)
        {
            Dodge();
        }



        Mousemove();
    }

    void Mousemove()
    {
        if (isMove)
        {
            Debug.Log($"moveVec :  {moveVec}");

            var dir = new Vector3(moveVec.x, transform.position.y, moveVec.z) - new Vector3(transform.position.x, transform.position.y, transform.position.z);
            transform.forward = dir;
            transform.position += dir.normalized * Time.deltaTime * moveSpeed;

            anim.SetBool("isRun", true); // 이동 중일 때만 'run' 애니메이션 실행

            // 소리가 아직 재생되지 않았다면 실행
            if (!isPlayingSound)
            {
                PlayerLoopSEManager.instance.LoopPlaySE("PlayerRun");
                isPlayingSound = true; // 소리 재생 상태 변경
            }
        }
        else
        {
            anim.SetBool("isRun", false); // 이동하지 않으면 'idle' 애니메이션

            // 이동이 끝나면 소리 멈추기
            if (isPlayingSound)
            {
                PlayerLoopSEManager.instance.StopSE("PlayerRun");
                isPlayingSound = false; // 소리 상태 초기화
            }
        }

        // 목표 지점에 도달하면 이동 멈추기
        if (Vector3.Distance(transform.position, moveVec) <= 0.1f)
        {
            isMove = false;
            anim.SetBool("isRun", false); // 목표 도착 시 idle 상태
            if (isPlayingSound)
            {
                PlayerLoopSEManager.instance.StopSE("PlayerRun");
                isPlayingSound = false; // 소리 상태 초기화
            }
        }
    }





    bool IsMage()
    {
        return gameObject.CompareTag("Mage"); // 태그가 "Mage"이면 마법사로 판별
    }

    void StopToWall() // 벽에 충돌했을 때 멈추는 함수
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);

        // 벽을 감지하는 Raycast
        isBorder1 = Physics.Raycast(transform.position, transform.forward, 5f, LayerMask.GetMask("Wall", "Enemy"));

        if (isBorder1) // 벽이 감지되면 이동을 멈추게 설정
        {
            nav.isStopped = true;  // NavMeshAgent의 이동 멈춤
            anim.SetBool("isRun", false); // 달리기 애니메이션 멈추기
        }
        else
        {
            nav.isStopped = false;  // 벽이 없으면 이동 재개
        }
    }

    void FreezeRotation ()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        rigid.constraints = RigidbodyConstraints.FreezeRotationY; // y축 회전만 제한
    }
    void FixedUpdate()
    {
      FreezeRotation();
      StopToWall();
    }

    void GETInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        DDown = Input.GetButtonDown("Dodge");
        FDown = Input.GetButtonDown("Fire1");
    }

    //void move()
    //{
    //    moveVec = new Vector3(hAxis, 0, vAxis).normalized;

    //    if (isDodge)
    //    {
    //        moveVec = dodgeVec;
    //    }

    //    if (!isFireReady)
    //    {
    //        moveVec = Vector3.zero;
    //    }
    //    if(!isBorder)
    //        transform.position += moveVec * speed * Time.deltaTime;

    //    anim.SetBool("isRun", moveVec != Vector3.zero);
    //}

    //void Turn()
    //{
    //    transform.LookAt(transform.position + moveVec);
    //}

    void Dodge()
    {
        // 아처, 로그만 Dodge 가능
        if (!gameObject.CompareTag("Archer") && !gameObject.CompareTag("Rogue"))
            return;

        if (DDown && !isDodge && !isOnCooldown)
        {
            dodgeVec = isMove ? (moveVec - transform.position).normalized : transform.forward; // 이동 중이면 이동 방향 사용
            moveSpeed *= 2;
            anim.SetTrigger("doDodge");
            if (gameObject.CompareTag("Rogue"))
            {
                SEManager.instance.PlaySE("RogueDodge");

            }
            if (gameObject.CompareTag("Archer"))
            {
                SEManager.instance.PlaySE("ArcherDodge");

            }
            isDodge = true;

            if (gameObject.CompareTag("Archer"))
            {
                ActivateDodgeEffect(dodgeEffectsArcher, ref dodgeEffectArcherIndex);
            }
            else if (gameObject.CompareTag("Rogue"))
            {
                ActivateDodgeEffect(dodgeEffectsRogue, ref dodgeEffectRogueIndex);
            }

            // 쿨타임 설정
            isOnCooldown = true;
            currentCooldown = DodgeCoolTime;  // 쿨타임 시간 설정

            Invoke("DodgeOut", 0.4f);
        }
    }

    void Teleport()
    {
        if (DDown && !isDodge)
        {
            isDodge = true;
            anim.SetTrigger("doTeleport");

            Vector3 teleportDirection = isMove ? (moveVec - transform.position).normalized : transform.forward;
            Vector3 teleportPosition = transform.position + teleportDirection * 10f;

            if (Physics.Raycast(transform.position, teleportDirection, out RaycastHit hit, 10f))
            {
                teleportPosition = hit.point; // 벽이 있으면 충돌한 위치로 조정
            }

            StartCoroutine(TeleportRoutine(teleportPosition));
        }
    }

    IEnumerator TeleportRoutine(Vector3 targetPosition)
    {
        ActivateEffect(teleportEffects, ref effectIndex, transform.position); // 사라지는 이펙트

        GetComponent<MeshRenderer>().enabled = false;

        yield return new WaitForSeconds(0.3f);

        transform.position = targetPosition; // 순간이동

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
        moveSpeed = 10f;
        isDodge = false;
    }


    public void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = AttackRate * equipWeapon.attackRate < fireDelay;

        // UI 클릭 시 공격 불가능하도록 처리
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (FDown && isFireReady && !isDodge)
        {
            equipWeapon.Use();

            // 마우스 클릭할 때마다 콤보 증가 (순환)
            comboIndex = (comboIndex + 1) % 3;

            anim.SetInteger("attackIndex", comboIndex);
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");

            if (gameObject.CompareTag("Rogue"))
            {
                SEManager.instance.PlaySE("RogueHit");

            }
            if (gameObject.CompareTag("Archer"))
            {
                SEManager.instance.PlaySE("ArcherHit");

            }
            if (gameObject.CompareTag("Spearman"))
            {
                SEManager.instance.PlaySE("SpearmanHit");

            }

            // 마우스 클릭한 위치를 기준으로 방향 설정
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 attackDirection = hit.point - transform.position;
                attackDirection.y = 0; // y축 회전 방지 (바닥 평면 기준)
                if (attackDirection.magnitude > 0.1f)
                {
                    transform.rotation = Quaternion.LookRotation(attackDirection);
                }
            }

            // 공격 시 이동 멈추기
            isMove = false;
            anim.SetBool("isRun", false);

            fireDelay = 0;
            lastAttackTime = Time.time; // 마지막 공격 시간 저장
        }

        // 일정 시간이 지나면 콤보 초기화
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboIndex = 0;
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyArrow")) // "EnemyArrow" 태그인지 확인
        {
            if (!isDamage)
            {
                EnemyArrow enemyArrow = other.GetComponent<EnemyArrow>();
                playerHealthBar.TakeDamage(enemyArrow.damage);
                currentHealth -= enemyArrow.damage;
                DamageManager.Instance.SpawnDamageText(enemyArrow.damage, transform.Find("Head"), isPlayerHit: true, 300f);
                Debug.Log("현재 체력: " + currentHealth);  // 현재 체력 로그 출력
                StartCoroutine(OnDamage());
            }

            if (currentHealth <= 0)
            {
                Die();  // 체력이 0 이하일 경우 죽음 처리
            }
        }
    }

    public IEnumerator OnDamage()
    {
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
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




}
