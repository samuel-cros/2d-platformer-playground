using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoninController2D : MonoBehaviour
{

    //---------------------------------------------------------------------------------
    // GENERAL
    private Rigidbody2D body;
    private Animator anim;

    // MOVEMENTS
    [SerializeField] private float moveSpeed;

    // JUMPS
    [SerializeField] private float jumpSpeed;
    private bool grounded;
    [SerializeField] private int additionalJumps = 1;
    private int jumpsLeft;

    //oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    // AWAKE
    private void Awake()
    {
        // Grab references from game object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    //oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    // UPDATE
    private void Update()
    {
        //---------------------------------------------------------------------------------
        // HORIZONTAL MOVEMEMENT
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, body.velocity.y);

        // Flip player when moving right or left
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);

        //---------------------------------------------------------------------------------
        // JUMP
        if (Input.GetKeyDown(KeyCode.Space) && ((jumpsLeft > 0) || grounded))
        {
            Jump();
            Debug.Log("Jump");
        }



        //---------------------------------------------------------------------------------
        // ANIMATOR
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", grounded);

        //---------------------------------------------------------------------------------
        // DEBUG
        Debug.DrawLine(transform.position, (transform.position + Vector3.down * transform.localScale.y / 4.4f), Color.green);

    }


    //oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    // FUNCTIONS
    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpSpeed);
        anim.SetTrigger("jump");
        if (grounded)
        {
            grounded = false;
        }
        else
        {
            jumpsLeft -= 1;
        }
    }

    //oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    // COLLISIONS
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 4.4f))
        {
            grounded = true;
            jumpsLeft = additionalJumps;
        }

    }

}
