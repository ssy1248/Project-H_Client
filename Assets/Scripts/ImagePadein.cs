using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleFadeIn : MonoBehaviour
{
    [SerializeField] private CanvasGroup titleCanvasGroup; // CanvasGroup을 통한 전체 페이드 제어

    private void Start()
    {
        titleCanvasGroup.alpha = 0; // 초기 투명도 설정
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
            titleCanvasGroup.alpha = alpha; // 투명도 적용
            yield return null;
        }
    }
}
