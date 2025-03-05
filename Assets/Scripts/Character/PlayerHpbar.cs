using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthSlider; // �÷��̾� ü���� ǥ���� �����̴�
    public TextMeshProUGUI healthText; // �÷��̾� ü�� �ؽ�Ʈ

    private int maxHealth;
    private int currentHealth;
    private bool isAnimating = false; // �ִϸ��̼� ���� ���� üũ

    void Start()
    {
        maxHealth = 100; // ���÷� �ִ� ü���� 100���� ����
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        UpdateHealthText(); // �ؽ�Ʈ ������Ʈ
    }

    public void TakeDamage(int damage)
    {
        if (isAnimating) return; // �ִϸ��̼� ���̸� �ߺ� ���� ����

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(AnimateHealthReduction(0)); // ü���� 0�� �Ǹ� �ִϸ��̼� ����
            return;
        }

        StartCoroutine(AnimateHealthReduction(currentHealth)); // ü�� �ִϸ��̼� ����
    }

    private IEnumerator AnimateHealthReduction(int targetValue)
    {
        isAnimating = true; // �ִϸ��̼� ����

        float duration = 0.5f; // �ִϸ��̼� ���� �ð�
        float elapsedTime = 0f;
        float startValue = healthSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            yield return null;
        }

        healthSlider.value = targetValue; // ���� �� ����

        isAnimating = false; // �ִϸ��̼� ����

        UpdateHealthText(); // ü�� �ؽ�Ʈ ������Ʈ
    }

    // �÷��̾� ü�� �ؽ�Ʈ ������Ʈ
    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}
