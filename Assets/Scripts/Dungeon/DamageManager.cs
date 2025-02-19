    using UnityEngine;

    public class DamageManager : MonoBehaviour
    {
        public static DamageManager Instance; // �̱��� ������ ���� ���� �ν��Ͻ�

        private void Awake()
        {
            // �̱��� �ʱ�ȭ: �̹� �ν��Ͻ��� �����ϸ� �ش� �ν��Ͻ��� ����ϰ�, ������ ���� ������Ʈ�� �Ҵ�
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // �̹� �����ϴ� �ν��Ͻ��� ����
            }
            else
            {
                Instance = this; // �ν��Ͻ��� ���� ������Ʈ�� �Ҵ�
                DontDestroyOnLoad(gameObject); // ���� ����Ǿ ������Ʈ�� �ı����� �ʵ��� ����
            }
        }

        public void SpawnDamageText(int damage, Vector3 position, bool isPlayerHit)
        {
            GameObject damageTextObj = DamageTextPool.Instance.GetDamageText();
            DamageText damageText = damageTextObj.GetComponent<DamageText>();
            damageText.ShowDamage(damage, position, isPlayerHit);
        }
    }
