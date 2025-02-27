using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    public GameObject arrowPrefab; // ȭ�� ������
    public int poolSize = 10; // Ǯ ũ��
    private List<GameObject> arrowPool; // Ǯ�� ������ ����Ʈ

    private void Awake()
    {
        // Ǯ �ʱ�ȭ
        arrowPool = new List<GameObject>();

        // �ʱ�ȭ�� Ǯ�� ȭ���� �߰�
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false); // ó������ ��Ȱ��ȭ
            arrowPool.Add(arrow);
        }
    }

    // ��Ȱ��ȭ�� ȭ���� ��ȯ�ϰ� ����ϴ� �Լ�
    public GameObject GetArrow()
    {
        foreach (var arrow in arrowPool)
        {
            if (!arrow.activeInHierarchy)
            {
                arrow.SetActive(true); // Ȱ��ȭ
                return arrow;
            }
        }

        // ���� ��Ȱ��ȭ�� ȭ���� ���ٸ� ���� �����Ͽ� ��ȯ
        GameObject newArrow = Instantiate(arrowPrefab);
        arrowPool.Add(newArrow);
        return newArrow;
    }

    // ����� ���� ȭ���� Ǯ�� ��ȯ�ϴ� �Լ�
    public void ReturnArrow(GameObject arrow)
    {
        arrow.SetActive(false); // ��Ȱ��ȭ
    }
}
