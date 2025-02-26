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

    // �Ӹ� ��ġ�� �޾� ������ ǥ��
    public void ShowDamage(int damage, Vector3 headPosition, bool isPlayerHit, float textSize)
    {
        damageText.text = damage.ToString();
        damageText.color = isPlayerHit ? Color.gray : Color.red;
        damageText.fontSize = textSize; // �ؽ�Ʈ ũ�� ����


        transform.position = headPosition; // �Ӹ� ��ġ�� ǥ��
        StartCoroutine(AnimateDamageText());
    }

    private IEnumerator AnimateDamageText()
    {
        Vector3 startPos = transform.position;
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
