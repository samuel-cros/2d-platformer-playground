using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    //---------------------------------------------------------------------------------
    // GENERAL
    public GameObject player;


    //oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
    // FIXED UPDATE
    private void FixedUpdate()
    {
        // Basic camera follow, front-oriented character-wise
        transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x + player.transform.localScale.x * 2, player.transform.position.y, -1), Time.deltaTime);
    }
}
