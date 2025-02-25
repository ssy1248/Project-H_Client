using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool DDown;
    bool FDown;

    bool isDodge;
    bool isFireReady;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator anim;
    Rigidbody rigid;

    Weapon equipWeapon;
    float fireDelay;
    private void Awake()
    {
        anim = GetComponent<Animator>(); // �ڽ��� �ƴ� �θ� ������Ʈ���� ��������
        rigid = GetComponent<Rigidbody>();
    }
    public void EquipWeapon(Weapon newWeapon)
    {
        equipWeapon = newWeapon;
    }

    void Start()
    {
        Weapon weapon = GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            EquipWeapon(weapon);
        }
    }

    void Update()
    {
        GETInput();
        move();
        Turn();
        Dodge();
        Attack();
    }

    // wasd �� �̵��� �ƴ� �ξ�ó�� ���콺 �̵�
    void GETInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        DDown = Input.GetButtonDown("Dodge");
        FDown = Input.GetButtonDown("Fire1");
        Debug.Log($"hAxis =" + hAxis);
        Debug.Log($"vAxis =" + vAxis);
    }

    void move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
        {
            moveVec = dodgeVec;
        }

        if (!isFireReady)
        {
            moveVec = Vector3.zero;
        }

        transform.position += moveVec * speed * Time.deltaTime;
        Debug.Log($"moveVec"+ moveVec);
        Debug.Log($"speed" + speed);

        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    // ȸ�� ������Ʈ
    void Dodge()
    {
        Debug.Log($"DDown: {DDown}, isDodge: {isDodge}, moveVec: {moveVec}");

        if (DDown && moveVec != Vector3.zero && !isDodge)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f);
        }
    }
    void DodgeOut()
    {
        speed = 10f;
        isDodge = false;
    }

    // ���⼭ �������� ���ø�����Ʈ �ڵ鷯�� ����ؼ� ������ �ҵ�
    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.attackRate < fireDelay;

        if (FDown && isFireReady && !isDodge)
        {
            equipWeapon.Use();

            // �������� ���� �ִϸ��̼� ����
            int attackIndex = Random.Range(0, 2); // 0, 1 �� �ϳ� ����
            anim.SetInteger("attackIndex", attackIndex); // �ִϸ����� ���� ����
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); // ���� Ʈ���� ����

            transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            fireDelay = 0;
        }
    }



}
