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

    [SerializeField] private float loadingSpeed = 0.2f; // �ε��ٰ� ���� ���� �ӵ� (1�� �� ������ �ɸ��� �ð��� ����)
    [SerializeField] private float alphaSpeed = 2f;     // �ؽ�Ʈ ���� �ִϸ��̼� �ӵ�

    private bool isLoadingComplete = false;

    void Start()
    {
        // �ʱ�ȭ: �ε��ٴ� 0���� ����, �Ϸ� �ؽ�Ʈ�� ��Ȱ��ȭ
        LoadingBarImage.fillAmount = 0f;
        LoadingCompleteText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isLoadingComplete)
        {
            // �ε��� ä��� (0 -> 1)
            LoadingBarImage.fillAmount += Time.deltaTime * loadingSpeed;

            if (LoadingBarImage.fillAmount >= 1f)
            {
                LoadingBarImage.fillAmount = 1f;
                isLoadingComplete = true;
                // �ε� �Ϸ� �� �ؽ�Ʈ Ȱ��ȭ
                LoadingCompleteText.gameObject.SetActive(true);
            }
        }
        else
        {
            // �ε� �Ϸ� �ؽ�Ʈ�� ���İ��� 0~1 ���̿��� �ݺ� (0~255�� �ش�)
            float alpha = Mathf.PingPong(Time.time * alphaSpeed, 1f);
            Color textColor = LoadingCompleteText.color;
            textColor.a = alpha;
            LoadingCompleteText.color = textColor;
        }

        LoadingPanelDeActiveAndChangeScene(Index);
    }

    public void LoadingPanelDeActiveAndChangeScene(int index)
    {
        // ȭ�� Ŭ�� �� �ش� ���� ������Ʈ ��Ȱ��ȭ
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            gameObject.SetActive(false);
            SceneManager.LoadScene("Dungeon " + index);
        }
    }
}
