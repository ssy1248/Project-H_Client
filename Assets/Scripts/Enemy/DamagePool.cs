using System.Collections.Generic;
using UnityEngine;

public class DamageTextPool : MonoBehaviour
{
    public static DamageTextPool Instance; // �̱��� ���� ����
    public GameObject damageTextPrefab; // ������ �ؽ�Ʈ ������
    public int poolSize = 50; // Ǯ���� ���� (�ִ� 50��)

    private Queue<GameObject> damageTextQueue = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this; // �̱��� �ʱ�ȭ
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(damageTextPrefab, transform);
            obj.SetActive(false);
            damageTextQueue.Enqueue(obj);
        }
    }

    public GameObject GetDamageText()
    {
        if (damageTextQueue.Count > 0)
        {
            GameObject textObj = damageTextQueue.Dequeue();
            textObj.SetActive(true);
            return textObj;
        }
        else
        {
            // Ǯ�� ������ �ʰ��� ��� ���� ���� (��ȿ�����̹Ƿ� Ǯ ũ�� ���� ���)
            GameObject obj = Instantiate(damageTextPrefab, transform);
            return obj;
        }
    }

    public void ReturnDamageText(GameObject textObj)
    {
        textObj.SetActive(false);
        damageTextQueue.Enqueue(textObj);
    }
}
