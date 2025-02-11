using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleFadeIn : MonoBehaviour
{
    [SerializeField] private CanvasGroup titleCanvasGroup; // CanvasGroup�� ���� ��ü ���̵� ����

    private void Start()
    {
        titleCanvasGroup.alpha = 0; // �ʱ� ���� ����
        StartCoroutine(FadeInTitle());
    }

    private IEnumerator FadeInTitle()
    {
        float duration = 2.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            titleCanvasGroup.alpha = alpha; // ���� ����
            yield return null;
        }
    }
}
