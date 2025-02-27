using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueController : PlayerController
{
    public GameObject grenadePoolObject;  // 수류탄 풀 (부모 오브젝트)
    private List<GameObject> grenadePool = new List<GameObject>();
    private bool GDOWN;

    private void Start()
    {
        // 수류탄 풀에서 미리 생성된 수류탄을 가져오기
        foreach (Transform child in grenadePoolObject.transform)
        {
            grenadePool.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        GetInput();
        Grenade();
    }

    void GetInput()
    {
        GDOWN = Input.GetButtonDown("Skill1");  // Q 키 입력 감지
    }

    void Grenade()
    {
        if (grenadePool.Count == 0)  // 남은 수류탄이 없으면 반환
        {
            Debug.Log("수류탄이 부족하여 던질 수 없음!");
            return;
        }

        if (GDOWN && !isDodge && !isMove)
        {
            Debug.Log("수류탄 던지기 실행!");
            ThrowGrenade();
        }
    }

    void ThrowGrenade()
    {
        // 풀에서 비활성화된 수류탄 찾기
        GameObject grenade = null;
        foreach (var g in grenadePool)
        {
            if (!g.activeInHierarchy)  // 비활성화된 오브젝트 찾기
            {
                grenade = g;
                break;
            }
        }

        if (grenade == null)
        {
            Debug.Log("사용 가능한 수류탄 없음!");
            return;
        }

        Debug.Log($"수류탄 던짐: {grenade.name}");

        // 수류탄을 플레이어 위치에서 생성
        grenade.transform.position = transform.position + Vector3.up * 1f;
        grenade.transform.rotation = Quaternion.identity;
        grenade.SetActive(true); // 활성화

        Rigidbody grenadeRb = grenade.GetComponent<Rigidbody>();
        grenadeRb.velocity = Vector3.zero;
        grenadeRb.angularVelocity = Vector3.zero;

        // 던질 방향 설정 (앞 + 위)
        Vector3 throwDirection = (transform.forward * 1.0f + Vector3.up * 0.5f).normalized;
        grenadeRb.velocity = throwDirection * 15f;
    }
}
