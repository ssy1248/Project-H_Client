using UnityEngine;

public class FanShapeMesh : MonoBehaviour
{
    public float radius = 5f;  // 부채꼴의 반지름
    public float angle = 60f;  // 부채꼴의 각도
    public int segments = 20;  // 부채꼴을 구성하는 세그먼트 수

    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // 부채꼴의 중심 점과 세그먼트 끝 점들을 포함한 벡터 배열
        Vector3[] vertices = new Vector3[segments + 1];  // 1은 중심점
        int[] triangles = new int[segments * 3];  // 삼각형을 이용해 부채꼴을 구성

        // 중심점 설정
        vertices[0] = Vector3.zero;

        // 부채꼴의 끝 점들 추가 (둥근 형태로 배치)
        float angleStep = angle / segments;
        for (int i = 0; i < segments; i++)
        {
            float angleRad = Mathf.Deg2Rad * (i * angleStep);
            // 끝 점들은 원의 경계에 놓이게 됩니다.
            vertices[i + 1] = new Vector3(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius, 0f);
        }

        // 삼각형 인덱스를 설정하여 부채꼴 형태로 만들기
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;  // 중심점
            triangles[i * 3 + 1] = i + 1;  // 첫 번째 점
            triangles[i * 3 + 2] = (i + 1) % segments + 1;  // 다음 점 (원형 연결)
        }

        // 메쉬에 벡터와 삼각형 정보 설정
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // 노멀과 UV 계산
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
