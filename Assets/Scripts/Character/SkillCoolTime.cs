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
    private float[] SkillTimes = { 3, 5 }; // �� ��ų�� ��Ÿ�� (�� ����)
    private float[] getSkillTimes = { 0, 0 }; // ��Ÿ���� �����ϴ� ����

    // Start is called before the first frame update
    void Start()
    {
        // �ʱ�ȭ: �ؽ�Ʈ, ��ư ���� ����
        for (int i = 0; i < TMP.Length; i++)
        {
            hideSkillTimeTexts[i] = TMP[i].GetComponent<TextMeshProUGUI>();
            hideSkillButtons[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Ű �Է� üũ: Q(0)�� Space(1) ������ �� �ش� ��ų ����
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ActivateSkill(0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateSkill(1);
        }

        // ��Ÿ�� ���� ������Ʈ
        HideSkillCheck();
    }

    // Ű �Է¿� ���� ��ų Ȱ��ȭ �Լ�
    private void ActivateSkill(int skillNum)
    {
        if (!isHideSkills[skillNum]) // �̹� Ȱ��ȭ�� ��ų�̸� ������� ����
        {
            HideSkillSetting(skillNum); // ��ų ����
        }
    }

    // ��ų Ȱ��ȭ �� ��Ÿ�� ����
    public void HideSkillSetting(int skillNum)
    {
        hideSkillButtons[skillNum].SetActive(true); // ��ư Ȱ��ȭ
        getSkillTimes[skillNum] = SkillTimes[skillNum]; // �ʱ�ȭ
        isHideSkills[skillNum] = true; // ��Ÿ�� ���� ǥ��
    }

    // ��Ÿ�� üũ
    private void HideSkillCheck()
    {
        if (isHideSkills[0])
        {
            StartCoroutine(SkillTimeCheck(0)); // 1�� ��ų
        }

        if (isHideSkills[1])
        {
            StartCoroutine(SkillTimeCheck(1)); // 2�� ��ų
        }
    }

    // ��Ÿ�� ���� (Ÿ�̸�)
    IEnumerator SkillTimeCheck(int skillNum)
    {
        yield return null;

        if (getSkillTimes[skillNum] > 0)
        {
            getSkillTimes[skillNum] -= Time.deltaTime; // �ð� ����

            if (getSkillTimes[skillNum] < 0)
            {
                getSkillTimes[skillNum] = 0;
                isHideSkills[skillNum] = false; // ��Ÿ�� ����
                hideSkillButtons[skillNum].SetActive(false); // ��ư ��Ȱ��ȭ
            }

            hideSkillTimeTexts[skillNum].text = getSkillTimes[skillNum].ToString("0"); // ���� �ð� ǥ��

            float time = getSkillTimes[skillNum] / SkillTimes[skillNum];
            hideSkillImages[skillNum].fillAmount = time; // �̹��� ä���
        }
    }
}
