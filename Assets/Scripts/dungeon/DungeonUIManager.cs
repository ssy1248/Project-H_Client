using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUIManager : MonoBehaviour
{
    private static DungeonUIManager _instance;
    public static DungeonUIManager Instance => _instance;

    // �÷��̾� ����ġ �����̴�
    [SerializeField] public Slider PlayerExpSlider;
    // �÷��̾� ���� �ؽ�Ʈ
    [SerializeField] public TextMeshProUGUI PlayerLevelText;
    // �÷��̾� ü�� �ؽ�Ʈ
    [SerializeField] public TextMeshProUGUI PlayerHpText;
    // �÷��̾� ���� �ؽ�Ʈ
    [SerializeField] public TextMeshProUGUI PlayerMpText;
    //�÷��̾� ������
    [SerializeField] public GameObject[] PlayerIcon;
    [SerializeField] public GameObject[] PlayerSkillIcon;
    [SerializeField] public GameObject[] PlayerDodgeIcon;
    public GameOverUI gameOverUI;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
