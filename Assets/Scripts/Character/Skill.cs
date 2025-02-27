using UnityEngine;

public class Skill : MonoBehaviour
{
    public int damage = 50; // 기본 데미지 값, 필요에 따라 설정 가능

    // 스킬 데미지 처리
    public void ApplySkillDamage(GameObject target)
    {
        if (target.CompareTag("Enemy"))
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ReceiveDamage(damage); // Enemy에서 데미지 받는 함수 호출
            }
        }
    }

    // 데미지 값을 설정하는 메서드
    public void SetDamage(int newDamage)
    {
        damage = newDamage; // 데미지 값 설정
    }
}
