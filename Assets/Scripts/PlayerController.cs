using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    public List<GameObject> teleportEffects = new List<GameObject>(); // 텔레포트 이펙트 풀
    public List<GameObject> dodgeEffectsArcher = new List<GameObject>(); // 궁수 Dodge 이펙트 풀
    public List<GameObject> dodgeEffectsRogue = new List<GameObject>(); // 도적 Dodge 이펙트 풀


    private int effectIndex = 0; // 텔레포트 이펙트 인덱스
    private int dodgeEffectArcherIndex = 0; // 궁수 Dodge 이펙트 인덱스
    private int dodgeEffectRogueIndex = 0; // 도적 Dodge 이펙트 인덱스

    public int health;

    public float speed;
    float hAxis;
    float vAxis;
    bool DDown;
    bool FDown;

    bool isDodge;
    bool isFireReady = true;
    bool isBorder;
    bool isDamage;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator anim;
    Rigidbody rigid;
    MeshRenderer[] meshs;

    Weapon equipWeapon;
    float fireDelay;

    private void Awake()
    {
        anim = GetComponent<Animator>(); // 자식이 아닌 부모 오브젝트에서 가져오기
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
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

    void Update()
    {
        GETInput();
        move();
        Turn();
        Attack();

        if (IsMage()) // 마법사라면 Dodge 대신 Teleport 사용
            Teleport();
        else
            Dodge();
    }

    bool IsMage()
    {
        return gameObject.CompareTag("Mage"); // 태그가 "Mage"이면 마법사로 판별
    }

    void StopToWall() // 충돌 해결
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward * 5, LayerMask.GetMask("Wall"));
    }
    void FreezeRotation ()
    {
        rigid.angularVelocity = Vector3.zero;
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

    void move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
        {
            moveVec = dodgeVec;
        }

        if (!isFireReady)
        {
            moveVec = Vector3.zero;
        }
        if(!isBorder)
            transform.position += moveVec * speed * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Dodge()
    {
        if (DDown && moveVec != Vector3.zero && !isDodge)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // 궁수라면 궁수 회피 이펙트 활성화
            if (gameObject.CompareTag("Archer"))
            {
                ActivateDodgeEffect(dodgeEffectsArcher, ref dodgeEffectArcherIndex);
            }
            // 도적이라면 도적 회피 이펙트 활성화
            else if (gameObject.CompareTag("Rogue"))
            {
                ActivateDodgeEffect(dodgeEffectsRogue, ref dodgeEffectRogueIndex);
            }

            Invoke("DodgeOut", 0.4f);
        }
    }

    void ActivateDodgeEffect(List<GameObject> dodgeEffectList, ref int effectIndex)
    {
        if (dodgeEffectList.Count > 0)
        {
            GameObject effect = dodgeEffectList[effectIndex];

            if (!effect.activeSelf) // 비활성화된 이펙트만 사용
            {
                effect.transform.position = transform.position;

                // 캐릭터의 이동 방향(dodgeVec)으로 이펙트 회전 설정
                if (dodgeVec != Vector3.zero)
                {
                    effect.transform.rotation = Quaternion.LookRotation(dodgeVec);
                }
                else
                {
                    effect.transform.rotation = transform.rotation; // 이동 방향이 없으면 캐릭터의 현재 방향 사용
                }

                effect.SetActive(true);

                // ParticleSystem이 있다면 강제 Play() 실행
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play(); // 강제 실행
                }

                StartCoroutine(DeactivateEffect(effect, 0.5f)); // 0.5초 후 비활성화

                effectIndex = (effectIndex + 1) % dodgeEffectList.Count; // 다음 이펙트 인덱스
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
        speed = 10f; // 원래 속도로 복구
        isDodge = false;
    }


    void Teleport()
    {
        if (DDown && !isDodge)
        {
            isDodge = true;
            anim.SetTrigger("doTeleport"); // 마법사 순간이동 애니메이션 실행

            Vector3 teleportDirection = moveVec != Vector3.zero ? moveVec : transform.forward;
            Vector3 teleportPosition = transform.position + teleportDirection * 10f;

            StartCoroutine(TeleportRoutine(teleportPosition));
        }
    }

    IEnumerator TeleportRoutine(Vector3 targetPosition)
    {
        // 사라지는 이펙트 사용 (오브젝트 풀링)
        if (teleportEffects.Count > 0)
        {
            GameObject effect = teleportEffects[effectIndex];

            if (!effect.activeSelf) // 비활성화된 이펙트만 사용
            {
                effect.transform.position = transform.position;
                effect.SetActive(true);

                // ParticleSystem이 있다면 강제 Play() 실행
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play(); // 강제 실행
                }

                effectIndex = (effectIndex + 1) % teleportEffects.Count;
            }
        }

        GetComponent<MeshRenderer>().enabled = false; // 캐릭터 숨기기

        yield return new WaitForSeconds(0.3f); // 사라지는 시간

        transform.position = targetPosition; // 순간이동

        yield return new WaitForSeconds(0.3f); // 나타나기 전 대기

        GetComponent<MeshRenderer>().enabled = true; // 캐릭터 다시 보이기

        isDodge = false;

        yield return new WaitForSeconds(1f);
        if (teleportEffects.Count > 0)
        {
            GameObject effect = teleportEffects[effectIndex];
            effect.SetActive(false);
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.attackRate < fireDelay;

        if (FDown && isFireReady && !isDodge)
        {
            equipWeapon.Use();

            int attackIndex = Random.Range(0, 2); // 0, 1 중 하나 선택
            anim.SetInteger("attackIndex", attackIndex); // 애니메이터 변수 설정
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); // 공격 트리거 실행

            transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            fireDelay = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
     if (other.tag == "EnemyAttack")
        {
            if(!isDamage)
            {
            Arrow enemyArrow = other.GetComponent<Arrow>();
            health -= enemyArrow.damage;
            StartCoroutine(OnDamage());
            }

        }
    }

    IEnumerator OnDamage()
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
