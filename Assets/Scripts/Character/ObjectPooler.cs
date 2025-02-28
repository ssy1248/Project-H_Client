using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public List<GameObject> pooledObjects; // 풀에 담을 오브젝트 목록
    public GameObject landeffectprefab; // 풀에 넣을 오브젝트 프리팹
    public int poolSize = 10; // 풀의 크기

    private void Awake()
    {

        pooledObjects = new List<GameObject>();
        // 10개의 오브젝트를 미리 풀에 추가
        for (int i = 0; i < poolSize; i++)
        {
            GameObject landeffect = Instantiate(landeffectprefab);
            landeffect.SetActive(false); // 처음에는 비활성화
            pooledObjects.Add(landeffect);
        }
    }

    // 비활성화된 오브젝트를 반환
    public GameObject GetPooledObject()
    {
        foreach (var landeffect in pooledObjects)
        {
            if (!landeffect.activeInHierarchy)
            {
                landeffect.SetActive(true); // 활성화
                return landeffect;
            }
        }
        // 만약 비활성화된 화살이 없다면 새로 생성하여 반환
        GameObject newlandeffect = Instantiate(landeffectprefab);
        pooledObjects.Add(newlandeffect);
        return newlandeffect;
    }

    // 오브젝트를 풀로 반환
    public void ReturnToPool(GameObject landeffect)
    {
        landeffect.SetActive(false); // 비활성화하여 풀로 반환
    }
}
