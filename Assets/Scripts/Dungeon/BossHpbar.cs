using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider healthBar;        // ���� HP ��
    public Image healthBarFill;     // HP �� Fill �̹��� (���� ������ ����)
    public Image healthBarBackground; // HP �� ��� �̹��� (���� ������ ����)
    public float maxHealth = 100f;  // ���� �ִ� ü��
    private float currentHealth;    // ���� ���� ü��

    private Color[] healthColors;   // �� ü�� �ܰ迡 �ش��ϴ� ����� (���� Fill ����)
    private int colorIndex = 0;     // ���� ���� �ܰ�

    void Start()
    {
        currentHealth = maxHealth;  // ���� �� ������ ü���� �ִ� ü��
        healthBar.maxValue = maxHealth;  // �����̴��� �ִ� �� ����
        healthBar.value = currentHealth;  // �����̴��� ���� �� ����

        // �� ���� �ܰ迡 ���� ���� (��� ����� Fill ����)
        healthColors = new Color[]
        {
            Color.red,     // 1�ܰ�: ������
            Color.yellow,  // 2�ܰ�: �����
            Color.green,   // 3�ܰ�: �ʷϻ�
            Color.blue     // 4�ܰ�: �Ķ���
        };

        // ó���� ���� Fill ������ ���������� ����
        UpdateHealthBarColor();
    }

    void Update()
    {
        // ü���� 0���� �ִ� ü�±��� �����Ѵٰ� ����
        // �׽�Ʈ�� ���� ü���� ���ݾ� ���ҽ�Ű�� �ڵ� �߰�
        if (currentHealth > 0)
        {
            currentHealth -= Time.deltaTime * 10f;  // 10�ʸ��� ü���� ����
            healthBar.value = currentHealth;        // HP �� �� ����
            UpdateHealthBarColor();                 // HP �� ���� ������Ʈ
        }
    }

    // ü�¿� ���� HP �� ���� �� ��� ���� ����
    void UpdateHealthBarColor()
    {
        // ü�� ���� ��� (0~1 ����)
        float healthPercentage = currentHealth / maxHealth;

        // ü�¿� ���� ���� �ܰ踦 ����
        if (healthPercentage > 0.75f)
        {
            colorIndex = 0;  // 75% �̻�: ������
        }
        else if (healthPercentage > 0.5f)
        {
            colorIndex = 1;  // 50% �̻�: �����
        }
        else if (healthPercentage > 0.25f)
        {
            colorIndex = 2;  // 25% �̻�: �ʷϻ�
        }
        else
        {
            colorIndex = 3;  // 25% ����: �Ķ���
        }

        // Fill ����� ��� ���� ����
        healthBarFill.color = healthColors[colorIndex];
        healthBarBackground.color = healthColors[colorIndex];
    }
}