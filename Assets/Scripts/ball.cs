using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ball : MonoBehaviourPunCallbacks
{

    public float startSpeed = 5f;
    public float maxSpeed = 20f;
    public float speedIncrease = 0.25f;
    private float currentSpeed;
    private Vector2 currentDirection;
    public Vector2 score;
    private float scoreGoalL;
    private float scoreGoalR;
    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = startSpeed;
        currentDirection = Random.insideUnitCircle.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        //Check how many players are in the room
        if (PhotonNetwork.CurrentRoom.Players.Count ==  0)
            return;
        //update the position of the ball
        Vector2 moveDirection = currentDirection * currentSpeed * Time.deltaTime;
        transform.Translate(new Vector3(moveDirection.x, moveDirection.y, 0f));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
        {
            currentDirection.y *= -1; //vertical boundary, reverse y direction
        }
        else if (other.tag == "Player")
        {
            currentDirection.x *= -1; //horizontal boundary, reverse x direction
        }
        else if (other.tag == "GoalL")
        {
            score.x++;
            ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))); //change the color of the ball to a random color
            ChangePositionTo(new Vector3(0f, 1.5f, -2f)); //change the position of the ball
            ChangeDirTo(Random.insideUnitCircle.normalized); //change the direction of the ball to a new random direction
            ChangeScore(score); //change the score of the ball
        }
        else if (other.tag == "GoalR")
        {
            score.y++;
            ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))); //change the color of the ball to a random color
            ChangePositionTo(new Vector3(0f, 1.5f, -2f)); //change the position of the ball
            ChangeDirTo(Random.insideUnitCircle.normalized); //change the direction of the ball to a new random direction
            ChangeScore(score); //change the score of the ball
        }
    }

    [PunRPC]

    void ChangeScore(Vector2 score)
    {
        Lel(score);
        if (photonView.IsMine)
        {
            photonView.RPC("ChangeScore", RpcTarget.OthersBuffered, score);
        }
    }

    [PunRPC]
    void Lel(Vector2 score)
    {
        scoreGoalL = score.x;
        scoreGoalR = score.y;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Score: " + scoreGoalL + " - " + scoreGoalR); //Display the score of the ball
    }

    [PunRPC]

    void ChangePositionTo(Vector3 myposition)
    {
        GetComponent<Transform>().position = myposition;
        if (photonView.IsMine)
        {
            photonView.RPC("ChangePositionTo", RpcTarget.OthersBuffered, myposition);
        }
    }

    [PunRPC]
    void ChangeDirTo(Vector2 mycurrentDirection)
    {
        currentDirection = mycurrentDirection;
        if (photonView.IsMine)
        {
            photonView.RPC("ChangeDirTo", RpcTarget.OthersBuffered, mycurrentDirection);
        }
    }

    [PunRPC]
    void ChangeColorTo(Vector3 color) 
    {
        GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);
        if (photonView.IsMine) //If I am the local player, call the RPC method. This will make sure that all other players will see the new color of the ball.
        {
            photonView.RPC("ChangeColorTo", RpcTarget.OthersBuffered, color);
        }
    }

}
