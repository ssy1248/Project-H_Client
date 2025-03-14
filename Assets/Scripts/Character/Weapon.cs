using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float attackRate = 1;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public Transform ArrowPos;
    public GameObject ArrowPrefab; // 기본 화살 프리팹
    public int ArrowId;

    public GameObject arrowPoolObject; // 빈 오브젝트로 전체 화살 풀을 관리

    [SerializeField]
    private List<GameObject> arrowPool; // 오브젝트 풀
    public int poolSize = 10; // 풀 크기


    public bool isCounterAttack = false; // 카운터 공격 여부

    private void Awake()
    {
        if (arrowPoolObject == null)
        {
            arrowPoolObject = GameObject.Find("Arrow Pool");
        }
    }

    private void Start()
    {
        // 풀 초기화 (원거리 무기일 때만 실행)
        if (type == Type.Range && arrowPoolObject != null)
        {
            arrowPool = new List<GameObject>();

            // 빈 오브젝트에 자식으로 있는 모든 프리팹을 가져옴
            foreach (Transform child in arrowPoolObject.transform)
            {
                GameObject arrow = child.gameObject;
                arrow.SetActive(false); // 처음에는 비활성화
                arrowPool.Add(arrow);

                // **충돌 감지 스크립트 추가**
                if (arrow.GetComponent<Arrow>().weapon == null)
                {
                    arrow.GetComponent<Arrow>().weapon = this;
                }
            }
        }
    }

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
    }

    public void RangeUse(int Id)
    {
        ArrowId = Id;
        if (type == Type.Range)
        {
            StopCoroutine("Shot");
            StartCoroutine("Shot");
        }
    }

    public void ActivateCounterAttack()
    {
        // 카운터 공격 활성화
        isCounterAttack = true;
        damage = 50; // 카운터 공격 시 데미지 변경 (예시로 50으로 설정)
    }

    public void DeactivateCounterAttack()
    {
        // 카운터 공격 비활성화
        isCounterAttack = false;
        damage = 20; // 기본 데미지로 되돌리기 (예시로 20으로 설정)
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f); // 1프레임 대기
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.3f); // 1프레임 대기
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f); // 1프레임 대기
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        yield return null;

        // 원거리 무기일 때만 실행
        if (type == Type.Range)
        {
            Debug.Log("Weapon USE : Range");
            // 오브젝트 풀에서 화살을 가져옴
            GameObject instantArrow = GetArrowFromPool();
            instantArrow.transform.position = ArrowPos.position;
            instantArrow.transform.rotation = ArrowPos.rotation;

            instantArrow.GetComponent<Arrow>().arrowId = ArrowId;
            Rigidbody arrowRigid = instantArrow.GetComponent<Rigidbody>();
            arrowRigid.velocity = ArrowPos.forward * 10;

            // 화살이 벽에 부딪히거나 일정 시간이 지나면 풀로 반환
            StartCoroutine(ReturnArrowToPool(instantArrow));
        }

        yield return null;
    }

    // 비활성화된 화살을 풀에서 꺼내는 함수
    private GameObject GetArrowFromPool()
    {
        Debug.Log("IN GetArrowFromPool : Range");
        foreach (var arrow in arrowPool)
        {
            if (!arrow.activeInHierarchy)
            {
                arrow.SetActive(true); // 활성화
                Debug.Log($"OUT GetArrowFromPool {arrow} : Range 이미 있는 화살");
                return arrow;
            }
        }

        // 만약 비활성화된 화살이 없다면 새로 생성
        GameObject newArrow = Instantiate(ArrowPrefab);
        arrowPool.Add(newArrow);

        if (newArrow.GetComponent<Arrow>() == null)
        {
            newArrow.AddComponent<Arrow>().weapon = this;
        }

        Debug.Log($"OUT GetArrowFromPool {newArrow} : Range 부족해 생성한 화살");
        return newArrow;
    }

    // 사용이 끝난 화살을 풀에 반환하는 함수
    IEnumerator ReturnArrowToPool(GameObject arrow)
    {
        Debug.Log("ReturnArrowToPool 코루틴 시작");
        yield return new WaitForSeconds(3f);

        if (arrow != null)
        {
            arrow.SetActive(false);
        }
    }

    public void ReturnArrow(GameObject arrow)
    {
        StartCoroutine(ReturnArrowToPool(arrow));
    }
}
