using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    Hashtable gameMode = new Hashtable(); //Esse gameMode vai possuir diversos valores de configuração de sala e configurações do modo de jogo
    public byte gameMaxPlayer = 2; //Número max de players
    string gameModeKey = "gameMode"; // A chave para facilitar quando quisermos apontar para o hashtable gameMode

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

    public void BotaoCriarSala()
    {
        string roomNameTemp = roomName.text;
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(roomNameTemp, roomOptions, TypedLobby.Default);
    }

    public void BotaoBuscarPartidaRapida()
    {
        //Buscar partida aleatória
        string[] typeGameRandom = new string[]
        {
            "PvP",
            "PvE"
        };

        gameMode.Add(gameModeKey, typeGameRandom[Random.Range(0, typeGameRandom.Length)]);
        PhotonNetwork.JoinLobby();
    }

    public void BotaoPartidaPvP()
    {
        gameMode.Add(gameModeKey, "PvP"); //Para adicionar o modo de jogo dentro do hashtable
        PhotonNetwork.JoinLobby();
    }

    public void BotaoPartidaPvE()
    {
        gameMode.Add(gameModeKey, "PvE"); //Para adicionar o modo de jogo dentro do hashtable
        PhotonNetwork.JoinLobby();
    }

    // Listar as salas
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ListasDeSalas(roomList);
    }

    void ListasDeSalas(List<RoomInfo> roomList)
    {
        foreach (var item in roomList)
        {
            Debug.Log("Room Name: " + item.Name);
            Debug.Log("Room IsOpen: " + item.IsOpen);
            Debug.Log("Room IsVisible: " + item.IsVisible);
            Debug.Log("Room MaxPlayers: " + item.MaxPlayers);
            Debug.Log("Room PlayerCount: " + item.PlayerCount);

            object temp;
            item.CustomProperties.TryGetValue(gameModeKey, out temp);
            Debug.Log("Room CustomProperties: " + temp.ToString());
        }
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

    //Conectando a uma sala do lobby passando como parâmetro a váriavel gameMode - onde possui as informações do modo de jogo
    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom(gameMode, gameMaxPlayer);
    }

    //Caso ocorra falha ao conectar a uma sala, criar uma nova sala com características específicas definidas no hashtable gameMode
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions options = new RoomOptions();
        options.IsOpen = true;
        options.IsVisible = true;
        options.MaxPlayers = gameMaxPlayer;
        options.CustomRoomProperties = gameMode;
        options.CustomRoomPropertiesForLobby = new string[] { gameModeKey };
        PhotonNetwork.CreateRoom("Room-" + Random.Range(100, 10000), options);
    }

    //Caso entre em uma sala
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        Debug.Log("RoomName: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Current Player in Room: " + PhotonNetwork.CurrentRoom.PlayerCount);

        loginGO.SetActive(false);
        partidasGO.SetActive(false);

        object typeGameValue;
        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(gameModeKey, out typeGameValue))
        {
            Debug.Log("GameMode: " + typeGameValue.ToString());
        }

        foreach (var item in PhotonNetwork.PlayerList)
        {
            //Debug.Log("Name: " + item.NickName);
            //Debug.Log("IsMaster: " + item.IsMasterClient);

            Hashtable playerCustom = new Hashtable();
            playerCustom.Add("Lives", 3);
            playerCustom.Add("Score", 0);

            item.SetCustomProperties(playerCustom, null, null);
            item.SetScore(0); //Para usar o sistema de score da própria photon
        }

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
