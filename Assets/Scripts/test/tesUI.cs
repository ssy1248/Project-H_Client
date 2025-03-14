using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tesUI : MonoBehaviour
{
    // Inspector���� �Ҵ��� UI �г� (��: Canvas�� ���� UI ������Ʈ)
    public GameObject uiPanel;

    // ������ ���� Inspector���� ���� ������ �� �ֵ��� public ������ �����մϴ�.
    public Vector3 offset = new Vector3(0.5f, 0.5f, -0.6f);

    void Update()
    {
        // ������ Ŭ�� ���� (Input.GetMouseButtonDown(1): 0�� ����, 1�� ������ Ŭ��)
        if (Input.GetMouseButtonDown(1))
        {
            // ���� ī�޶󿡼� ���� ���콺 ��ġ�� Ray(����)�� �����մϴ�.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast�� ������Ʈ�� ���� (�浹�� ������Ʈ�� �ִٸ� hit ������ ������ ���ϴ�)
            if (Physics.Raycast(ray, out hit))
            {
                // ������ ������Ʈ�� �±װ� "Player"���� Ȯ���մϴ�.
                if (hit.transform.CompareTag("Player"))
                {
                    // UI �г��� �Ҵ�Ǿ� �ִٸ� Ȱ��ȭ�մϴ�.
                    if (uiPanel != null)
                    {
                        uiPanel.SetActive(!uiPanel.activeSelf);

                        // �÷��̾� ��ġ(hit.transform.position)�� ������(offset)�� ���Ͽ� UI �г��� ��ġ�� �����մϴ�.
                        uiPanel.transform.position = hit.transform.position + offset;
                    }
                    else
                    {
                        Debug.LogWarning("UI Panel�� �Ҵ���� �ʾҽ��ϴ�!");
                    }
                }
            }
        }
    }
}
