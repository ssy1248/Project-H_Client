using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider healthBar;        // 보스 HP 바
    public Image healthBarFill;     // HP 바 Fill 이미지 (색상 변경을 위한)
    public Image healthBarBackground; // HP 바 배경 이미지 (색상 변경을 위한)
    public float maxHealth = 100f;  // 보스 최대 체력
    private float currentHealth;    // 보스 현재 체력

    private Color[] healthColors;   // 각 체력 단계에 해당하는 색상들 (배경과 Fill 색상)
    private int colorIndex = 0;     // 현재 색상 단계

    void Start()
    {
        currentHealth = maxHealth;  // 시작 시 보스의 체력은 최대 체력
        healthBar.maxValue = maxHealth;  // 슬라이더의 최대 값 설정
        healthBar.value = currentHealth;  // 슬라이더의 현재 값 설정

        // 각 색상 단계에 대한 정의 (배경 색상과 Fill 색상)
        healthColors = new Color[]
        {
            Color.red,     // 1단계: 빨간색
            Color.yellow,  // 2단계: 노란색
            Color.green,   // 3단계: 초록색
            Color.blue     // 4단계: 파란색
        };

        // 처음에 배경과 Fill 색상을 빨간색으로 설정
        UpdateHealthBarColor();
    }

    void Update()
    {
        // 체력이 0에서 최대 체력까지 감소한다고 가정
        // 테스트를 위해 체력을 조금씩 감소시키는 코드 추가
        if (currentHealth > 0)
        {
            currentHealth -= Time.deltaTime * 10f;  // 10초마다 체력이 감소
            healthBar.value = currentHealth;        // HP 바 값 갱신
            UpdateHealthBarColor();                 // HP 바 색상 업데이트
        }
    }

    // 체력에 따라 HP 바 색상 및 배경 색상 변경
    void UpdateHealthBarColor()
    {
        // 체력 비율 계산 (0~1 범위)
        float healthPercentage = currentHealth / maxHealth;

        // 체력에 따라 색상 단계를 결정
        if (healthPercentage > 0.75f)
        {
            colorIndex = 0;  // 75% 이상: 빨간색
        }
        else if (healthPercentage > 0.5f)
        {
            colorIndex = 1;  // 50% 이상: 노란색
        }
        else if (healthPercentage > 0.25f)
        {
            colorIndex = 2;  // 25% 이상: 초록색
        }
        else
        {
            colorIndex = 3;  // 25% 이하: 파란색
        }

        // Fill 색상과 배경 색상 변경
        healthBarFill.color = healthColors[colorIndex];
        healthBarBackground.color = healthColors[colorIndex];
    }
}