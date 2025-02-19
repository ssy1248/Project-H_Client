using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MonsterHealth : MonoBehaviour
{
    public Slider HpBarSlider; // 체력 바 UI
    private float curHealth; // 현재 체력
    public float maxHealth = 2000f; // 최대 체력
    public float damageAnimationSpeed = 0.3f; // 체력 감소 애니메이션 속도

    private void Start()
    {
        SetHp(maxHealth); // 체력 초기화

        InvokeRepeating("AutoDamage", 0.5f, 0.5f);
    }

    public void SetHp(float amount)
    {
        maxHealth = amount;
        curHealth = maxHealth;
        UpdateHpBar(); // 체력바 UI 갱신
    }

    public class LookAtCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            // 카메라가 존재할 경우, 해당 카메라를 바라보도록 설정
            if (Camera.main != null)
            {
                transform.LookAt(transform.position + Camera.main.transform.forward);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (curHealth <= 0) return; // 이미 체력이 0 이하이면 리턴

        curHealth -= damage;
        if (curHealth <= 0)
        {
            curHealth = 0;
            StartCoroutine(SmoothHpDecrease(0)); // 마지막 체력 감소 애니메이션 실행
            Die(); // 몬스터 사망 처리
        }
        else
        {
            StartCoroutine(SmoothHpDecrease(curHealth / maxHealth));
        }
    }

    private IEnumerator SmoothHpDecrease(float targetValue)
    {
        float startValue = HpBarSlider.value;
        float elapsedTime = 0f;

        while (elapsedTime < damageAnimationSpeed)
        {
            elapsedTime += Time.deltaTime;
            HpBarSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / damageAnimationSpeed);
            yield return null;
        }

        HpBarSlider.value = targetValue; // 마지막 값 보정
    }

    private void UpdateHpBar()
    {
        if (HpBarSlider != null)
        {
            HpBarSlider.value = curHealth / maxHealth;
        }
    }

    private void Die()
    {
        Debug.Log("몬스터 사망!");
        // 게임 오브젝트 삭제
        Destroy(gameObject);
    }

    private void AutoDamage()
    {
        TakeDamage(200);
    }
}