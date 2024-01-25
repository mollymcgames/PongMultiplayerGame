using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class playerPad : MonoBehaviourPunCallbacks
{
    public float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine) //If you are the local player then you can change the movement
        {
            InputMovement();
        }
    }

    void InputMovement()
    {
        //Take the input from the keyboard
        if (Input.GetKey(KeyCode.UpArrow))
        {
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + Vector3.up * speed * Time.deltaTime); //move in the up direction at the specified speed
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position - Vector3.up * speed * Time.deltaTime); //move in the down direction at the specified speed
        }
    }
}
