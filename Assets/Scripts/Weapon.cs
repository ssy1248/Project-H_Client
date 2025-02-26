using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float attackRate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public Transform ArrowPos;
    public GameObject ArrowPrefab; // �⺻ ȭ�� ������
    public Transform ArrowFormPos;

    public GameObject arrowPoolObject; // �� ������Ʈ�� ��ü ȭ�� Ǯ�� ����
    private List<GameObject> arrowPool; // ������Ʈ Ǯ
    public int poolSize = 10; // Ǯ ũ��


    public bool isCounterAttack = false; // ī���� ���� ����

    private void Start()
    {
        // Ǯ �ʱ�ȭ (���Ÿ� ������ ���� ����)
        if (type == Type.Range && arrowPoolObject != null)
        {
            arrowPool = new List<GameObject>();

            // �� ������Ʈ�� �ڽ����� �ִ� ��� �������� ������
            foreach (Transform child in arrowPoolObject.transform)
            {
                GameObject arrow = child.gameObject;
                arrow.SetActive(false); // ó������ ��Ȱ��ȭ
                arrowPool.Add(arrow);
            }
        }
    }

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range)
        {
            StopCoroutine("Shot");
            StartCoroutine("Shot");
        }
    }

    public void ActivateCounterAttack()
    {
        // ī���� ���� Ȱ��ȭ
        isCounterAttack = true;
        damage = 50; // ī���� ���� �� ������ ���� (���÷� 50���� ����)
    }

    public void DeactivateCounterAttack()
    {
        // ī���� ���� ��Ȱ��ȭ
        isCounterAttack = false;
        damage = 20; // �⺻ �������� �ǵ����� (���÷� 20���� ����)
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f); // 1������ ���
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.3f); // 1������ ���
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f); // 1������ ���
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        yield return null;

        // ���Ÿ� ������ ���� ����
        if (type == Type.Range)
        {
            // ������Ʈ Ǯ���� ȭ���� ������
            GameObject instantArrow = GetArrowFromPool();
            instantArrow.transform.position = ArrowPos.position;
            instantArrow.transform.rotation = ArrowPos.rotation;

            Rigidbody arrowRigid = instantArrow.GetComponent<Rigidbody>();
            arrowRigid.velocity = ArrowPos.forward * 40;

            // ȭ���� ���� �ε����ų� ���� �ð��� ������ Ǯ�� ��ȯ
            StartCoroutine(ReturnArrowToPool(instantArrow));
        }

        yield return null;
    }

    // ��Ȱ��ȭ�� ȭ���� Ǯ���� ������ �Լ�
    private GameObject GetArrowFromPool()
    {
        foreach (var arrow in arrowPool)
        {
            if (!arrow.activeInHierarchy)
            {
                arrow.SetActive(true); // Ȱ��ȭ
                return arrow;
            }
        }

        // ���� ��Ȱ��ȭ�� ȭ���� ���ٸ� ���� ����
        GameObject newArrow = Instantiate(ArrowPrefab);
        arrowPool.Add(newArrow);
        return newArrow;
    }

    // ����� ���� ȭ���� Ǯ�� ��ȯ�ϴ� �Լ�
    IEnumerator ReturnArrowToPool(GameObject arrow)
    {
        yield return new WaitForSeconds(3f); // 3�� �Ŀ� Ǯ�� ��ȯ (ȭ���� �ٴڿ� �������ٰ� ����)

        arrow.SetActive(false); // ��Ȱ��ȭ
    }
}
