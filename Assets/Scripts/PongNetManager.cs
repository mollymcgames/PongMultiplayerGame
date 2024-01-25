using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PongNetManager : MonoBehaviourPunCallbacks
{
    string playerName = "Player 1";
    string gameVersion = "0.9";
    List<RoomInfo> createdRooms = new List<RoomInfo>(); //List of created rooms
    string roomName = "Room 1";
    Vector2 roomListScroll = Vector2.zero;
    bool joiningRoom = false;
    public GameObject player;
    public GameObject ball;
    bool render = true;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true; //Sync the scene across the network
        if (!PhotonNetwork.IsConnected) //If you are not connected to the Photon server
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = gameVersion; //Set the game version for this client
            PhotonNetwork.ConnectUsingSettings(); //Connect to the Photon master server
        } 
    }

    public override void OnDisconnected(DisconnectCause cause) //If you are disconnected from the Photon server
    {
        Debug.Log("Disconnected from server for reason " + cause.ToString() + "ServerAddress:" + PhotonNetwork.ServerAddress); //Log the reason for disconnection
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server"); //Log that you are connected to the Photon master server
        Debug.Log("Connection made to" + PhotonNetwork.CloudRegion + " server"); //Log the region of the server that you are connected to
        PhotonNetwork.JoinLobby(TypedLobby.Default); //Join the Photon lobby
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated"); //Log that the room list has been updated
        createdRooms = roomList; //Update the list of created rooms
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (render == true)
        {
            GUI.Window(0, new Rect(Screen.width / 2 - 450, Screen.height/ 2 - 200, 900, 400), LobbyWindow, "Lobby"); //create a new window
        }
    }


    //Can be done in the UI instead
    void LobbyWindow(int index)
    {
        GUILayout.BeginHorizontal(); //Begin a horizontal group
        GUILayout.Label("Status: " + PhotonNetwork.NetworkClientState); //Show the connection status

        if (joiningRoom || !PhotonNetwork.IsConnected || PhotonNetwork.NetworkClientState != ClientState.JoinedLobby) //If you are joining a room or you are not connected to the Photon server or you are not in the Photon lobby
        {
            GUI.enabled = false; //Disable the GUI
        }

        GUILayout.FlexibleSpace(); //Insert a space

        //room name text field
        roomName = GUILayout.TextField(roomName, GUILayout.Width(250)); //Create a textbox for the room name

        if (GUILayout.Button("Create Room", GUILayout.Width(125))) //Button allows us to create a room
        {
            if (roomName != "") //If the room name is not empty
            {
                joiningRoom = true; //You are joining a room

                RoomOptions roomOptions = new RoomOptions(); //Create a new room
                roomOptions.IsOpen = true; //The room is open
                roomOptions.IsVisible = true; //The room is visible
                roomOptions.MaxPlayers = (byte)2; //The room can have a maximum of 2 players

                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default); //Join the room or create it if it does not exist
            }
        }

        GUILayout.EndHorizontal(); //End the horizontal group
        //scroll through the available rooms
        roomListScroll = GUILayout.BeginScrollView(roomListScroll, true, true); //Create a scroll view for the room list

        if (createdRooms.Count == 0)
        {
            GUILayout.Label("No rooms available"); //If there are no rooms available
        }
        else
        {
            for (int i = 0; i < createdRooms.Count; i++) //For each room in the room list
            {
                GUILayout.BeginHorizontal("box"); 
                GUILayout.Label(createdRooms[i].Name, GUILayout.Width(400)); //Show the room name
                GUILayout.Label(createdRooms[i].PlayerCount + "/" + createdRooms[i].MaxPlayers); //Show the number of players in the room

                GUILayout.FlexibleSpace(); //Insert a space

                if (GUILayout.Button("Join Room")) //Button allows us to join a room
                {
                    joiningRoom = true; //You are joining a room
                    PhotonNetwork.NickName = playerName; //Set the player name

                    //Join the room
                    PhotonNetwork.JoinRoom(createdRooms[i].Name);
                }
                GUILayout.EndHorizontal(); //End the horizontal group
            }
        }

        GUILayout.EndScrollView(); //End the scroll view

        GUILayout.BeginHorizontal(); //Begin a horizontal group
        GUILayout.Label("Player Name: ", GUILayout.Width(85)); //Show the player name label
        playerName = GUILayout.TextField(playerName, GUILayout.Width(250)); //Create a textbox for the player name
        GUILayout.FlexibleSpace(); //Insert a space

        GUI.enabled = (PhotonNetwork.NetworkClientState == ClientState.JoinedLobby || PhotonNetwork.NetworkClientState == ClientState.Disconnected) && !joiningRoom; //Enable the GUI
        if (GUILayout.Button("Refresh", GUILayout.Width(100)))
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinLobby(TypedLobby.Default); //Join the Photon lobby
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings(); //Connect to the Photon master server
            }
        }

        GUILayout.EndHorizontal(); //End the horizontal group

        if (joiningRoom) //If you are joining a room
        {
            GUI.enabled = true; //Enable the GUI
            GUI.Label(new Rect(900 / 2 - 50, 400 / 2 - 10, 100, 20), "Connecting..."); //Show the joining room label
        }
    }

    public override void OnJoinedRoom() //If you have joined a room
    {
        Debug.Log("Joined room"); //Log that you have joined a room
        print(PhotonNetwork.CurrentRoom.Players.Count);
        render = false;
        if (PhotonNetwork.CurrentRoom.Players.Count > 1)
        {
            PhotonNetwork.Instantiate(player.name, new Vector3(-4f, 1.5f, -2f), Quaternion.identity, 0); //Instantiate the player
            PhotonNetwork.Instantiate(ball.name, new Vector3(0f, 1.5f, -2f), Quaternion.identity, 0); //Instantiate the ball
        }
        else
        {
            PhotonNetwork.Instantiate(player.name, new Vector3(4f, 1.5f, -2f), Quaternion.identity, 0); //Instantiate the player
        }
    }
}
