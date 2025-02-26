using UnityEngine;

public class Skill : MonoBehaviour
{
    public int damage = 50; // �⺻ ������ ��, �ʿ信 ���� ���� ����

    // ��ų ������ ó��
    public void ApplySkillDamage(GameObject target)
    {
        if (target.CompareTag("Enemy"))
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ReceiveDamage(damage); // Enemy���� ������ �޴� �Լ� ȣ��
            }
        }
    }

    // ������ ���� �����ϴ� �޼���
    public void SetDamage(int newDamage)
    {
        damage = newDamage; // ������ �� ����
    }
}
