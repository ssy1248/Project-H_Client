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
    public GameObject[] skillButton;
    private bool[] isHideSkills = { false, false, false, false, false };
    private float[] SkillTimes = { 10, 5, 7, 5, 10 }; // 각 스킬의 쿨타임 (초 단위)
    private float[] getSkillTimes = { 0, 0, 0, 0, 0 }; // 쿨타임을 추적하는 변수

    private string playerTag;

    // Start is called before the first frame update
    void Start()
    {
        //태그별 캐릭터 찾기
        string[] tags = { "Rogue", "Archer", "Spearman" };
        GameObject player = null;

        foreach (string tag in tags)
        {
            GameObject foundPlayer = GameObject.FindWithTag(tag);
            if (foundPlayer != null)
            {
                player = foundPlayer;
                playerTag = tag;
                break;
            }
        }

        // 초기화: 텍스트, 버튼 상태 설정
        for (int i = 0; i < TMP.Length; i++)
        {
            hideSkillTimeTexts[i] = TMP[i].GetComponent<TextMeshProUGUI>();
            hideSkillButtons[i].SetActive(false);
        }

        //캐릭터별 버튼 활성화
        SetSkillButtonsByJob();
    }

    // Update is called once per frame
    void Update()
    {
        // 키 입력 체크: Q(0)와 Space(1) 눌렀을 때 해당 스킬 시작
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ActivateSkill(0);
            ActivateSkill(2);
            ActivateSkill(4);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateSkill(1);
            ActivateSkill(3);
        }

        // 쿨타임 상태 업데이트
        HideSkillCheck();
    }

    private void SetSkillButtonsByJob()
    {
        for (int i = 0; i < skillButton.Length; i++)
        {
            skillButton[i].SetActive(false); // 일단 모두 비활성화
        }

        if (playerTag == "Rogue") // 로그일 경우 0,1번 스킬 활성화
        {
            skillButton[0].SetActive(true);
            skillButton[1].SetActive(true);
        }
        else if (playerTag == "Archer") // 아처일 경우 2,3번 스킬 활성화
        {
            skillButton[2].SetActive(true);
            skillButton[3].SetActive(true);
        }
        else if (playerTag == "Spearman") // 스피어맨일 경우 4번 스킬만 활성화
        {
            skillButton[4].SetActive(true);
        }
    }

    private void ActivateSkill(int skillNum)
    {
        // 🚨 직업별로 사용 가능한 스킬인지 확인
        if ((playerTag == "Rogue" && (skillNum == 0 || skillNum == 1)) ||
            (playerTag == "Archer" && (skillNum == 2 || skillNum == 3)) ||
            (playerTag == "Spearman" && skillNum == 4))
        {
            if (!isHideSkills[skillNum])
            {
                HideSkillSetting(skillNum);
            }
        }
    }

    //// 키 입력에 따라 스킬 활성화 함수
    //private void ActivateSkill(int skillNum)
    //{
    //    if (!isHideSkills[skillNum]) // 이미 활성화된 스킬이면 실행되지 않음
    //    {
    //        HideSkillSetting(skillNum); // 스킬 설정
    //    }
    //}

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

        if (isHideSkills[2])
        {
            StartCoroutine(SkillTimeCheck(2)); // 3번 스킬
        }

        if (isHideSkills[3])
        {
            StartCoroutine(SkillTimeCheck(3)); // 4번 스킬
        }

        if (isHideSkills[4])
        {
            StartCoroutine(SkillTimeCheck(4)); // 5번 스킬
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
