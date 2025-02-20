using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool DDown;

    bool isDodge;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator anim;
    Rigidbody rigid;

    private void Awake()
    {
        anim = GetComponent<Animator>(); // 자식이 아닌 부모 오브젝트에서 가져오기
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GETInput();
        move();
        Turn();
        Dodge();
    }

    void GETInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        DDown = Input.GetButtonDown("Dodge");
    }

    void move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        
        if(isDodge)
        {
            moveVec = dodgeVec;
        }

        transform.position += moveVec * speed * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

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
}
