using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Arrow 함수에 아이디를 만든다.
    public int arrowId; // 이것은 Arrow 함수 생성을 하거나 할때 보내줘야 할듯?;
    public Weapon weapon;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            // 부딪힌 패킷 전송
            C_RangeAttackImpact hitObj = new C_RangeAttackImpact
            {
                ArrowId = arrowId,
                HitObject = 1, // 몬스터
                MonsterId = other.transform.GetComponent<Monster>().MonsterId,
            };

            Debug.Log("부딪혔다 몬스터");
            gameObject.SetActive(false);  // 화살 비활성화
            if (weapon != null)
            {
                weapon.ReturnArrow(gameObject);
            }

            GameManager.Network.Send(hitObj);
        } 
        else
        {
            // 부딪힌 패킷 전송
            C_RangeAttackImpact hitObj = new C_RangeAttackImpact
            {
                ArrowId = arrowId,
                HitObject = 2, // 장애물
                MonsterId = "0",
            };

            Debug.Log("부딪혔다 몬스터가 아닌것에");
            gameObject.SetActive(false);
            if (weapon != null)
            {
                weapon.ReturnArrow(gameObject);
            }

            GameManager.Network.Send(hitObj);
        }
    }
}