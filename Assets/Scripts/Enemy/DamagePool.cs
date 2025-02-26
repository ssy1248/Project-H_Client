using System.Collections.Generic;
using UnityEngine;

public class DamageTextPool : MonoBehaviour
{
    public static DamageTextPool Instance; // 싱글턴 패턴 적용
    public GameObject damageTextPrefab; // 데미지 텍스트 프리팹
    public int poolSize = 50; // 풀링할 개수 (최대 50개)

    private Queue<GameObject> damageTextQueue = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this; // 싱글턴 초기화
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
            // 풀링 개수를 초과할 경우 새로 생성 (비효율적이므로 풀 크기 조정 고려)
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
