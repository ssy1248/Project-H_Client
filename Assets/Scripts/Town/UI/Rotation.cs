using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Rotation : MonoBehaviour
{
    public float cycleDuration = 1f; // 한 사이클(모든 점이 밝아졌다 흐려지는 데 걸리는 시간)
    private Image[] dots;
    [SerializeField] TextMeshProUGUI matchText;
    [SerializeField] private float scaleMin = 0.9f;    // 최소 스케일
    [SerializeField] private float scaleMax = 1.1f;    // 최대 스케일

    void Start()
    {
        // 자식 Image 컴포넌트를 전부 찾음
        dots = GetComponentsInChildren<Image>();
        matchText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        // 현재 시간을 cycleDuration으로 나눈 비율
        float t = (Time.time % cycleDuration) / cycleDuration;

        // 각 점의 인덱스에 따라 약간씩 시간차를 줘서 알파값을 계산
        for (int i = 0; i < dots.Length; i++)
        {
            // 예: i번째 점에 대해 시간차 offset
            float offset = (float)i / dots.Length;
            // (t + offset)이 1을 넘어가면 다시 0부터 시작하게 만들어줌
            float alphaTime = (t + offset) % 1f;

            // 알파값을 0~1로 주고 싶으면
            // 0~0.5 구간에서는 밝아지고, 0.5~1 구간에서는 어두워지게
            float alpha = alphaTime < 0.5f ? alphaTime * 2f : 2f - (alphaTime * 2f);

            // 점의 색상에 알파 적용
            Color c = dots[i].color;
            c.a = alpha;
            dots[i].color = c;
        }

        // t가 0~0.5일 때는 (scaleMin → scaleMax)로, 0.5~1일 때는 (scaleMax → scaleMin)으로 보간
        float scale;
        if (t < 0.5f)
        {
            // 0 ~ 0.5 구간
            float progress = t / 0.5f;  // 0 ~ 1
            scale = Mathf.Lerp(scaleMin, scaleMax, progress);
        }
        else
        {
            // 0.5 ~ 1 구간
            float progress = (t - 0.5f) / 0.5f; // 0 ~ 1
            scale = Mathf.Lerp(scaleMax, scaleMin, progress);
        }

        // 실제 RectTransform에 스케일 적용
        matchText.rectTransform.localScale = new Vector3(scale, scale, 1f);
    }
}
