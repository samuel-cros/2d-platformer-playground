using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{

    //---------------------------------------------------------------------------------
    // GENERAL
    private Rigidbody2D body;
    private Animator anim;

    // MOVEMENTS
    [SerializeField] private float moveSpeed;

    // JUMP
    [SerializeField] private float jumpSpeed;
    private bool grounded;



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
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
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
        Debug.DrawLine(transform.position, (transform.position + Vector3.down * transform.localScale.y / (1.92435f * 2)), Color.green);

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
    }




    //oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    // COLLISIONS
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && Physics2D.Raycast(transform.position, Vector2.down, 0.25f * transform.localScale.y))
        {
            grounded = true;
        }

    }

}
