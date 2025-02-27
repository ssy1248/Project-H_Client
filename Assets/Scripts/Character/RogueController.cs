using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueController : MonoBehaviour
{
    private PlayerController playerController;
    public GameObject grenadePoolObject;  // ����ź Ǯ ������Ʈ ����
    private float throwForce = 15f;  // ����ź�� ������ ��
    private float backflipForce = 5f;  // ���ø��� ��

    private int damage = 50;  // ������ �� (����)

    Animator anim;
    Rigidbody rigid;
    private bool isFlipping = false;

    private Queue<GameObject> grenadePool = new Queue<GameObject>();  // ����ź Ǯ

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        InitializeGrenadePool();  // Ǯ ������Ʈ�κ��� ����ź���� �ʱ�ȭ
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isFlipping)
        {
            StartCoroutine(ExecuteBackflipAndThrow());
        }
    }

    private IEnumerator ExecuteBackflipAndThrow()
    {
        isFlipping = true;
        anim.SetTrigger("doBackflip");
        playerController.isMove = false;

        // ���ø� �� �ڷ� ���ư��� �� �߰�
        rigid.AddForce(Vector3.up * backflipForce + -transform.forward * 5f, ForceMode.Impulse);

        yield return new WaitForSeconds(0.2f); // ���ø� ���� �� �ణ�� ������ �� ��ô
        ThrowGrenade();

        yield return new WaitForSeconds(0.8f); // ���ø� �ִϸ��̼� �ð� ���
        isFlipping = false;
    }

    private void ThrowGrenade()
    {
        // ����ź�� Ǯ���� ������
        GameObject grenade = GetGrenadeFromPool();
        grenade.transform.position = transform.position + transform.forward * 2f + Vector3.up * 0.5f;
        grenade.transform.rotation = transform.rotation;

        Rigidbody grenadeRb = grenade.GetComponent<Rigidbody>();

        // ����ź�� ���������� ���ư��� �ϱ� ���� ������ �չ��⿡ ���� ��
        Vector3 throwDirection = (transform.forward + Vector3.up * 0.5f).normalized;  // ���� ���� (�� + ��)
        grenadeRb.velocity = throwDirection * throwForce;

        // ����ź�� ������ �� 1.5�� �Ŀ� �ڵ����� ����
    }

    private void InitializeGrenadePool()
    {
        // ����ź Ǯ ������Ʈ ���� ����ź���� ã�Ƽ� ��Ȱ��ȭ�� �� ť�� �߰�
        foreach (Transform child in grenadePoolObject.transform)
        {
            GameObject grenade = child.gameObject;
            grenade.SetActive(false);  // Ǯ ������Ʈ�� �ִ� ����ź�� ��Ȱ��ȭ
            grenadePool.Enqueue(grenade);  // ��Ȱ��ȭ�� ����ź�� Ǯ�� �߰�
        }
    }

    private GameObject GetGrenadeFromPool()
    {
        // Ǯ���� ��Ȱ��ȭ�� ����ź�� ������
        if (grenadePool.Count > 0)
        {
            GameObject grenade = grenadePool.Dequeue();  // ť���� �ϳ��� ����
            grenade.SetActive(true);  // Ȱ��ȭ
            return grenade;
        }

        // Ǯ�� ����ź�� ������ ��� �޽��� ���
        Debug.LogWarning("Grenade pool is empty!");
        return null;  // ����ź�� ������ null ��ȯ (�� ��� �߰� ������ ���� �� ����)
    }
}
