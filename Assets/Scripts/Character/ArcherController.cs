using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : MonoBehaviour
{
    //private PlayerController playerController;
    private bool isBuffActive = false;
    private float originalAttackRate;

    public List<GameObject> buffEffects; // ����Ʈ ������Ʈ Ǯ
    private int effectIndex = 0; // ���� ����� ����Ʈ �ε���
    private GameObject activeEffect; // ���� Ȱ��ȭ�� ����Ʈ

    public float buffCooldown = 7f; // ���� ��Ÿ�� (7��)
    private bool canUseBuff = true; // ��ų ��� ���� ����

    private void Start()
    {
        //playerController = GetComponent<PlayerController>();
        //if (playerController != null)
        //{
        //    originalAttackRate = playerController.AttackRate;
        //}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isBuffActive && canUseBuff && DungeonManager.Instance.MyPlayer == true)
        {
            StartCoroutine(BoostAttackSpeed());
            StartCoroutine(BuffCooldownRoutine()); // ��Ÿ�� ����
        }

        // Ȱ��ȭ�� ����Ʈ�� ������ ��� �÷��̾ ����ٴϰ� ��
        if (activeEffect != null && activeEffect.activeSelf)
        {
            activeEffect.transform.position = transform.position;
        }
    }

    private IEnumerator BoostAttackSpeed()
    {
        isBuffActive = true;
        canUseBuff = false; // ��ų ���� �Ұ� ����
        //playerController.AttackRate *= 0.15f; // ���� �ӵ� ����

        // ����Ʈ Ȱ��ȭ
        ActivateEffect(buffEffects, ref effectIndex, transform.position);
        SEManager.instance.PlaySE("ArcherSkill1");

        yield return new WaitForSeconds(5f); // 5�� ����

        //playerController.AttackRate = originalAttackRate; // ���� �ӵ��� ����
        isBuffActive = false;

        // ����Ʈ ��Ȱ��ȭ
        if (activeEffect != null)
        {
            activeEffect.SetActive(false);
            activeEffect = null;
        }
    }

    // ���� ��ų ��Ÿ�� ��ƾ
    private IEnumerator BuffCooldownRoutine()
    {
        Debug.Log("Buff skill cooldown started.");
        yield return new WaitForSeconds(buffCooldown); // ��Ÿ�� ���
        canUseBuff = true; // ��ų �ٽ� ��� ����
        Debug.Log("Buff skill is ready again!");
    }

    void ActivateEffect(List<GameObject> effectList, ref int effectIndex, Vector3 position, Vector3? direction = null)
    {
        if (effectList.Count > 0)
        {
            GameObject effect = effectList[effectIndex];

            effect.transform.position = position;
            effect.transform.rotation = direction.HasValue ? Quaternion.LookRotation(direction.Value) : transform.rotation;
            effect.SetActive(true);

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();

            activeEffect = effect; // ���� Ȱ��ȭ�� ����Ʈ ����

            effectIndex = (effectIndex + 1) % effectList.Count; // ���� �ε����� �̵�
        }
    }
}
