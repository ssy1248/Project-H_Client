using UnityEngine;
using UnityEngine.AI;
using System.IO;
using System.Collections;
using Unity.AI.Navigation;

public class NavMeshToJson : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;  // �׺�޽� �����̽� ������Ʈ

    // JSON ����ȭ�� �� ����� Ŭ���� ����
    [System.Serializable]
    public class NavMeshData
    {
        public Vector3[] vertices;
        public int[] indices;
    }

    void Start()
    {
        // �׺�޽ð� ������ �����ϱ�
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh(); // �׺�޽� ����
        }

        // �׺�޽� �����Ͱ� �غ�� ������ ����� �� �����͸� ����
        StartCoroutine(WaitForNavMeshAndExtract());
    }

    // �׺�޽� ���� �� �����͸� �����ϴ� �ڷ�ƾ
    private IEnumerator WaitForNavMeshAndExtract()
    {
        // �׺�޽� ���尡 �Ϸ�� ������ ��� ��ٸ�
        yield return new WaitForSeconds(2f); // 2�� ���� ��� (���� �ϷḦ ��ٸ�)

        // �׺�޽� ������ ���
        var navMeshData = NavMesh.CalculateTriangulation();

        // �׺�޽� �����Ͱ� ������� ������
        if (navMeshData.vertices.Length == 0 || navMeshData.indices.Length == 0)
        {
            Debug.LogError("�׺�޽� ������ ���� ����: ��ȿ�� �׺�޽ð� �����ϴ�.");
            yield break; // �׺�޽ð� ������ ����
        }

        // JSON���� ��ȯ�� ��ü ����
        NavMeshData data = new NavMeshData();
        data.vertices = navMeshData.vertices;
        data.indices = navMeshData.indices;

        // JSON���� ��ȯ
        string json = JsonUtility.ToJson(data, true);  // true�� �߰��ؼ� �� ������ ����

        // ������ ���� ���
        string filePath = Application.dataPath + "/NavMeshData.json";

        // �̹� ������ �����ϴ��� Ȯ��
        if (File.Exists(filePath))
        {
            Debug.Log("������ �̹� �����մϴ�. �׺�޽� ������ ������ �ǳʶݴϴ�.");
            yield break; // ������ �����ϸ� ������ ��ŵ�ϰ� ����
        }

        try
        {
            // JSON ���Ϸ� ����
            File.WriteAllText(filePath, json);
            Debug.Log("�׺�޽� ������ JSON ���� �Ϸ�!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("���� ���� ����: " + e.Message);
        }
    }
}
