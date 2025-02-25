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

        // �÷��̾� �ǰ� �� ȸ��, ���� �ǰ� �� �����
        damageText.color = isPlayerHit ? Color.gray : Color.yellow;

        transform.position = position;
        StartCoroutine(AnimateDamageText());
    }

    private IEnumerator AnimateDamageText()
    {
        Vector3 startPos = transform.position;
        startPos.y += 15f; // ���ϴ� ��ŭ y���� �ø�
        Vector3 endPos = startPos + new Vector3(0, 1f, 0); // ���� ��¦ �̵�

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / fadeDuration);
            damageText.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        // �ִϸ��̼� ���� �� Ǯ������ ��ȯ
        DamageTextPool.Instance.ReturnDamageText(gameObject);
    }
}
