using UnityEngine;

public class FanShapeMesh : MonoBehaviour
{
    public float radius = 5f;  // ��ä���� ������
    public float angle = 60f;  // ��ä���� ����
    public int segments = 20;  // ��ä���� �����ϴ� ���׸�Ʈ ��

    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // ��ä���� �߽� ���� ���׸�Ʈ �� ������ ������ ���� �迭
        Vector3[] vertices = new Vector3[segments + 1];  // 1�� �߽���
        int[] triangles = new int[segments * 3];  // �ﰢ���� �̿��� ��ä���� ����

        // �߽��� ����
        vertices[0] = Vector3.zero;

        // ��ä���� �� ���� �߰� (�ձ� ���·� ��ġ)
        float angleStep = angle / segments;
        for (int i = 0; i < segments; i++)
        {
            float angleRad = Mathf.Deg2Rad * (i * angleStep);
            // �� ������ ���� ��迡 ���̰� �˴ϴ�.
            vertices[i + 1] = new Vector3(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius, 0f);
        }

        // �ﰢ�� �ε����� �����Ͽ� ��ä�� ���·� �����
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;  // �߽���
            triangles[i * 3 + 1] = i + 1;  // ù ��° ��
            triangles[i * 3 + 2] = (i + 1) % segments + 1;  // ���� �� (���� ����)
        }

        // �޽��� ���Ϳ� �ﰢ�� ���� ����
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // ��ְ� UV ���
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
