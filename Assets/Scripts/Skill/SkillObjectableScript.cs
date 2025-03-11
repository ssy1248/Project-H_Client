using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Object/Skill Data")]
public class SkillObjectableScript : ScriptableObject
{
    // 스킬 아이디
    public int SkillId;
    // 스킬 스프라이트
    public Sprite SkillImage;
    // 스킬 타입 (1: 단일, 2: 범위, 3: 버프, 4: 디버프)
    public int SkillType;
    // 스킬 이름
    public string SkillName;
    // 스킬 설명
    public string SkillDescription;
    // 스킬 데미지
    public int SkillDamage;
    // 스킬 쿨타임
    public int SkillCoolDown;
    // 스킬 소비 마나
    public int SkillCost;
    // 스킬 범위
    public int SkillRange;
    // 스킬 지속시간
    public int SkillDuration;
}
