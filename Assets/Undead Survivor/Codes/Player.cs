using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter=GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands=GetComponentsInChildren<Hand>(true);
    }
    private void OnEnable()
    {
        speed*=Character.Speed;
        anim.runtimeAnimatorController=animCon[GameManager.Instance.playerId];
    }
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isLive)
        {
            return;
        }
        //inputVec.x = Input.GetAxisRaw("Horizontal");
        //inputVec.y = Input.GetAxisRaw("Vertical");


    }
    void FixedUpdate()
    {
        if (!GameManager.Instance.isLive)
        {
            return;
        }
        //1.add force
        //rigid.AddForce(inputVec);
        //2.control velocity
        //rigid.linearVelocity = inputVec;
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        //3.move postition
        rigid.MovePosition(rigid.position+nextVec);

    }
    void LateUpdate()
    {
        if (!GameManager.Instance.isLive)
        {
            return;
        }
        anim.SetFloat("Speed",inputVec.magnitude);
        if (inputVec.x!=0){
            spriter.flipX = inputVec.x < 0;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.Instance.isLive) 
        { 
            return;
        }
        GameManager.Instance.health -= Time.deltaTime * 10;
        if (GameManager.Instance.health < 0)
        {
            for (int index = 2; index < transform.childCount; index++) 
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");
            GameManager.Instance.GameOver();
        }
    }
    void OnMove(InputValue value) 
    {
        inputVec=value.Get<Vector2>();
    }
}
