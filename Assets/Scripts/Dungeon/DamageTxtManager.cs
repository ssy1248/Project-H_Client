using UnityEngine;
using TMPro;
using System.Collections;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    private RectTransform rectTransform;
    private float moveSpeed = 1f;
    private float fadeDuration = 0.5f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void ShowDamage(int damage, Vector3 position, bool isPlayerHit)
    {
        damageText.text = damage.ToString();

        // 플레이어 피격 → 회색, 몬스터 피격 → 노란색
        damageText.color = isPlayerHit ? Color.gray : Color.yellow;

        transform.position = position;
        StartCoroutine(AnimateDamageText());
    }

    private IEnumerator AnimateDamageText()
    {
        Vector3 startPos = transform.position;
        startPos.y += 15f; // 원하는 만큼 y축을 올림
        Vector3 endPos = startPos + new Vector3(0, 1f, 0); // 위로 살짝 이동

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / fadeDuration);
            damageText.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        // 애니메이션 종료 후 풀링으로 반환
        DamageTextPool.Instance.ReturnDamageText(gameObject);
    }
}
