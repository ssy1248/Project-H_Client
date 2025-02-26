using UnityEngine;
using TMPro;
using System.Collections;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    private RectTransform rectTransform;
    private float fadeDuration = 0.5f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // 머리 위치를 받아 데미지 표시
    public void ShowDamage(int damage, Vector3 headPosition, bool isPlayerHit, float textSize)
    {
        damageText.text = damage.ToString();
        damageText.color = isPlayerHit ? Color.gray : Color.red;
        damageText.fontSize = textSize; // 텍스트 크기 적용


        transform.position = headPosition; // 머리 위치에 표시
        StartCoroutine(AnimateDamageText());
    }

    private IEnumerator AnimateDamageText()
    {
        Vector3 startPos = transform.position;
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
