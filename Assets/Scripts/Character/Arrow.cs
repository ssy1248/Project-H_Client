using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage;
    public Weapon weapon;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // �ε��� ��Ŷ ����
            gameObject.SetActive(false);  // ȭ�� ��Ȱ��ȭ
            if (weapon != null)
            {
                weapon.ReturnArrow(gameObject);
            }
        } 
        else
        {
            gameObject.SetActive(false);
            if (weapon != null)
            {
                weapon.ReturnArrow(gameObject);
            }
        }
    }
}