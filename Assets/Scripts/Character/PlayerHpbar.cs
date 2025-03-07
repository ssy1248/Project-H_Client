using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthSlider; // 플레이어 체력을 표시할 슬라이더
    public Slider partyHealthSlider; // 파티 UI에서 표시될 내 HP 바
    public TextMeshProUGUI healthText; // 플레이어 체력 텍스트

    private int maxHealth;
    private int currentHealth;
    private bool isAnimating = false; // 애니메이션 실행 여부 체크

    public PlayerController playerController;
    private PartyHealthBar partyHealthBar;

    void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("playerController 찾을 수 없습니다! 씬에 존재하는지 확인하세요.");
                return;
            }
        }

        maxHealth = playerController.maxHealth;
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        // 파티 UI에서 내 HP 바를 자동으로 찾음 (없으면 인스펙터에서 수동 연결)
        if (partyHealthSlider == null)
        {
            PartyHealthBar partyHealthBar = FindObjectOfType<PartyHealthBar>();
            if (partyHealthBar != null)
            {
                partyHealthSlider = partyHealthBar.GetMyHealthBar(); // 내 캐릭터의 파티 UI HP 바 가져오기
                if (partyHealthSlider != null)
                {
                    partyHealthSlider.maxValue = maxHealth;
                    partyHealthSlider.value = maxHealth;
                }
                if (partyHealthSlider == null)
                {
                    Debug.LogError("partyHealthSlider 찾을 수 없습니다! 씬에 존재하는지 확인하세요.");
                    return;
                }
            }
        }

        UpdateHealthText();
    }

    public void TakeDamage(int damage)
    {
        if (isAnimating) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(AnimateHealthReduction(0));
            return;
        }

        StartCoroutine(AnimateHealthReduction(currentHealth));
    }

    private IEnumerator AnimateHealthReduction(int targetValue)
    {
        isAnimating = true;

        float duration = 0.5f;
        float elapsedTime = 0f;
        float startValue = healthSlider.value;
        float startPartyValue = partyHealthSlider != null ? partyHealthSlider.value : startValue;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            healthSlider.value = newValue;

            if (partyHealthSlider != null)
            {
                partyHealthSlider.value = Mathf.Lerp(startPartyValue, targetValue, elapsedTime / duration);
            }

            yield return null;
        }

        healthSlider.value = targetValue;
        if (partyHealthSlider != null)
        {
            partyHealthSlider.value = targetValue;
        }

        isAnimating = false;
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}
