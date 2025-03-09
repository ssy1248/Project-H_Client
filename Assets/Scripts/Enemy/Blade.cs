using System.Collections;
using UnityEngine;

public class Blade : MonoBehaviour
{
    private BladePool bladePool;
    private Collider bladeCollider;
    private Rigidbody rb;

    private void Awake()
    {
        bladePool = FindObjectOfType<BladePool>();

        // Rigidbody 추가 및 설정
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.isKinematic = false; // 물리적 충돌을 위해 활성화

        // Capsule Collider 추가 & 설정
        bladeCollider = GetComponent<CapsuleCollider>();
        if (bladeCollider == null)
        {
            bladeCollider = gameObject.AddComponent<CapsuleCollider>();
        }

        CapsuleCollider capsule = bladeCollider as CapsuleCollider;
        capsule.height = 2.5f;
        capsule.radius = 0.2f;
        capsule.center = new Vector3(0, 1.25f, 0);
        capsule.direction = 1; // Y축 기준 (세로)

        bladeCollider.isTrigger = false;  // 충돌 감지를 위해 Trigger 해제
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 바닥에 닿으면 폭발 이펙트 활성화
        if (collision.gameObject.CompareTag("Floor"))
        {
            GameObject impactEffect = bladePool.GetImpactEffect();
            impactEffect.transform.position = transform.position;
            impactEffect.transform.rotation = Quaternion.identity;

            // 일정 시간 후 풀에 반환
            StartCoroutine(ReturnBladeAndEffect(impactEffect));
        }
    }

    private IEnumerator ReturnBladeAndEffect(GameObject impactEffect)
    {
        yield return new WaitForSeconds(1f); // 폭발 이펙트 유지 시간

        // 칼과 폭발 이펙트를 풀에 반환
        bladePool.ReturnBlade(gameObject);
        bladePool.ReturnImpactEffect(impactEffect);
    }
}
