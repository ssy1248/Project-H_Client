using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpearmanCounter : MonoBehaviour
{
    Animator anim;
    public float counterDuration = 3f; // ī���� �ڼ� ���� �ð�
    private bool isInCounter = false; // ī���� ���� ����
    private float counterTimer = 0f; // ī���� �ð�

    private Weapon weapon; // ���� ��ü ����

    public float counterCooldown = 10f; // ī���� ��ų ��Ÿ�� (10��)
    private bool canUseCounter = true; // ��ų ��� ���� ����

    private void Awake()
    {
        anim = GetComponent<Animator>();
        weapon = GetComponentInChildren<Weapon>(); // Weapon ������Ʈ ��������

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
        canUseCounter = false; // ��ų ���� �Ұ� ����
        counterTimer = 0f;
        anim.SetTrigger("doCounterStance");
        SEManager.instance.PlaySE("SpearmanGuard");
        weapon.ActivateCounterAttack();

        StartCoroutine(CounterCooldownRoutine()); // ��Ÿ�� ����
    }

    private void EndCounter()
    {
        if (weapon == null) return;

        isInCounter = false;
        anim.SetTrigger("doExitCounterStance");
        weapon.DeactivateCounterAttack();
        Debug.Log("Exiting counter stance...");
    }

    // ī���� ��ų ��Ÿ�� ��ƾ
    private IEnumerator CounterCooldownRoutine()
    {
        Debug.Log("Counter skill cooldown started.");
        yield return new WaitForSeconds(counterCooldown); // ��Ÿ�� ���
        canUseCounter = true; // ��ų �ٽ� ��� ����
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
