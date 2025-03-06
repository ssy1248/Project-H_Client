using UnityEngine;

public class Blade : MonoBehaviour
{
    public GameObject warningCircle; // ��� ��
    private void OnCollisionEnter(Collision collision)
    {
        // Į�� ���鿡 ����� ��
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(this.gameObject); // Į ����
            if (warningCircle != null)
            {
                Destroy(warningCircle); // ���� ��� ���� ����
            }
        }
    }
}