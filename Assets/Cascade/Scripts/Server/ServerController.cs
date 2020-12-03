using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(GameCreator))]
public class ServerController : NetworkManager
{
    public static ServerController Server 
    { 
        get 
        {
#if UNITY_SERVER || UNITY_EDITOR
            return _server;
#endif
            return null;
        } 
    }
    private static ServerController _server = null;

    GameCreator GameMaker = null;

    public Dictionary<uint, Room> Rooms => _rooms;
    private Dictionary<uint, Room> _rooms = new Dictionary<uint, Room>();
    
    public HubRoom ServerHub => _serverHub;
    private HubRoom _serverHub = null;

    private RoomSettings HubSettings;

    bool isServer = false;

    public override void Awake()
    {
        base.Awake();
        GameMaker = GetComponent<GameCreator>();
        if (GameMaker == null)
            Debug.Log("Could not find the game maker. Is it a component of ServerController?");

        //Add hub id
        gameids.Add(0);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    #region Server Methods
    public override void OnStartServer()
    {
        base.OnStartServer();

        if (_server == null)
            _server = this;
        else Debug.Log("Failed to make singleton single");

        SetupHub();
        isServer = true;

        Room.RoomClosed += OnRoomClosed;
        Room.RoomOpened += OnRoomOpened;

    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        Room.RoomClosed -= OnRoomClosed;
        Room.RoomOpened -= OnRoomOpened;
        if (Application.isEditor)
            UnityEditor.EditorApplication.isPlaying = false;
        else
            Application.Quit();
    }

    private void SetupHub()
    {
        HubSettings = ScriptableObject.CreateInstance("RoomSettings") as RoomSettings;
        HubSettings.Init((uint)maxConnections, 0, true);

        _serverHub = new HubRoom(this, "Hub", 0, eRoomType.Hub, HubSettings);

        _rooms.Add(0, ServerHub);
        Debug.Log("Hub has been setup");

        Debug.Log("Running room test");
        NewGameRoom(eRoomType.Clasic1v1);
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////
    #region New Client Methods

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (conn == null)
            Debug.Log("The added player's connection is null");

        GameObject newPlayer = SpawnPlayer(conn);

        NetworkServer.AddPlayerForConnection(conn, newPlayer);
        AssignNewClient(conn);

        //Tests
        //DoTest(newPlayer, conn);
        
    }

    private void DoTest(GameObject newPlayer, NetworkConnection conn)
    {
        NetworkIdentity identity = newPlayer.GetComponent<NetworkIdentity>();
        int id = identity.connectionToClient.connectionId;
        Debug.Log("Connect id =  " + conn.connectionId.ToString() + " NetId = " + id.ToString());
        newPlayer.GetComponent<PlayerReceiver>().OtherJoinedLobby(new LobbyPlayer(1));
        //_lobbyManager.OnPlayerRequestEnterLobby(conn, eRoomType.Clasic1v1);

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
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////
    #region Game Room Methods

    public GameRoom NewGameRoom(eRoomType type)
    {
        uint id = NewGameID();
        CascadeGame newGame = GameMaker.GetGame(type);
        if (newGame == null)
        {
            Debug.Log("Cannot create a new game. CascadeGame not found for type " + type.ToString());
            return null;
        }
        GameData data = new GameData(newGame.Board.Layout, newGame.GameSettings);
        GameController controller = new GameController(newGame.Rules, data);

        GameRoom newRoom = new GameRoom(newGame.GameModeName + "_" + id.ToString(), id, type, newGame.RoomSettings, controller);
        Debug.Log("Server created a new room: " + newRoom.Name);

        return newRoom;
    }

    private List<uint> gameids = new List<uint>(1);
    private uint NewGameID()
    {
        uint newid;
        do
        {
            newid = (uint)UnityEngine.Random.Range(1, 200000);
        }
        while (gameids.Contains(newid));

        gameids.Add(newid);
        return newid;
    }

    private void OnRoomOpened(Room room)
    {
        if (room == null || Rooms.ContainsKey(room.RoomId))
            return;

        _rooms.Add(room.RoomId, room);
        RoomCount();
    }

    public void RoomCount()
    {
        if (_rooms == null)
            return;

        Debug.Log("Server room count: " + Rooms.Count.ToString());
    }

    private void OnRoomClosed(Room room)
    {
        if (room == null || !Rooms.ContainsKey(room.RoomId))
            return;

        _rooms.Remove(room.RoomId);
        RoomCount();
    }
    #endregion
}
