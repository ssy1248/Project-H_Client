using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUIManager : MonoBehaviour
{
    private static DungeonUIManager _instance;
    public static DungeonUIManager Instance => _instance;

    // 플레이어 경험치 슬라이더
    [SerializeField] public Slider PlayerExpSlider;
    // 플레이어 레벨 텍스트
    [SerializeField] public TextMeshProUGUI PlayerLevelText;
    // 플레이어 체력 텍스트
    [SerializeField] public TextMeshProUGUI PlayerHpText;
    // 플레이어 마나 텍스트
    [SerializeField] public TextMeshProUGUI PlayerMpText;
    //플레이어 아이콘
    [SerializeField] public GameObject[] PlayerIcon;

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
