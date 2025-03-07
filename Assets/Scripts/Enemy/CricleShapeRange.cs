using UnityEngine;

public class CircleShapeMesh : MonoBehaviour
{
    public float radius = 5f;  // 원의 반지름
    public int segments = 36;  // 원을 구성하는 세그먼트 수 (각도에 따른 세분화)

    void Start()
    {
        // 새로운 Mesh 생성
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // 원의 점들을 저장할 배열
        Vector3[] vertices = new Vector3[segments + 1];  // 1은 중심점
        int[] triangles = new int[segments * 3];  // 원을 그리기 위한 삼각형의 인덱스

        // 중심점 추가
        vertices[0] = Vector3.zero;

        // 원의 끝 점들 추가 (원형 모양)
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angleRad = Mathf.Deg2Rad * (i * angleStep);
            vertices[i + 1] = new Vector3(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius, 0f);
        }

        // 삼각형 인덱스를 설정하여 원을 그리기
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;  // 중심점
            triangles[i * 3 + 1] = i + 1;  // 첫 번째 점
            triangles[i * 3 + 2] = (i + 1) % segments + 1;  // 다음 점 (원형 연결)
        }

        // Mesh에 점들과 삼각형 정보 설정
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // 노멀과 UV 계산
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
