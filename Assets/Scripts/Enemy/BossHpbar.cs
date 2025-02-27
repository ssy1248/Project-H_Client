using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthSlider; // 체력을 표시할 슬라이더
    public Image fillImage; // 현재 체력바 색상
    public Image backgroundImage; // 배경색 변경
    public Color[] stageColors = { Color.red, new Color(1f, 0.5f, 0f), Color.green, new Color(0.6f, 0f, 0.8f), Color.blue }; // 빨강 → 주황 → 초록 → 보라 → 파랑
    public Color lastStageColor = Color.black; // 마지막 10% 배경색

    private int maxHealth = 3000;
    private int currentHealth;
    private int stageHealth; // 10% 단위 체력
    private int currentStageIndex = 0;
    private bool isAnimating = false; // 애니메이션 실행 여부 체크

    public TextMeshProUGUI healthText; // 보스 체력 텍스트

    void Start()
    {
        currentHealth = maxHealth;
        stageHealth = maxHealth / stageColors.Length; // 10% 단위 계산
        healthSlider.maxValue = stageHealth;
        healthSlider.value = stageHealth;

        fillImage.color = stageColors[currentStageIndex]; // 첫 Fill Image 색상
        backgroundImage.color = stageColors[(currentStageIndex + 1) % stageColors.Length]; // 첫 배경색

        // 5초마다 체력 100 감소
        InvokeRepeating("AutoDamage", 0.5f, 0.5f);

        UpdateHealthText(); // 텍스트 업데이트
    }

    public void TakeDamage(int damage)
    {
        if (isAnimating) return; // 애니메이션 중이면 중복 실행 방지

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // 💡 체력을 바로 0으로 설정하지 않고, 마지막 애니메이션 실행 후 처리
            StartCoroutine(AnimateLastHealthReduction());
            return;
        }

        int stageRemainder = currentHealth % stageHealth;
        int newStageIndex = (maxHealth - currentHealth) / stageHealth;

        // 애니메이션 실행
        StartCoroutine(AnimateHealthReduction(stageRemainder, newStageIndex));
    }

    private IEnumerator AnimateHealthReduction(int targetValue, int newStageIndex)
    {
        isAnimating = true; // 애니메이션 시작

        float duration = 0.5f; // 애니메이션 지속 시간
        float elapsedTime = 0f;
        float startValue = healthSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            yield return null;
        }

        healthSlider.value = targetValue; // 최종 값 보정

        if (newStageIndex != currentStageIndex)
        {
            currentStageIndex = newStageIndex % stageColors.Length;
            UpdateHealthBarColors();
        }

        isAnimating = false; // 애니메이션 종료

        UpdateHealthText(); // 체력 텍스트 업데이트
    }

    private void UpdateHealthBarColors()
    {
        fillImage.color = backgroundImage.color; // 배경색이었던 것을 Fill Image로
        healthSlider.value = healthSlider.maxValue; // 새로운 체력바는 꽉 찬 상태에서 시작

        if (currentStageIndex == stageColors.Length - 1)
        {
            backgroundImage.color = lastStageColor; // 마지막 10%에서는 검은색 고정
        }
        else
        {
            int nextIndex = (currentStageIndex + 1) % stageColors.Length;
            backgroundImage.color = stageColors[nextIndex];
        }
    }

    private IEnumerator AnimateLastHealthReduction()
    {
        isAnimating = true; // 애니메이션 시작

        float duration = 0.5f;
        float elapsedTime = 0f;
        float startValue = healthSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startValue, 0f, elapsedTime / duration);
            yield return null;
        }

        healthSlider.value = 0f; // 마지막 체력 0으로 설정
        currentHealth = 0; // 💡 이제 실제 체력을 0으로 설정
        Debug.Log("보스 처치됨!"); // 💡 애니메이션이 끝난 후 메시지 출력

        isAnimating = false; // 애니메이션 종료

        UpdateHealthText(); // 체력 텍스트 업데이트
    }

    private void AutoDamage()
    {
        TakeDamage(100);
    }

    // 보스 체력 텍스트 업데이트
    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}
