using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Image LoadingBarImage;
    public TextMeshProUGUI LoadingCompleteText;
    [SerializeField] public int Index;

    [SerializeField] private float loadingSpeed = 0.2f; // 로딩바가 가득 차는 속도 (1이 될 때까지 걸리는 시간의 역수)
    [SerializeField] private float alphaSpeed = 2f;     // 텍스트 알파 애니메이션 속도

    private bool isLoadingComplete = false;

    void Start()
    {
        // 초기화: 로딩바는 0부터 시작, 완료 텍스트는 비활성화
        LoadingBarImage.fillAmount = 0f;
        LoadingCompleteText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isLoadingComplete)
        {
            // 로딩바 채우기 (0 -> 1)
            LoadingBarImage.fillAmount += Time.deltaTime * loadingSpeed;

            if (LoadingBarImage.fillAmount >= 1f)
            {
                LoadingBarImage.fillAmount = 1f;
                isLoadingComplete = true;
                // 로딩 완료 시 텍스트 활성화
                LoadingCompleteText.gameObject.SetActive(true);
            }
        }
        else
        {
            // 로딩 완료 텍스트의 알파값을 0~1 사이에서 반복 (0~255에 해당)
            float alpha = Mathf.PingPong(Time.time * alphaSpeed, 1f);
            Color textColor = LoadingCompleteText.color;
            textColor.a = alpha;
            LoadingCompleteText.color = textColor;
        }

        LoadingPanelDeActiveAndChangeScene(Index);
    }

    public void LoadingPanelDeActiveAndChangeScene(int index)
    {
        // 화면 클릭 시 해당 게임 오브젝트 비활성화
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            gameObject.SetActive(false);
            SceneManager.LoadScene("Dungeon " + index);
        }
    }
}
