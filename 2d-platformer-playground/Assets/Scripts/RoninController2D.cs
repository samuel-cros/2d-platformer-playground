using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoninController2D : MonoBehaviour
{

    //---------------------------------------------------------------------------------
    // GENERAL
    private Rigidbody2D body;
    private Animator anim;
    private Collider2D collid;

    // MOVEMENTS
    [SerializeField] private float moveSpeed;

    // JUMPS
    [SerializeField] private float jumpSpeed;
    private bool grounded;
    [SerializeField] private int additionalJumps = 1;
    private int jumpsLeft;

    // BLINK
    private Vector3 mousePosition;
    private Vector3 mouseDirection;
    [SerializeField] private float blinkDistance;
    private float blinkTimer = 0f;
    [SerializeField] private float blinkCooldown = 2f;

    //oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    // AWAKE
    private void Awake()
    {
        // Grab references from game object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collid = GetComponent<Collider2D>();

    }

    //oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    // FIXED UPDATE
    private void FixedUpdate()
    {
        // Get mouse position and direction to raycast towards the mouse
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseDirection = mousePosition - transform.position;
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
        // BLINK
        if (Input.GetKeyDown(KeyCode.Q) && (blinkTimer < Time.time))
        {
            Blink();
            Debug.Log("Blink");
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

    // Jump
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

    // Blink
    private void Blink()
    {
        body.velocity = Vector2.zero;
        jumpsLeft -= 1;

        // Check for collisions with ground
        float mouseDistance = Mathf.Min(blinkDistance, Mathf.Sqrt(Mathf.Pow(mousePosition.x - transform.position.x, 2) + Mathf.Pow(mousePosition.y - transform.position.y, 2)));

        RaycastHit2D hit = Physics2D.Raycast(transform.position, mouseDirection, mouseDistance); // setting layerMask doesn't work while it should 
        
        // If the ray hits the ground, the hit point becomes the new blink position
        if (hit.collider != null && hit.transform.tag == "Ground")
        {
            blinkTimer = Time.time + blinkCooldown;
            transform.position = new Vector2(hit.point.x, hit.point.y);
        }
        // If the ray hits an enemi, the blink position is set right behind the enemi position
        // For flavor, we also reduce the blink cooldown to 0.5 sec and reset the additional jumps
        else if (hit.collider != null && hit.transform.tag == "Enemy")
        {
            //Destroy(hit.transform.gameObject);
            blinkTimer = Time.time + 0.5f;
            jumpsLeft = additionalJumps;
            hit.collider.SendMessage("OnTriggerEnter2D", collid);
            transform.position = new Vector2(hit.collider.transform.position.x + mouseDirection.x / mouseDirection.magnitude,
                                             hit.collider.transform.position.y + mouseDirection.y / mouseDirection.magnitude);
        }
        // Else the blink is set to blinkDistance
        else
        {
            blinkTimer = Time.time + blinkCooldown;
            transform.position = new Vector2(mousePosition.x, mousePosition.y);
        }

        // Animation management
        anim.SetTrigger("blink");
    }

    //oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    // COLLISIONS
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // The character is grounded if his feet touch the ground
        // We also reset his jump count
        if (collision.gameObject.tag == "Ground" && Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 4.4f))
        {
            grounded = true;
            jumpsLeft = additionalJumps;
        }

    }

}
