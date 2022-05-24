using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyeController2D : MonoBehaviour
{
    // General
    private Animator anim;
    private Rigidbody2D body;
    private Collider2D collid;

    // Fall
    [SerializeField] private float fallSpeed = 7f;
    private bool dead = false;

    //oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    // AWAKE
    private void Awake()
    {
        // Grab references from game object
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        collid = GetComponent<Collider2D>();

    }

    //oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    // UPDATE
    private void Update()
    {
        //---------------------------------------------------------------------------------
        // ANIMATOR
        anim.SetBool("dead", dead);
    }

    // Encapsulates what happens when the enemy gets blinked on
    private void GetBlinkedOn()
    {
        if (!dead)
        {
            // Trigger the dying phase, falling down + animation
            body.velocity = Vector2.down * fallSpeed;
            anim.SetTrigger("dying");

            // Untag the enemy so the player can't blink on it again
            tag = "Untagged";
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Ground" && !dead)
        {
            // Trigger the dead phase, stop movement + animation
            body.velocity = Vector2.zero;
            dead = true;

            // Disable collider to avoid disrupting the player's (grounded, blink) raycast(s)
            collid.enabled = false;
            
        }
    }
}
