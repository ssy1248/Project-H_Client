using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Arrow �Լ��� ���̵� �����.
    public int arrowId; // �̰��� Arrow �Լ� ������ �ϰų� �Ҷ� ������� �ҵ�?;
    public Weapon weapon;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            // �ε��� ��Ŷ ����
            C_RangeAttackImpact hitObj = new C_RangeAttackImpact
            {
                
            };
            Debug.Log("�ε����� ����");
            gameObject.SetActive(false);  // ȭ�� ��Ȱ��ȭ
            if (weapon != null)
            {
                weapon.ReturnArrow(gameObject);
            }
        } 
        else
        {
            Debug.Log("�ε����� ���Ͱ� �ƴѰͿ�");
            gameObject.SetActive(false);
            if (weapon != null)
            {
                weapon.ReturnArrow(gameObject);
            }
        }
    }
}