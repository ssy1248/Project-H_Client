using System.Collections.Generic;
using UnityEngine;

public class BladePool : MonoBehaviour
{
    public GameObject bladePrefab; // 떨어지는 칼 프리팹
    public GameObject impactEffectPrefab; // 칼이 닿을 때 터지는 폭발 이펙트 프리팹

    public int poolSize = 100; // 초기 풀 크기
    private Queue<GameObject> bladePool;
    private Queue<GameObject> impactEffectPool; // 폭발 이펙트 풀

    private void Awake()
    {
        bladePool = new Queue<GameObject>();
        impactEffectPool = new Queue<GameObject>();

        // 게임 시작 시 100개의 블레이드와 폭발 이펙트를 미리 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject blade = Instantiate(bladePrefab, transform);
            blade.SetActive(false);
            bladePool.Enqueue(blade);

            GameObject effect = Instantiate(impactEffectPrefab, transform); // 폭발 이펙트 생성
            effect.SetActive(false);
            impactEffectPool.Enqueue(effect);
        }
    }

    // 블레이드 꺼내서 사용하기
    public GameObject GetBlade()
    {
        if (bladePool.Count > 0)
        {
            GameObject blade = bladePool.Dequeue();
            blade.SetActive(true);
            return blade;
        }

        GameObject newBlade = Instantiate(bladePrefab, transform);
        return newBlade;
    }

    // 블레이드를 다시 풀에 반환
    public void ReturnBlade(GameObject blade)
    {
        blade.SetActive(false);
        bladePool.Enqueue(blade);
    }

    // 폭발 이펙트 가져오기
    public GameObject GetImpactEffect()
    {
        if (impactEffectPool.Count > 0)
        {
            GameObject effect = impactEffectPool.Dequeue();
            effect.SetActive(true);
            return effect;
        }

        GameObject newEffect = Instantiate(impactEffectPrefab, transform);
        return newEffect;
    }

    // 폭발 이펙트를 다시 풀에 반환
    public void ReturnImpactEffect(GameObject effect)
    {
        effect.SetActive(false);
        impactEffectPool.Enqueue(effect);
    }
}
