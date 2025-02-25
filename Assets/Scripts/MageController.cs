using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.AI;
using Google.Protobuf.Protocol;
using UnityEngine.XR;

public class MageController : PlayerController
{
    public GameObject meteorEffectPrefab; // ���׿� ����Ʈ ������
    public float meteorDelay = 1f; // ���׿� ���� �ð�
    public float meteorSpeed = 15f; // ���׿��� �������� �ӵ�

    private bool isMeteorCasting = false; // ���׿� ���� ����

    Animator anim;
    MeshRenderer[] meshs;
    Rigidbody rigid;
    private Camera camera;
    private NavMeshAgent nav;

    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); // �ڽ��� �ƴ� �θ� ������Ʈ���� ��������
        meshs = GetComponentsInChildren<MeshRenderer>();
        camera = Camera.main;
        nav = GetComponent<NavMeshAgent>();
        nav.updateRotation = false;
    }

    void Update()
    {
        base.Update(); // �θ� Ŭ������ Update ȣ�� (�⺻���� �÷��̾� ������ ó��)

        if (Input.GetKeyDown(KeyCode.Q) && !isMeteorCasting)
        {
            CastMeteor();
        }
    }

    // ���׿� ���� �Լ�
    void CastMeteor()
    {
        isMeteorCasting = true;
        anim.SetTrigger("doMeteorCast"); // ���׿� ���� �ִϸ��̼� Ʈ����

        // ���׿� ����Ʈ ����
        StartCoroutine(MeteorFallRoutine());
    }

    // ���׿��� �ϴÿ��� �������� �ڷ�ƾ
    IEnumerator MeteorFallRoutine()
    {
        // ���׿��� ���� ���̿��� �������� �ϱ� ���� �ʱ� ��ġ�� ����
        Vector3 spawnPosition = new Vector3(transform.position.x + Random.Range(-5f, 5f), 10f, transform.position.z + Random.Range(-5f, 5f));
        GameObject meteor = Instantiate(meteorEffectPrefab, spawnPosition, Quaternion.identity);

        // ���׿� ����Ʈ ����
        ParticleSystem ps = meteor.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        // ���׿��� ��ǥ ��ġ�� ���������� ���
        Vector3 targetPosition = transform.position;
        float fallTime = 0;

        while (fallTime < meteorDelay)
        {
            meteor.transform.position = Vector3.Lerp(spawnPosition, targetPosition, fallTime / meteorDelay);
            fallTime += Time.deltaTime;
            yield return null;
        }

        meteor.transform.position = targetPosition; // ���׿� ���� ��ǥ ��ġ�� ��ġ

        // ���׿��� �������� �������� �ִ� ó��
        Collider[] hitEnemies = Physics.OverlapSphere(targetPosition, 3f, LayerMask.GetMask("Enemy"));
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                // Enemy ��ũ��Ʈ���� ������ ó�� ��Ŀ� �°� ȣ��
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null && !enemyScript.isDead)
                {
                    int damage = 100; // ���÷� 100 ������, �ʿ�� �ٸ� ������ ���� ����
                    enemyScript.curHealth -= damage;
                    DamageManager.Instance.SpawnDamageText(damage, enemy.transform.position, isPlayerHit: true);

                    // ������ ���� �� ���� ó��
                    if (enemyScript.curHealth <= 0)
                    {
                        enemyScript.isDead = true;
                        enemyScript.anim.SetTrigger("doDie");
                        Destroy(enemy.gameObject, 4f); // ���� ���� �� 4�� �ڿ� ����
                    }
                }
            }
        }

        // ���׿� ����Ʈ ��Ȱ��ȭ
        Destroy(meteor);
        isMeteorCasting = false;
    }
}
