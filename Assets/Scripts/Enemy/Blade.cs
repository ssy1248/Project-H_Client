using UnityEngine;

public class Blade : MonoBehaviour
{
    public GameObject warningCircle; // 경고 원
    private void OnCollisionEnter(Collision collision)
    {
        // 칼이 지면에 닿았을 때
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(this.gameObject); // 칼 삭제
            if (warningCircle != null)
            {
                Destroy(warningCircle); // 원형 경고 범위 삭제
            }
        }
    }
}