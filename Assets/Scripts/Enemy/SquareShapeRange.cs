using UnityEngine;

public class SquareShapeMesh : MonoBehaviour
{
    public float width = 5f;  // �簢���� �ʺ�
    public float height = 3f;  // �簢���� ����

    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // �簢���� �� ������
        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];  // �� ���� �ﰢ������ �簢���� ����

        // �簢���� ������ ����
        vertices[0] = new Vector3(-width / 2, 0f, -height / 2);  // ���� �Ʒ�
        vertices[1] = new Vector3(width / 2, 0f, -height / 2);   // ������ �Ʒ�
        vertices[2] = new Vector3(width / 2, 0f, height / 2);    // ������ ��
        vertices[3] = new Vector3(-width / 2, 0f, height / 2);   // ���� ��

        // �簢���� �� ���� �ﰢ������ ����
        triangles[0] = 0;  // ù ��° �ﰢ��
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 0;  // �� ��° �ﰢ��
        triangles[4] = 3;
        triangles[5] = 2;

        // �޽��� ���Ϳ� �ﰢ�� ���� ����
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // ��ְ� UV ���
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
