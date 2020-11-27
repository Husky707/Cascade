using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;

public class ServerController : NetworkManager
{
    public Dictionary<uint, Room> Rooms => _rooms;
    private Dictionary<uint, Room> _rooms = new Dictionary<uint, Room>();
    
    public Room ServerHub => _serverHub;
    private Room _serverHub = null;

    private HubRules HubRules;
    private RoomSettings HubSettings;
    private HubController HubControll;

    private ServerLobbyManager LobbyManager;

    bool isServer = false;
    private void Update()
    {
        if (!isServer) return;

        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Server testing messesage");
            ServerHub.SendTest();
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        SetupHub();
        isServer = true;

    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (conn == null)
            Debug.Log("The added player's connection is null");

        GameObject newPlayer = SpawnPlayer(conn);

        NetworkServer.AddPlayerForConnection(conn, newPlayer);
        AssignNewClient(conn);

        //Tests
        LobbyManager.OnPlayerRequestEnterLobby(conn, eRoomType.Clasic1v1);
    }

    [Server]
    private GameObject SpawnPlayer(NetworkConnection conn)
    {
        GameObject newPlayer = Instantiate(playerPrefab);
        NetworkServer.Spawn(newPlayer, conn);

        Debug.Log("Spawned a new player") ;
        return newPlayer;
    }

    [Server]
    private void AssignNewClient(NetworkConnection conn)
    {
        if (!ServerHub.AddObserver(conn))
            Debug.Log("The new client was not allowed into the server hub ");
    }

    #region Helpers


    [Server]
    private void SetupHub()
    {
        HubSettings = ScriptableObject.CreateInstance("RoomSettings") as RoomSettings;
        HubSettings.Init((uint)maxConnections, 0, true);

        _serverHub = new Room("Hub", 0, eRoomType.Hub, HubSettings);

        LobbyManager = gameObject.AddComponent<ServerLobbyManager>();

        _rooms.Add(0, ServerHub);
        Debug.Log("Hub has been setup");
    }

    #endregion
}
