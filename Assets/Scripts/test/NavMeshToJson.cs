using UnityEngine;
using UnityEngine.AI;
using System.IO;
using System.Collections;
using Unity.AI.Navigation;

public class NavMeshToJson : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;  // 네비메시 서페이스 컴포넌트

    // JSON 직렬화할 때 사용할 클래스 정의
    [System.Serializable]
    public class NavMeshData
    {
        public Vector3[] vertices;
        public int[] indices;
    }

    void Start()
    {
        // 네비메시가 없으면 빌드하기
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh(); // 네비메시 빌드
        }

        // 네비메시 데이터가 준비될 때까지 대기한 후 데이터를 추출
        StartCoroutine(WaitForNavMeshAndExtract());
    }

    // 네비메시 빌드 후 데이터를 추출하는 코루틴
    private IEnumerator WaitForNavMeshAndExtract()
    {
        // 네비메시 빌드가 완료될 때까지 잠시 기다림
        yield return new WaitForSeconds(2f); // 2초 정도 대기 (빌드 완료를 기다림)

        // 네비메시 데이터 계산
        var navMeshData = NavMesh.CalculateTriangulation();

        // 네비메시 데이터가 비어있지 않으면
        if (navMeshData.vertices.Length == 0 || navMeshData.indices.Length == 0)
        {
            Debug.LogError("네비메시 데이터 추출 실패: 유효한 네비메시가 없습니다.");
            yield break; // 네비메시가 없으면 종료
        }

        // JSON으로 변환할 객체 생성
        NavMeshData data = new NavMeshData();
        data.vertices = navMeshData.vertices;
        data.indices = navMeshData.indices;

        // JSON으로 변환
        string json = JsonUtility.ToJson(data, true);  // true를 추가해서 잘 읽히게 포맷

        // 저장할 파일 경로
        string filePath = Application.dataPath + "/NavMeshData.json";

        // 이미 파일이 존재하는지 확인
        if (File.Exists(filePath))
        {
            Debug.Log("파일이 이미 존재합니다. 네비메시 데이터 저장을 건너뜁니다.");
            yield break; // 파일이 존재하면 저장을 스킵하고 종료
        }

        try
        {
            // JSON 파일로 저장
            File.WriteAllText(filePath, json);
            Debug.Log("네비메시 데이터 JSON 저장 완료!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("파일 저장 실패: " + e.Message);
        }
    }
}
