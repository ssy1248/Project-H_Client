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
    public Transform ArrowPos;
    public GameObject Arrow;
    public Transform ArrowFormPos;


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

    IEnumerator Shot()
    {
        yield return null;
        //ȭ�� �߻�
        GameObject intantArrow = Instantiate(Arrow, ArrowPos.position, ArrowPos.rotation);
        Rigidbody arrowRigid = intantArrow.GetComponent<Rigidbody>();
        arrowRigid.velocity = ArrowPos.forward * 40;
        yield return null;

        //ȭ�� ����
    }
}
