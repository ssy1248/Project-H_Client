using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : MonoBehaviour
{
    private PlayerController playerController;
    private bool isBuffActive = false;
    private float originalAttackRate;

    public List<GameObject> buffEffects; // 이펙트 오브젝트 풀
    private int effectIndex = 0; // 현재 사용할 이펙트 인덱스
    private GameObject activeEffect; // 현재 활성화된 이펙트

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            originalAttackRate = playerController.AttackRate;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isBuffActive)
        {
            StartCoroutine(BoostAttackSpeed());
        }

        // 활성화된 이펙트가 있으면 계속 플레이어를 따라다니게 함
        if (activeEffect != null && activeEffect.activeSelf)
        {
            activeEffect.transform.position = transform.position;
        }
    }

    private IEnumerator BoostAttackSpeed()
    {
        isBuffActive = true;
        playerController.AttackRate *= 0.5f; // 공격 속도 1.5배 증가

        // 이펙트 활성화
        ActivateEffect(buffEffects, ref effectIndex, transform.position);

        yield return new WaitForSeconds(5f); // 5초 지속

        playerController.AttackRate = originalAttackRate; // 원래 속도로 복귀
        isBuffActive = false;

        // 이펙트 비활성화
        if (activeEffect != null)
        {
            activeEffect.SetActive(false);
            activeEffect = null;
        }
    }

    void ActivateEffect(List<GameObject> effectList, ref int effectIndex, Vector3 position, Vector3? direction = null)
    {
        if (effectList.Count > 0)
        {
            GameObject effect = effectList[effectIndex];

            effect.transform.position = position;
            effect.transform.rotation = direction.HasValue ? Quaternion.LookRotation(direction.Value) : transform.rotation;
            effect.SetActive(true);

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();

            activeEffect = effect; // 현재 활성화된 이펙트 저장

            effectIndex = (effectIndex + 1) % effectList.Count; // 다음 인덱스로 이동
        }
    }
}
