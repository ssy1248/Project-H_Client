using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeaponEffect : MonoBehaviour
{
    private string groundTag = "Floor"; // �� �±� ����
    public GameObject effectPrefab; // ����� ��ƼŬ ������
    public int poolSize = 10; // ������Ʈ Ǯ ũ��

    private Queue<GameObject> effectPool = new Queue<GameObject>();

    private void Start()
    {
        // ������Ʈ Ǯ ����
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(effectPrefab);
            obj.SetActive(false);
            effectPool.Enqueue(obj);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("�浹 ������: " + collision.gameObject.name);

        // �±� �� ������� �浹 Ȯ��
        if (collision.gameObject.CompareTag(groundTag))
        {
            Debug.Log("���� �浹: ����Ʈ ����!");
            ContactPoint contact = collision.contacts[0]; // �浹 ���� ���� ��������
            Vector3 spawnPosition = contact.point; // �浹 ���� ��ġ
            Quaternion spawnRotation = Quaternion.LookRotation(contact.normal); // ǥ�� ���⿡ �°� ȸ��

            SpawnEffect(spawnPosition, spawnRotation);
        }
    }

    private void SpawnEffect(Vector3 position, Quaternion rotation)
    {
        // ������Ʈ Ǯ���� ��������
        GameObject effect = GetFromPool(position, rotation);
        effect.SetActive(true);
    }

    private GameObject GetFromPool(Vector3 position, Quaternion rotation)
    {
        if (effectPool.Count == 0)
        {
            // Ǯ�� ������Ʈ�� ������ ���� ����
            GameObject newEffect = Instantiate(effectPrefab);
            newEffect.SetActive(false);
            effectPool.Enqueue(newEffect);
        }

        GameObject pooledEffect = effectPool.Dequeue();
        pooledEffect.transform.position = position;
        pooledEffect.transform.rotation = rotation;
        pooledEffect.SetActive(true);

        // ���� �ð� �� �ٽ� Ǯ�� ��ȯ
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
