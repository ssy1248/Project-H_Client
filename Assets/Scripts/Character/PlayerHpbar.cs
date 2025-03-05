using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthSlider; // 플레이어 체력을 표시할 슬라이더
    public TextMeshProUGUI healthText; // 플레이어 체력 텍스트

    private int maxHealth;
    private int currentHealth;
    private bool isAnimating = false; // 애니메이션 실행 여부 체크

    void Start()
    {
        maxHealth = 100; // 예시로 최대 체력을 100으로 설정
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        UpdateHealthText(); // 텍스트 업데이트
    }

    public void TakeDamage(int damage)
    {
        if (isAnimating) return; // 애니메이션 중이면 중복 실행 방지

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(AnimateHealthReduction(0)); // 체력이 0이 되면 애니메이션 실행
            return;
        }

        StartCoroutine(AnimateHealthReduction(currentHealth)); // 체력 애니메이션 실행
    }

    private IEnumerator AnimateHealthReduction(int targetValue)
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

        isAnimating = false; // 애니메이션 종료

        UpdateHealthText(); // 체력 텍스트 업데이트
    }

    // 플레이어 체력 텍스트 업데이트
    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}
