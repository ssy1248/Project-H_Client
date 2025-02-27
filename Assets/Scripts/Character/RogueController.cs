using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueController : MonoBehaviour
{
    private PlayerController playerController;
    public GameObject grenadePoolObject;  // 수류탄 풀 오브젝트 참조
    private float throwForce = 15f;  // 수류탄의 던지는 힘
    private float backflipForce = 5f;  // 백플립의 힘

    private int damage = 50;  // 데미지 값 (예시)

    Animator anim;
    Rigidbody rigid;
    private bool isFlipping = false;

    private Queue<GameObject> grenadePool = new Queue<GameObject>();  // 수류탄 풀

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        InitializeGrenadePool();  // 풀 오브젝트로부터 수류탄들을 초기화
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isFlipping)
        {
            StartCoroutine(ExecuteBackflipAndThrow());
        }
    }

    private IEnumerator ExecuteBackflipAndThrow()
    {
        isFlipping = true;
        anim.SetTrigger("doBackflip");
        playerController.isMove = false;

        // 백플립 후 뒤로 날아가는 힘 추가
        rigid.AddForce(Vector3.up * backflipForce + -transform.forward * 5f, ForceMode.Impulse);

        yield return new WaitForSeconds(0.2f); // 백플립 시작 후 약간의 딜레이 후 투척
        ThrowGrenade();

        yield return new WaitForSeconds(0.8f); // 백플립 애니메이션 시간 대기
        isFlipping = false;
    }

    private void ThrowGrenade()
    {
        // 수류탄을 풀에서 가져옴
        GameObject grenade = GetGrenadeFromPool();
        grenade.transform.position = transform.position + transform.forward * 2f + Vector3.up * 0.5f;
        grenade.transform.rotation = transform.rotation;

        Rigidbody grenadeRb = grenade.GetComponent<Rigidbody>();

        // 수류탄이 포물선으로 날아가게 하기 위해 상방향과 앞방향에 힘을 줌
        Vector3 throwDirection = (transform.forward + Vector3.up * 0.5f).normalized;  // 던질 방향 (앞 + 위)
        grenadeRb.velocity = throwDirection * throwForce;

        // 수류탄이 던져진 후 1.5초 후에 자동으로 폭발
    }

    private void InitializeGrenadePool()
    {
        // 수류탄 풀 오브젝트 안의 수류탄들을 찾아서 비활성화한 뒤 큐에 추가
        foreach (Transform child in grenadePoolObject.transform)
        {
            GameObject grenade = child.gameObject;
            grenade.SetActive(false);  // 풀 오브젝트에 있는 수류탄을 비활성화
            grenadePool.Enqueue(grenade);  // 비활성화된 수류탄을 풀에 추가
        }
    }

    private GameObject GetGrenadeFromPool()
    {
        // 풀에서 비활성화된 수류탄을 가져옴
        if (grenadePool.Count > 0)
        {
            GameObject grenade = grenadePool.Dequeue();  // 큐에서 하나를 꺼냄
            grenade.SetActive(true);  // 활성화
            return grenade;
        }

        // 풀에 수류탄이 없으면 경고 메시지 출력
        Debug.LogWarning("Grenade pool is empty!");
        return null;  // 수류탄이 없으면 null 반환 (이 경우 추가 로직을 넣을 수 있음)
    }
}
