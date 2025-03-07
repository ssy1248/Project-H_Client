using UnityEngine;

public class CircleShapeMesh : MonoBehaviour
{
    public float radius = 5f;  // ���� ������
    public int segments = 36;  // ���� �����ϴ� ���׸�Ʈ �� (������ ���� ����ȭ)

    void Start()
    {
        // ���ο� Mesh ����
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // ���� ������ ������ �迭
        Vector3[] vertices = new Vector3[segments + 1];  // 1�� �߽���
        int[] triangles = new int[segments * 3];  // ���� �׸��� ���� �ﰢ���� �ε���

        // �߽��� �߰�
        vertices[0] = Vector3.zero;

        // ���� �� ���� �߰� (���� ���)
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angleRad = Mathf.Deg2Rad * (i * angleStep);
            vertices[i + 1] = new Vector3(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius, 0f);
        }

        // �ﰢ�� �ε����� �����Ͽ� ���� �׸���
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;  // �߽���
            triangles[i * 3 + 1] = i + 1;  // ù ��° ��
            triangles[i * 3 + 2] = (i + 1) % segments + 1;  // ���� �� (���� ����)
        }

        // Mesh�� ����� �ﰢ�� ���� ����
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // ��ְ� UV ���
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
