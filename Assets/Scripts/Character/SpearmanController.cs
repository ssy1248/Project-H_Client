using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearmanCounter : MonoBehaviour
{
    Animator anim;
    public float counterDuration = 3f; // 카운터 자세 유지 시간
    private bool isInCounter = false; // 카운터 자세 여부
    private float counterTimer = 0f; // 카운터 시간

    private Weapon weapon; // Weapon 객체 참조

    private void Awake()
    {
        anim = GetComponent<Animator>();
        weapon = GetComponent<Weapon>(); // Weapon 컴포넌트 가져오기

        // Weapon 컴포넌트가 없다면 에러 로그 출력
        if (weapon == null)
        {
            Debug.LogError("Weapon component not found on this GameObject. Please add a Weapon component.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isInCounter)
        {
            StartCounter(); // 카운터 자세 시작
        }

        if (isInCounter)
        {
            counterTimer += Time.deltaTime;

            // 카운터 시간이 끝나면 idle로 돌아가기
            if (counterTimer >= counterDuration)
            {
                EndCounter();
            }
        }
    }

    private void StartCounter()
    {
        if (weapon == null) return; // Weapon이 없으면 카운터 시작하지 않음

        isInCounter = true;
        counterTimer = 0f;
        anim.SetTrigger("doCounterStance"); // 카운터 자세 애니메이션 트리거
        weapon.ActivateCounterAttack(); // 카운터 공격 활성화
        Debug.Log("Entering counter stance...");
    }

    private void EndCounter()
    {
        if (weapon == null) return; // Weapon이 없으면 카운터 종료하지 않음

        isInCounter = false;
        anim.SetTrigger("doExitCounterStance"); // 카운터 종료 애니메이션 트리거
        weapon.DeactivateCounterAttack(); // 카운터 공격 비활성화
        Debug.Log("Exiting counter stance...");
    }

    // 카운터 자세에서 공격을 받을 때 데미지 처리
    private void OnTriggerEnter(Collider other)
    {
        if (weapon == null) return; // Weapon이 없으면 처리하지 않음

        // 카운터 자세일 때, 'EnemyArrow' 태그의 공격을 받으면 카운터 공격 처리
        if (isInCounter && other.CompareTag("EnemyArrow"))
        {
            
            // 카운터 공격 애니메이션 실행
            anim.SetTrigger("doCounterAttack");

            Weapon incomingWeapon = other.GetComponent<Weapon>();
            if (incomingWeapon != null && incomingWeapon.isCounterAttack)
            {
                // 카운터 공격 처리
                Debug.Log("Counter attack triggered!");
                weapon.Use();  // 카운터 공격 실행 (Swing() 코루틴 실행됨)
            }
        }
    }
}
