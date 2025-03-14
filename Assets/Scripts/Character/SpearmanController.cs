using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpearmanCounter : MonoBehaviour
{
    Animator anim;
    public float counterDuration = 3f; // 카운터 자세 유지 시간
    private bool isInCounter = false; // 카운터 상태 여부
    private float counterTimer = 0f; // 카운터 시간

    private Weapon weapon; // 무기 객체 참조

    public float counterCooldown = 10f; // 카운터 스킬 쿨타임 (10초)
    private bool canUseCounter = true; // 스킬 사용 가능 여부

    private void Awake()
    {
        anim = GetComponent<Animator>();
        weapon = GetComponentInChildren<Weapon>(); // Weapon 컴포넌트 가져오기

        if (weapon == null)
        {
            Debug.LogError("Weapon component not found");
        }
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name.StartsWith("Dungeon"))
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isInCounter && canUseCounter)
            {
                StartCounter();
            }
        }

        if (isInCounter)
        {
            counterTimer += Time.deltaTime;

            if (counterTimer >= counterDuration)
            {
                EndCounter();
            }
        }
    }

    private void StartCounter()
    {
        if (weapon == null) return;

        isInCounter = true;
        canUseCounter = false; // 스킬 재사용 불가 설정
        counterTimer = 0f;
        anim.SetTrigger("doCounterStance");
        SEManager.instance.PlaySE("SpearmanGuard");
        weapon.ActivateCounterAttack();

        StartCoroutine(CounterCooldownRoutine()); // 쿨타임 시작
    }

    private void EndCounter()
    {
        if (weapon == null) return;

        isInCounter = false;
        anim.SetTrigger("doExitCounterStance");
        weapon.DeactivateCounterAttack();
        Debug.Log("Exiting counter stance...");
    }

    // 카운터 스킬 쿨타임 루틴
    private IEnumerator CounterCooldownRoutine()
    {
        Debug.Log("Counter skill cooldown started.");
        yield return new WaitForSeconds(counterCooldown); // 쿨타임 대기
        canUseCounter = true; // 스킬 다시 사용 가능
        Debug.Log("Counter skill is ready again!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (weapon == null) return;

        if (isInCounter && other.CompareTag("EnemyArrow"))
        {
            anim.SetTrigger("doCounterAttack");
            SEManager.instance.PlaySE("SpearmanCounterAttack");

            Weapon incomingWeapon = other.GetComponent<Weapon>();
            if (incomingWeapon != null && incomingWeapon.isCounterAttack)
            {
                Debug.Log("Counter attack triggered!");
                weapon.Use();
            }
        }
    }
}
