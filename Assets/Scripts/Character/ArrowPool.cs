using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    public GameObject arrowPrefab; // 화살 프리팹
    public int initialPoolSize = 20; // 게임 시작 시 풀에 넣을 화살 개수
    private List<GameObject> arrowPool; // 화살 풀 리스트

    private void Awake()
    {
        // 처음에는 풀을 빈 상태로 초기화
        arrowPool = new List<GameObject>();
    }

    private void Start()
    {
        // 게임이 시작되면 화살 프리팹 20개를 생성하여 풀에 추가
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, transform);
            arrow.SetActive(false); // 처음에는 비활성화
            arrowPool.Add(arrow);
        }
    }

    // 비활성화된 화살을 반환하고 사용하는 함수
    public GameObject GetArrow()
    {
        foreach (var arrow in arrowPool)
        {
            if (!arrow.activeInHierarchy)
            {
                arrow.SetActive(true);
                return arrow;
            }
        }

        // 비활성화된 화살이 없다면 새로 생성하여 풀을 확장
        GameObject newArrow = Instantiate(arrowPrefab, transform);
        arrowPool.Add(newArrow);
        return newArrow;
    }

    // 사용이 끝난 화살을 풀에 반환하는 함수
    public void ReturnArrow(GameObject arrow)
    {
        arrow.SetActive(false); // 비활성화
    }
}
