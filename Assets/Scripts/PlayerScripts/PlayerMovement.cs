using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

    public bool isGrounded = false;
    public bool isMoving = false;
    public float jumpForce = 18;
    public float speed = 400f;

    public GameObject GroundCheck;
    public Animator anim;

    private float horizontalMove;
    private bool m_FacingRight = true;

    //private Vector3 m_Velocity = Vector3.zero; bored from the same warning
    private Rigidbody2D m_Rigidbody2D;


    private void Start(){
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update () {
        if (hasAuthority)
            horizontalMove = Input.GetAxisRaw("Horizontal") * speed;
    }

    void FixedUpdate(){
        if (hasAuthority)
        {
            if (horizontalMove != 0)
            {
                isMoving = true;
            }else
            {
                isMoving = false;
            }
            Move(horizontalMove * Time.fixedDeltaTime);

            anim.SetFloat("Blend", Mathf.Abs(horizontalMove * 10));

            if (Input.GetButtonDown("Jump"))
            {
                Jump();
                isMoving = true;
            }
        }

    }

    public void Move(float move) {
        Vector2 targetVelocity = new Vector2(move, m_Rigidbody2D.velocity.y);
        m_Rigidbody2D.velocity = targetVelocity;

        if (move > 0 && !m_FacingRight)
        {
            m_FacingRight = !m_FacingRight;
            CmdFlip();
        }
        else if (move < 0 && m_FacingRight)
        {
            m_FacingRight = !m_FacingRight;
            CmdFlip();
        }
    }

    public void Jump() {
        if (isGrounded){
            m_Rigidbody2D.velocity += jumpForce * Vector2.up;
            m_Rigidbody2D.velocity =new Vector2(m_Rigidbody2D.velocity.x, Mathf.Min(m_Rigidbody2D.velocity.y, jumpForce));
            isGrounded = false;
        }  
    }

    [Command]
    void CmdFlip()
    {
        RpcFlip();
    }

    [ClientRpc]
    void RpcFlip()
    {
        Flip();
    }

    void Flip() {
        transform.Rotate(0f, 180f, 0f);
    }


}
