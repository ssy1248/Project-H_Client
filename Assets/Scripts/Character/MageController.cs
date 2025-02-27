using UnityEngine;
using System.Collections;

public class MageController : PlayerController
{
    public GameObject meteorPrefab; // 떨어지는 메테오 프리팹
    public GameObject explosionPrefab; // 바닥에 닿았을 때 터지는 이펙트 프리팹
    public GameObject targetingIndicator; // 바닥에 위치를 표시하는 원 오브젝트

    public float meteorFallTime = 1.5f; // 메테오가 떨어지는 시간
    public float meteorHeight = 10f; // 메테오가 시작되는 높이

    private bool isSelectingTarget = false; // 메테오 위치를 지정 중인지 확인
    private Vector3 targetPosition; // 선택된 메테오 타겟 위치

    void Update()
    {
        base.Update();

        // Q 키를 누르면 메테오 위치 지정 시작
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isSelectingTarget)
            {
                StartTargeting();
            }
            else
            {
                ConfirmTarget();
            }
        }

        // 마우스를 따라다니는 위치 표시 업데이트
        if (isSelectingTarget)
        {
            UpdateTargetingIndicator();
        }
    }

    // 1. 위치 지정 시작
    void StartTargeting()
    {
        isSelectingTarget = true;
        targetingIndicator.SetActive(true);
    }

    // 2. 마우스 위치를 계속 업데이트하여 바닥을 따라다니게 함
    void UpdateTargetingIndicator()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Floor")))
        {
            targetingIndicator.transform.position = hit.point;
        }
    }

    // 3. 최종 위치 확정 후 메테오 발동
    void ConfirmTarget()
    {
        isSelectingTarget = false;
        targetingIndicator.SetActive(false);
        targetPosition = targetingIndicator.transform.position;
        StartCoroutine(MeteorSkill(targetPosition));
    }

    // 4. 메테오 발동
    IEnumerator MeteorSkill(Vector3 position)
    {
        // 메테오 시작 위치 (위쪽에서 떨어지기 위해 Y값을 더함)
        Vector3 spawnPos = position + Vector3.up * meteorHeight;
        GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

        // 대각선 방향으로 떨어지게 할 벡터
        Vector3 direction = (position - spawnPos).normalized;

        // 이동 속도 설정
        float speed = meteorHeight / meteorFallTime;

        float elapsedTime = 0f;
        Vector3 startPos = meteor.transform.position;

        // 대각선 방향으로 이동
        while (elapsedTime < meteorFallTime)
        {
            // 대각선 방향으로 이동
            meteor.transform.position = Vector3.Lerp(startPos, position, elapsedTime / meteorFallTime) + direction * Mathf.Sin(elapsedTime * Mathf.PI * 2f / meteorFallTime) * 0.5f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        meteor.transform.position = position;

        // 바닥에 닿았을 때 폭발 이펙트
        Instantiate(explosionPrefab, position, Quaternion.identity);
        Destroy(meteor);

        Collider[] hitEnemies = Physics.OverlapSphere(position, 3f, LayerMask.GetMask("Enemy"));
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null && !enemyScript.isDead)
                {
                    int damage = 100; // 예시 데미지 값
                    enemyScript.curHealth -= damage;
                    //DamageManager.Instance.SpawnDamageText(damage, enemy.transform.position, isPlayerHit: false);
                }
            }
        }
    }
}
