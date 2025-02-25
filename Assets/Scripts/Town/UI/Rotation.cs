using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Rotation : MonoBehaviour
{
    public float cycleDuration = 1f; // �� ����Ŭ(��� ���� ������� ������� �� �ɸ��� �ð�)
    private Image[] dots;
    [SerializeField] TextMeshProUGUI matchText;
    [SerializeField] private float scaleMin = 0.9f;    // �ּ� ������
    [SerializeField] private float scaleMax = 1.1f;    // �ִ� ������

    void Start()
    {
        // �ڽ� Image ������Ʈ�� ���� ã��
        dots = GetComponentsInChildren<Image>();
        matchText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        // ���� �ð��� cycleDuration���� ���� ����
        float t = (Time.time % cycleDuration) / cycleDuration;

        // �� ���� �ε����� ���� �ణ�� �ð����� �༭ ���İ��� ���
        for (int i = 0; i < dots.Length; i++)
        {
            // ��: i��° ���� ���� �ð��� offset
            float offset = (float)i / dots.Length;
            // (t + offset)�� 1�� �Ѿ�� �ٽ� 0���� �����ϰ� �������
            float alphaTime = (t + offset) % 1f;

            // ���İ��� 0~1�� �ְ� ������
            // 0~0.5 ���������� �������, 0.5~1 ���������� ��ο�����
            float alpha = alphaTime < 0.5f ? alphaTime * 2f : 2f - (alphaTime * 2f);

            // ���� ���� ���� ����
            Color c = dots[i].color;
            c.a = alpha;
            dots[i].color = c;
        }

        // t�� 0~0.5�� ���� (scaleMin �� scaleMax)��, 0.5~1�� ���� (scaleMax �� scaleMin)���� ����
        float scale;
        if (t < 0.5f)
        {
            // 0 ~ 0.5 ����
            float progress = t / 0.5f;  // 0 ~ 1
            scale = Mathf.Lerp(scaleMin, scaleMax, progress);
        }
        else
        {
            // 0.5 ~ 1 ����
            float progress = (t - 0.5f) / 0.5f; // 0 ~ 1
            scale = Mathf.Lerp(scaleMax, scaleMin, progress);
        }

        // ���� RectTransform�� ������ ����
        matchText.rectTransform.localScale = new Vector3(scale, scale, 1f);
    }
}
