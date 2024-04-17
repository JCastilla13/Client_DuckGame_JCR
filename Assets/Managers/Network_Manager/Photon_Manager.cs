using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Photon_Manager : MonoBehaviourPunCallbacks
{
    public static Photon_Manager _PHOTON_MANAGER;
    private void Awake()
    {
        if (_PHOTON_MANAGER != null && _PHOTON_MANAGER != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _PHOTON_MANAGER = this;
            DontDestroyOnLoad(this.gameObject);

            //Conecto con el servidor
            PhotonConnect();
        }
    }
    private void PhotonConnect()
    {
        //Configurar el inicio de partida
        PhotonNetwork.AutomaticallySyncScene = true;

        //Me conecto al servidor
        PhotonNetwork.ConnectUsingSettings();
    }

    //Conexion al servidor
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conexion al servidor realizada correctamente");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("He implosionado because: " + cause);
        Application.Quit();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("He accedido al lobby");
    }

    public void CreateRoom(string nameRoom)
    {
        PhotonNetwork.CreateRoom(nameRoom, new RoomOptions {MaxPlayers = 2});
    }

    public void JoinRoom(string nameRoom)
    {
        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinRoom(nameRoom);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Me he unido a una sala to guapa llamada: " + PhotonNetwork.CurrentRoom.Name + " conn: " + PhotonNetwork.CurrentRoom.PlayerCount + " jugadores");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Mecachis, no se me ha podido conectar a la sala " + returnCode + "TU" + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("InGame");
        }
    }
}
