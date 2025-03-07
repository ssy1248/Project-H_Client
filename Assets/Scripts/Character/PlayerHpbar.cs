using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthSlider; // �÷��̾� ü���� ǥ���� �����̴�
    public Slider partyHealthSlider; // ��Ƽ UI���� ǥ�õ� �� HP ��
    public TextMeshProUGUI healthText; // �÷��̾� ü�� �ؽ�Ʈ

    private int maxHealth;
    private int currentHealth;
    private bool isAnimating = false; // �ִϸ��̼� ���� ���� üũ

    public PlayerController playerController;
    private PartyHealthBar partyHealthBar;

    void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("playerController ã�� �� �����ϴ�! ���� �����ϴ��� Ȯ���ϼ���.");
                return;
            }
        }

        maxHealth = playerController.maxHealth;
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        // ��Ƽ UI���� �� HP �ٸ� �ڵ����� ã�� (������ �ν����Ϳ��� ���� ����)
        if (partyHealthSlider == null)
        {
            PartyHealthBar partyHealthBar = FindObjectOfType<PartyHealthBar>();
            if (partyHealthBar != null)
            {
                partyHealthSlider = partyHealthBar.GetMyHealthBar(); // �� ĳ������ ��Ƽ UI HP �� ��������
                if (partyHealthSlider != null)
                {
                    partyHealthSlider.maxValue = maxHealth;
                    partyHealthSlider.value = maxHealth;
                }
                if (partyHealthSlider == null)
                {
                    Debug.LogError("partyHealthSlider ã�� �� �����ϴ�! ���� �����ϴ��� Ȯ���ϼ���.");
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
