using System.Collections.Generic;
using UnityEngine;

public class GrenadePool : MonoBehaviour
{
    public GameObject grenadePrefab; // ����ź ������
    public int poolSize = 10; // Ǯ ũ��
    private List<GameObject> grenadePool; // Ǯ�� ������ ����Ʈ

    private void Awake()
    {
        // Ǯ �ʱ�ȭ
        grenadePool = new List<GameObject>();

        // Ǯ�� ����ź�� �̸� �����Ͽ� ����
        for (int i = 0; i < poolSize; i++)
        {
            GameObject grenade = Instantiate(grenadePrefab);
            grenade.SetActive(false); // ó������ ��Ȱ��ȭ
            grenadePool.Add(grenade);
        }
    }

    // ��Ȱ��ȭ�� ����ź�� ��ȯ�ϰ� ����ϴ� �Լ�
    public GameObject GetGrenade()
    {
        foreach (var grenade in grenadePool)
        {
            if (!grenade.activeInHierarchy)
            {
                grenade.SetActive(true); // Ȱ��ȭ
                return grenade;
            }
        }

        // ���� ��Ȱ��ȭ�� ����ź�� ���ٸ� ���� ����
        GameObject newGrenade = Instantiate(grenadePrefab);
        grenadePool.Add(newGrenade);
        return newGrenade;
    }

    // ����� ���� ����ź�� Ǯ�� ��ȯ�ϴ� �Լ�
    public void ReturnGrenade(GameObject grenade)
    {
        grenade.SetActive(false); // ��Ȱ��ȭ
    }
}
