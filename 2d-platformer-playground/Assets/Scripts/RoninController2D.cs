using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoninController2D : MonoBehaviour
{

    //---------------------------------------------------------------------------------
    // GENERAL
    private Rigidbody2D body;
    private Animator anim;
    //private Collider2D collid;

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
    [SerializeField] private float reducedBlinkCooldown = 0.5f;

    //oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    // AWAKE
    private void Awake()
    {
        // Grab references from game object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //collid = GetComponent<Collider2D>();

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
        Debug.DrawLine(transform.position, (transform.position + Vector3.down * transform.localScale.y / 4.4f), Color.green); // Grounded debug, couldn't yet find a formula to replace "4.4f"
        Debug.DrawRay(transform.position, mouseDirection, Color.green); // Blink debug

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

        // Compute mouse distance
        float mouseDistance = Mathf.Sqrt(Mathf.Pow(mousePosition.x - transform.position.x, 2) + Mathf.Pow(mousePosition.y - transform.position.y, 2));

        // Raycast to a set distance
        float actualBlinkDistance = Mathf.Min(blinkDistance, mouseDistance);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, mouseDirection, actualBlinkDistance); // setting layerMask doesn't work while it should 
        
        // If the ray hits the ground, the hit point becomes the new blink position
        if (hit.collider != null && hit.transform.tag == "Ground")
        {
            blinkTimer = Time.time + blinkCooldown;
            transform.position = new Vector2(hit.point.x, hit.point.y);
        }
        // If the ray hits an enemi, the blink position is set to be right behind the enemy position
        // For flavor, we also reduce the blink cooldown and reset additional jumps
        else if (hit.collider != null && hit.transform.tag == "Enemy")
        {
            // Manage player's behaviour
            blinkTimer = Time.time + reducedBlinkCooldown;
            jumpsLeft = additionalJumps;
            transform.position = new Vector2(hit.collider.transform.position.x + mouseDirection.x / Mathf.Max(1, mouseDirection.magnitude),
                                             hit.collider.transform.position.y + mouseDirection.y / Mathf.Max(1, mouseDirection.magnitude));

            // Manage enemy's behaviour
            //Destroy(hit.transform.gameObject
            hit.collider.SendMessage("GetBlinkedOn");
            
        }
        // Else the blink is set to the min between the blink distance and the mouse distance
        else
        {
            blinkTimer = Time.time + blinkCooldown;
            // Blink by replacing player position with: player.position + ray.direction * distance
            transform.position = new Vector2(transform.position.x + mouseDirection.x * actualBlinkDistance / Mathf.Max(1, mouseDirection.magnitude),
                                             transform.position.y + mouseDirection.y * actualBlinkDistance / Mathf.Max(1, mouseDirection.magnitude));
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && !Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 4.4f))
        {
            if (grounded)
            {
                // Ensures that if we run out of a platform, grounded becomes false
                grounded = false;
            }
        }
    }

}
