using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float attackRate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
    }

    IEnumerator Swing()
    {
        //1�� ����
        yield return new WaitForSeconds(0.1f); //1������ ���
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        //2�� ����
        yield return new WaitForSeconds(0.3f); //1������ ���
        meleeArea.enabled = false;

        //3�� ����
        yield return new WaitForSeconds(0.3f); //1������ ���
        trailEffect.enabled = false;
    }
}
