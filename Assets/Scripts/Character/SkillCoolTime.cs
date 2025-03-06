using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCoolTime : MonoBehaviour
{
    public GameObject[] hideSkillButtons;
    public GameObject[] TMP;
    public TextMeshProUGUI[] hideSkillTimeTexts;
    public Image[] hideSkillImages;
    private bool[] isHideSkills = { false, false };
    private float[] SkillTimes = { 3, 5 }; // 각 스킬의 쿨타임 (초 단위)
    private float[] getSkillTimes = { 0, 0 }; // 쿨타임을 추적하는 변수

    // Start is called before the first frame update
    void Start()
    {
        // 초기화: 텍스트, 버튼 상태 설정
        for (int i = 0; i < TMP.Length; i++)
        {
            hideSkillTimeTexts[i] = TMP[i].GetComponent<TextMeshProUGUI>();
            hideSkillButtons[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 키 입력 체크: Q(0)와 Space(1) 눌렀을 때 해당 스킬 시작
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ActivateSkill(0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateSkill(1);
        }

        // 쿨타임 상태 업데이트
        HideSkillCheck();
    }

    // 키 입력에 따라 스킬 활성화 함수
    private void ActivateSkill(int skillNum)
    {
        if (!isHideSkills[skillNum]) // 이미 활성화된 스킬이면 실행되지 않음
        {
            HideSkillSetting(skillNum); // 스킬 설정
        }
    }

    // 스킬 활성화 및 쿨타임 시작
    public void HideSkillSetting(int skillNum)
    {
        hideSkillButtons[skillNum].SetActive(true); // 버튼 활성화
        getSkillTimes[skillNum] = SkillTimes[skillNum]; // 초기화
        isHideSkills[skillNum] = true; // 쿨타임 시작 표시
    }

    // 쿨타임 체크
    private void HideSkillCheck()
    {
        if (isHideSkills[0])
        {
            StartCoroutine(SkillTimeCheck(0)); // 1번 스킬
        }

        if (isHideSkills[1])
        {
            StartCoroutine(SkillTimeCheck(1)); // 2번 스킬
        }
    }

    // 쿨타임 관리 (타이머)
    IEnumerator SkillTimeCheck(int skillNum)
    {
        yield return null;

        if (getSkillTimes[skillNum] > 0)
        {
            getSkillTimes[skillNum] -= Time.deltaTime; // 시간 감소

            if (getSkillTimes[skillNum] < 0)
            {
                getSkillTimes[skillNum] = 0;
                isHideSkills[skillNum] = false; // 쿨타임 종료
                hideSkillButtons[skillNum].SetActive(false); // 버튼 비활성화
            }

            hideSkillTimeTexts[skillNum].text = getSkillTimes[skillNum].ToString("0"); // 남은 시간 표시

            float time = getSkillTimes[skillNum] / SkillTimes[skillNum];
            hideSkillImages[skillNum].fillAmount = time; // 이미지 채우기
        }
    }
}
