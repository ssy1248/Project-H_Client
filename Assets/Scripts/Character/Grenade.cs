using System.Collections.Generic;
using UnityEngine;

public class GrenadePool : MonoBehaviour
{
    public GameObject grenadePrefab; // 수류탄 프리팹
    public int poolSize = 10; // 풀 크기
    private List<GameObject> grenadePool; // 풀을 저장할 리스트

    private void Awake()
    {
        // 풀 초기화
        grenadePool = new List<GameObject>();

        // 풀에 수류탄을 미리 생성하여 저장
        for (int i = 0; i < poolSize; i++)
        {
            GameObject grenade = Instantiate(grenadePrefab);
            grenade.SetActive(false); // 처음에는 비활성화
            grenadePool.Add(grenade);
        }
    }

    // 비활성화된 수류탄을 반환하고 사용하는 함수
    public GameObject GetGrenade()
    {
        foreach (var grenade in grenadePool)
        {
            if (!grenade.activeInHierarchy)
            {
                grenade.SetActive(true); // 활성화
                return grenade;
            }
        }

        // 만약 비활성화된 수류탄이 없다면 새로 생성
        GameObject newGrenade = Instantiate(grenadePrefab);
        grenadePool.Add(newGrenade);
        return newGrenade;
    }

    // 사용이 끝난 수류탄을 풀에 반환하는 함수
    public void ReturnGrenade(GameObject grenade)
    {
        grenade.SetActive(false); // 비활성화
    }
}
