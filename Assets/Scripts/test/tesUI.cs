using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tesUI : MonoBehaviour
{
    // Inspector에서 할당할 UI 패널 (예: Canvas의 하위 UI 오브젝트)
    public GameObject uiPanel;

    // 오프셋 값을 Inspector에서 직접 수정할 수 있도록 public 변수로 선언합니다.
    public Vector3 offset = new Vector3(0.5f, 0.5f, -0.6f);

    void Update()
    {
        // 오른쪽 클릭 감지 (Input.GetMouseButtonDown(1): 0은 왼쪽, 1은 오른쪽 클릭)
        if (Input.GetMouseButtonDown(1))
        {
            // 메인 카메라에서 현재 마우스 위치로 Ray(광선)을 생성합니다.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast로 오브젝트를 검출 (충돌한 오브젝트가 있다면 hit 변수에 정보가 담깁니다)
            if (Physics.Raycast(ray, out hit))
            {
                // 검출한 오브젝트의 태그가 "Player"인지 확인합니다.
                if (hit.transform.CompareTag("Player"))
                {
                    // UI 패널이 할당되어 있다면 활성화합니다.
                    if (uiPanel != null)
                    {
                        uiPanel.SetActive(!uiPanel.activeSelf);

                        // 플레이어 위치(hit.transform.position)에 오프셋(offset)을 더하여 UI 패널의 위치를 설정합니다.
                        uiPanel.transform.position = hit.transform.position + offset;
                    }
                    else
                    {
                        Debug.LogWarning("UI Panel이 할당되지 않았습니다!");
                    }
                }
            }
        }
    }
}
