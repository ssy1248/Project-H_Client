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
            // 부딪힌 패킷 전송
            gameObject.SetActive(false);  // 화살 비활성화
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