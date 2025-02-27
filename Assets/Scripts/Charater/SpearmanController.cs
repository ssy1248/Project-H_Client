using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearmanCounter : MonoBehaviour
{
    Animator anim;
    public float counterDuration = 3f; // ī���� �ڼ� ���� �ð�
    private bool isInCounter = false; // ī���� �ڼ� ����
    private float counterTimer = 0f; // ī���� �ð�

    private Weapon weapon; // Weapon ��ü ����

    private void Awake()
    {
        anim = GetComponent<Animator>();
        weapon = GetComponent<Weapon>(); // Weapon ������Ʈ ��������

        // Weapon ������Ʈ�� ���ٸ� ���� �α� ���
        if (weapon == null)
        {
            Debug.LogError("Weapon component not found on this GameObject. Please add a Weapon component.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isInCounter)
        {
            StartCounter(); // ī���� �ڼ� ����
        }

        if (isInCounter)
        {
            counterTimer += Time.deltaTime;

            // ī���� �ð��� ������ idle�� ���ư���
            if (counterTimer >= counterDuration)
            {
                EndCounter();
            }
        }
    }

    private void StartCounter()
    {
        if (weapon == null) return; // Weapon�� ������ ī���� �������� ����

        isInCounter = true;
        counterTimer = 0f;
        anim.SetTrigger("doCounterStance"); // ī���� �ڼ� �ִϸ��̼� Ʈ����
        weapon.ActivateCounterAttack(); // ī���� ���� Ȱ��ȭ
        Debug.Log("Entering counter stance...");
    }

    private void EndCounter()
    {
        if (weapon == null) return; // Weapon�� ������ ī���� �������� ����

        isInCounter = false;
        anim.SetTrigger("doExitCounterStance"); // ī���� ���� �ִϸ��̼� Ʈ����
        weapon.DeactivateCounterAttack(); // ī���� ���� ��Ȱ��ȭ
        Debug.Log("Exiting counter stance...");
    }

    // ī���� �ڼ����� ������ ���� �� ������ ó��
    private void OnTriggerEnter(Collider other)
    {
        if (weapon == null) return; // Weapon�� ������ ó������ ����

        // ī���� �ڼ��� ��, 'EnemyArrow' �±��� ������ ������ ī���� ���� ó��
        if (isInCounter && other.CompareTag("EnemyArrow"))
        {
            
            // ī���� ���� �ִϸ��̼� ����
            anim.SetTrigger("doCounterAttack");

            Weapon incomingWeapon = other.GetComponent<Weapon>();
            if (incomingWeapon != null && incomingWeapon.isCounterAttack)
            {
                // ī���� ���� ó��
                Debug.Log("Counter attack triggered!");
                weapon.Use();  // ī���� ���� ���� (Swing() �ڷ�ƾ �����)
            }
        }
    }
}
