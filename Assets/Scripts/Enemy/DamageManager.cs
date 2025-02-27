using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance; // �̱��� ����

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // �� ������ �Ӹ� ���� DamageText ǥ��
    public void SpawnDamageText(int damage, Transform headTransform, bool isPlayerHit, float textSize = 36f)
    {
        GameObject damageTextObj = DamageTextPool.Instance.GetDamageText();
        DamageText damageText = damageTextObj.GetComponent<DamageText>();
        // �Ӹ� ��ġ�� �����ͼ� ǥ��
        damageText.ShowDamage(damage, headTransform.position, isPlayerHit, textSize);
    }
}
