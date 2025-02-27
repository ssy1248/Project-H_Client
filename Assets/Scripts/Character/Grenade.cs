using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObject;
    public GameObject effecObject;
    public Rigidbody rigid;
    private bool hasExploded = false;  // 중복 폭발 방지

    private void OnEnable()
    {
        hasExploded = false;  // 다시 던질 때 초기화
        meshObject.SetActive(true);
        effecObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasExploded)  // 이미 폭발한 상태가 아니면 실행
        {
            hasExploded = true;
            StartCoroutine(Explosion());
        }
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);  // 충돌 후 3초 후 폭발
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObject.SetActive(false);
        effecObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
