using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B};
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public Material mat;
    public NavMeshAgent nav;
    public Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        //if(enemyType != Type.B)
        Invoke("ChaseStart", 2);
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        //if (nav.enabled && enemyType != Type.B)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
     
    void Targetting()
    {
        if (!isDead && enemyType != Type.B)
        {
            float targetRadius = 0;
            float targetRange = 0;

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
            }
            RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;

        }
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    void FixedUpdate()
    {
        Targetting();
        FreezeVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();

            // 카운터 공격 여부 확인
            if (weapon != null && weapon.isCounterAttack)
            {
                curHealth -= weapon.damage;  // 카운터 공격으로 받은 데미지 처리
                DamageManager.Instance.SpawnDamageText(weapon.damage, transform.Find("Head"), isPlayerHit: false, 400f);
                StartCoroutine(OnDamage());

                Debug.Log("Counter Attack: " + curHealth);
            }
            else if (weapon != null)
            {
                curHealth -= weapon.damage;  // 일반 공격으로 받은 데미지 처리
                DamageManager.Instance.SpawnDamageText(weapon.damage, transform.Find("Head"), isPlayerHit: false, 400f);
                StartCoroutine(OnDamage());

                Debug.Log("Melee: " + curHealth);
            }
        }

        else if (other.tag == "Arrow")
        {
            Arrow arrow = other.GetComponent<Arrow>();
            curHealth -= arrow.damage;
            DamageManager.Instance.SpawnDamageText(arrow.damage, transform.Find("Head"), isPlayerHit: false, 400f);
            StartCoroutine(OnDamage());

            Debug.Log("Range : " + curHealth);
        }
    }

    public void ReceiveDamage(int damage) //스킬 판정
    {
        curHealth -= damage; // 받은 데미지만큼 체력 감소
        DamageManager.Instance.SpawnDamageText(damage, transform.Find("Head"), isPlayerHit: false, 400f); // 데미지 텍스트
        StartCoroutine(OnDamage()); // 데미지 처리 애니메이션
    }
    IEnumerator OnDamage()
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 14;
            isDead = true;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");
            // if (enemyType != Type.B)
            Destroy(gameObject, 4);
        }
    }
}
