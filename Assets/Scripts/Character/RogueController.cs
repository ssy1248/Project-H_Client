using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueController : PlayerController
{
    public GameObject landEffectPrefab;  // 착지 이펙트 프리팹
    private bool isLanding = false;  // 스킬 진행 중인지 체크하는 변수
    public GameObject LandEffectPoolObj; // 풀을 저장할 리스트
    private List<GameObject> LandEffectPool; // 풀을 저장할 리스트


    private float SkillCoolTime;
    //현재 애니메이션 동작으로 인해 기본 쿨타임은 3초

    private void Start()
    {
        anim = GetComponent<Animator>();

        LandEffectPool = new List<GameObject>();

        // 빈 오브젝트에 자식으로 있는 모든 프리팹을 가져옴
        foreach (Transform child in LandEffectPoolObj.transform)
        {
            GameObject landeffect = child.gameObject;
            landeffect.SetActive(false); // 처음에는 비활성화
            LandEffectPool.Add(landeffect);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Skill1") && !isLanding) // 스킬1 입력 받기 전에 isLanding이 false인지 확인
        {
            JumpAndSmash();
        }
    }

    private void JumpAndSmash()
    {
        if (!isLanding)  // 스킬이 진행 중이지 않을 때만 실행
        {
            isLanding = true;  // 스킬 진행 중으로 설정
            anim.SetTrigger("doSkill1");  // 점프 및 내려찍기 애니메이션 트리거
            StartCoroutine(PerformSmash());
        }

        isMove = false; // 스킬 사용 중 이동 불가
    }

    private IEnumerator PerformSmash()
    {
        // 점프 애니메이션이 끝날 때까지 기다립니다 (예: 1초)
        yield return new WaitForSeconds(1f);

        // 캐릭터의 위치에서 앞쪽으로 약간 이동한 위치 계산
        Vector3 effectPosition = transform.position + transform.forward * 1.5f;  // 1.5f는 이동할 거리

        // 착지 이펙트 풀에서 비활성화된 오브젝트를 꺼내서 활성화
        GameObject instantlandeffect = GetLandEffectFromPool();
        if (instantlandeffect != null)
        {
            instantlandeffect.transform.position = effectPosition;  // 수정된 이펙트 위치 설정
            instantlandeffect.SetActive(true);

            // 착지 후 일정 시간 후 이펙트를 풀로 반환
            yield return new WaitForSeconds(2f);  // 이펙트가 보여질 시간
            StartCoroutine(ReturnLandEffectToPool(instantlandeffect));  // 풀로 반환
        }
        else
        {
            // 만약 풀에서 오브젝트를 가져올 수 없으면 로그 출력
            Debug.LogError("No available effect object in the pool.");
        }

        //추후 쿨타임 계산 가능
        //yield return new WaitForSeconds(SkillCoolTime);

        isLanding = false;  // 착지 상태 초기화 (스킬이 끝났으므로 다시 스킬을 사용할 수 있음)
    }

    // 비활성화된 착지 이펙트 오브젝트를 풀에서 꺼내는 함수
    private GameObject GetLandEffectFromPool()
    {
        foreach (var landeffect in LandEffectPool)
        {
            if (!landeffect.activeInHierarchy)
            {
                landeffect.SetActive(true); // 활성화
                return landeffect;
            }
        }

        // 만약 비활성화된 오브젝트가 없다면 null 반환
        GameObject newEffect = Instantiate(landEffectPrefab);
        LandEffectPool.Add(newEffect);
        return newEffect;
    }

    // 사용이 끝난 착지 이펙트를 풀에 반환하는 함수
    private IEnumerator ReturnLandEffectToPool(GameObject landeffect)
    {
        yield return new WaitForSeconds(2f); // 이펙트가 화면에 보여지는 시간 (여기서 2초)
        landeffect.SetActive(false); // 비활성화하여 풀로 반환
    }
}
