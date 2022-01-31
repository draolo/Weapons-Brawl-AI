using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovementOffline : MonoBehaviour
{
    public bool isGrounded = true;
    public bool isMoving = false;
    public float jumpForce = 18;
    public float speed = 400f;
    public Transform spriteObject;

    public GameObject GroundCheck;
    public Animator anim;

    public float horizontalMove;
    public bool jump;
    public bool m_FacingRight = true;

    //private Vector3 m_Velocity = Vector3.zero; bored from the same warning
    private Rigidbody2D m_Rigidbody2D;

    private void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    public void PlayerMovement()
    {
        if (horizontalMove != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        Move(horizontalMove * Time.fixedDeltaTime);
        anim.SetFloat("Blend", Mathf.Abs(horizontalMove * 10));
        if (jump)
        {
            Jump();
            isMoving = true;
        }
    }

    public void Move(float move)
    {
        Vector2 targetVelocity = new Vector2(move, m_Rigidbody2D.velocity.y);
        m_Rigidbody2D.velocity = targetVelocity;

        FaceTowards(move);
    }

    public void FaceTowards(float move)
    {
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

    public void Jump()
    {
        if (isGrounded)
        {
            m_Rigidbody2D.velocity = new Vector3(m_Rigidbody2D.velocity.x, jumpForce);
        }
        jump = false;
    }

    private void CmdFlip()
    {
        RpcFlip();
    }

    private void RpcFlip()
    {
        Flip();
    }

    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
    }
}