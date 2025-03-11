using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Object/Skill Data")]
public class SkillObjectableScript : ScriptableObject
{
    // ��ų ���̵�
    public int SkillId;
    // ��ų ��������Ʈ
    public Sprite SkillImage;
    // ��ų Ÿ�� (1: ����, 2: ����, 3: ����, 4: �����)
    public int SkillType;
    // ��ų �̸�
    public string SkillName;
    // ��ų ����
    public string SkillDescription;
    // ��ų ������
    public int SkillDamage;
    // ��ų ��Ÿ��
    public int SkillCoolDown;
    // ��ų �Һ� ����
    public int SkillCost;
    // ��ų ����
    public int SkillRange;
    // ��ų ���ӽð�
    public int SkillDuration;
}
