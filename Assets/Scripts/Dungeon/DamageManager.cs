    using UnityEngine;

    public class DamageManager : MonoBehaviour
    {
        public static DamageManager Instance; // 싱글턴 패턴을 위한 정적 인스턴스

        private void Awake()
        {
            // 싱글턴 초기화: 이미 인스턴스가 존재하면 해당 인스턴스를 사용하고, 없으면 현재 오브젝트를 할당
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // 이미 존재하는 인스턴스를 삭제
            }
            else
            {
                Instance = this; // 인스턴스를 현재 오브젝트에 할당
                DontDestroyOnLoad(gameObject); // 씬이 변경되어도 오브젝트가 파괴되지 않도록 설정
            }
        }

        public void SpawnDamageText(int damage, Vector3 position, bool isPlayerHit)
        {
            GameObject damageTextObj = DamageTextPool.Instance.GetDamageText();
            DamageText damageText = damageTextObj.GetComponent<DamageText>();
            damageText.ShowDamage(damage, position, isPlayerHit);
        }
    }
