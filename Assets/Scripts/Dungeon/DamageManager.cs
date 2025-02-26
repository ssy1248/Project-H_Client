using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance; // 싱글턴 패턴

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

    // 각 유닛의 머리 위에 DamageText 표시
    public void SpawnDamageText(int damage, Transform headTransform, bool isPlayerHit, float textSize = 36f)
    {
        GameObject damageTextObj = DamageTextPool.Instance.GetDamageText();
        DamageText damageText = damageTextObj.GetComponent<DamageText>();
        // 머리 위치를 가져와서 표시
        damageText.ShowDamage(damage, headTransform.position, isPlayerHit, textSize);
    }
}
