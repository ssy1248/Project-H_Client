using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeaponEffect : MonoBehaviour
{
    private string groundTag = "Floor"; // 땅 태그 설정
    public GameObject effectPrefab; // 사용할 파티클 프리팹
    public int poolSize = 10; // 오브젝트 풀 크기

    private Queue<GameObject> effectPool = new Queue<GameObject>();

    private void Start()
    {
        // 오브젝트 풀 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(effectPrefab);
            obj.SetActive(false);
            effectPool.Enqueue(obj);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("충돌 감지됨: " + collision.gameObject.name);

        // 태그 비교 방식으로 충돌 확인
        if (collision.gameObject.CompareTag(groundTag))
        {
            Debug.Log("땅과 충돌: 이펙트 생성!");
            ContactPoint contact = collision.contacts[0]; // 충돌 지점 정보 가져오기
            Vector3 spawnPosition = contact.point; // 충돌 지점 위치
            Quaternion spawnRotation = Quaternion.LookRotation(contact.normal); // 표면 방향에 맞게 회전

            SpawnEffect(spawnPosition, spawnRotation);
        }
    }

    private void SpawnEffect(Vector3 position, Quaternion rotation)
    {
        // 오브젝트 풀에서 가져오기
        GameObject effect = GetFromPool(position, rotation);
        effect.SetActive(true);
    }

    private GameObject GetFromPool(Vector3 position, Quaternion rotation)
    {
        if (effectPool.Count == 0)
        {
            // 풀에 오브젝트가 없으면 새로 생성
            GameObject newEffect = Instantiate(effectPrefab);
            newEffect.SetActive(false);
            effectPool.Enqueue(newEffect);
        }

        GameObject pooledEffect = effectPool.Dequeue();
        pooledEffect.transform.position = position;
        pooledEffect.transform.rotation = rotation;
        pooledEffect.SetActive(true);

        // 일정 시간 후 다시 풀로 반환
        StartCoroutine(ReturnToPool(pooledEffect, 2f));

        return pooledEffect;
    }

    private IEnumerator ReturnToPool(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        effect.SetActive(false);
        effectPool.Enqueue(effect);
    }
}
