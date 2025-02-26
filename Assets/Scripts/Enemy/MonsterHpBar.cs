using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MonsterHealth : MonoBehaviour
{
    public Slider HpBarSlider; // ü�� �� UI
    private float curHealth; // ���� ü��
    public float maxHealth = 2000f; // �ִ� ü��
    public float damageAnimationSpeed = 0.3f; // ü�� ���� �ִϸ��̼� �ӵ�

    private void Start()
    {
        SetHp(maxHealth); // ü�� �ʱ�ȭ

        InvokeRepeating("AutoDamage", 0.5f, 0.5f);
    }

    public void SetHp(float amount)
    {
        maxHealth = amount;
        curHealth = maxHealth;
        UpdateHpBar(); // ü�¹� UI ����
    }

    public class LookAtCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            // ī�޶� ������ ���, �ش� ī�޶� �ٶ󺸵��� ����
            if (Camera.main != null)
            {
                transform.LookAt(transform.position + Camera.main.transform.forward);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (curHealth <= 0) return; // �̹� ü���� 0 �����̸� ����

        curHealth -= damage;
        if (curHealth <= 0)
        {
            curHealth = 0;
            StartCoroutine(SmoothHpDecrease(0)); // ������ ü�� ���� �ִϸ��̼� ����
            Die(); // ���� ��� ó��
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

        HpBarSlider.value = targetValue; // ������ �� ����
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
        Debug.Log("���� ���!");
        // ���� ������Ʈ ����
        Destroy(gameObject);
    }

    private void AutoDamage()
    {
        TakeDamage(200);
    }
}