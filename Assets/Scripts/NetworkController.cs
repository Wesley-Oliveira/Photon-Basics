using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [Header("GO")]
    public GameObject loginGO;
    public GameObject partidasGO;

    [Header("PLAYER")]
    public InputField playerNameIput;
    private string playerNameTemp;
    public GameObject myPlayer;

    [Header("ROOM")]
    public InputField roomName;

    //Conectando aos servidores da Photon usando as configurações fornecidas no projeto
    void Start ()
    {
        //PhotonNetwork.ConnectUsingSettings();
        playerNameTemp = "Player" + Random.Range(1000, 10000);
        playerNameIput.text = playerNameTemp;

        roomName.text = "Room" + Random.Range(1000, 10000);

        loginGO.SetActive(true);
        partidasGO.SetActive(false);
	}

    public void Login()
    {
        if(playerNameIput.text != "")
        {
            PhotonNetwork.NickName = playerNameIput.text;
        }
        else
        {
            PhotonNetwork.NickName = playerNameTemp;
        }

        PhotonNetwork.ConnectUsingSettings();

        loginGO.SetActive(false);
    }

    public void BotaoBuscarPartidaRapida()
    {
        PhotonNetwork.JoinLobby();
    }

    public void BotaoCriarSala()
    {
        string roomNameTemp = roomName.text;
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(roomNameTemp, roomOptions, TypedLobby.Default);
    }

    #region PunCallbacks

    //Quando conectado
    public override void OnConnected()
    {
        Debug.Log("OnConnected");
    }

    //Quando conectado ao master
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        Debug.Log("Server: " + PhotonNetwork.CloudRegion + "Ping: " + PhotonNetwork.GetPing());

        partidasGO.SetActive(true);
        //Conectando a um lobby existente
        //PhotonNetwork.JoinLobby();
    }

    //Conectando a uma sala do lobby
    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    //Caso ocorra falha ao conectar a uma sala, criar uma nova sala
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("Room-" + Random.Range(100, 10000));
    }

    //Caso entre em uma sala
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        Debug.Log("RoomName: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Current Player in Room: " + PhotonNetwork.CurrentRoom.PlayerCount);

        loginGO.SetActive(false);
        partidasGO.SetActive(false);

        //Para instanciar um objeto multiplayer
        PhotonNetwork.Instantiate(myPlayer.name, myPlayer.transform.position, myPlayer.transform.rotation, 0);
    }

    //Quando desconectado
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected: " + cause);
    }
    #endregion

}
