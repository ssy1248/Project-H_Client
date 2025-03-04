using UnityEngine;

public class SquareShapeMesh : MonoBehaviour
{
    public float width = 5f;  // 사각형의 너비
    public float height = 3f;  // 사각형의 높이

    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // 사각형의 네 꼭지점
        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];  // 두 개의 삼각형으로 사각형을 만듦

        // 사각형의 꼭지점 설정
        vertices[0] = new Vector3(-width / 2, 0f, -height / 2);  // 왼쪽 아래
        vertices[1] = new Vector3(width / 2, 0f, -height / 2);   // 오른쪽 아래
        vertices[2] = new Vector3(width / 2, 0f, height / 2);    // 오른쪽 위
        vertices[3] = new Vector3(-width / 2, 0f, height / 2);   // 왼쪽 위

        // 사각형을 두 개의 삼각형으로 나눔
        triangles[0] = 0;  // 첫 번째 삼각형
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 0;  // 두 번째 삼각형
        triangles[4] = 3;
        triangles[5] = 2;

        // 메쉬에 벡터와 삼각형 정보 설정
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // 노멀과 UV 계산
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
