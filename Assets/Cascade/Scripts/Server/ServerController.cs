using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;

public class ServerController : NetworkManager
{
    public Dictionary<uint, GameRoom> Rooms => _rooms;
    private Dictionary<uint, GameRoom> _rooms = new Dictionary<uint, GameRoom>();
    
    public GameRoom ServerHub => _ServerHub;
    [SerializeField] private GameRoom _ServerHub = null;

    private HubRules HubRules;
    private RoomSettings HubSettings;
    private HubController HubControll;

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
        GameObject newPlayer = SpawnPlayer(conn);
        PlayerMessenger messeneger = newPlayer.GetComponent<PlayerMessenger>();
        //messeneger.ServerLink = HubControll;
        messeneger.SetUpPlayer(0, -1);

        NetworkServer.AddPlayerForConnection(conn, newPlayer);
        AssignNewClient(conn);
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
        ServerHub.AddObserver(conn);
    }

    #region Helpers
    private uint count = 0;
    [Server]
    private uint GenerateUniqueID()
    {
        int random = (int)UnityEngine.Random.Range(1000000, 4000000);
        int topper = random - random % 1000000;
        uint retVal = (uint)topper + count;
        count++;
        return retVal;
    }

    [Server]
    private void SetupHub()
    {
        int macCon = maxConnections;
        HubSettings = ScriptableObject.CreateInstance("RoomSettings") as RoomSettings;
        HubSettings.Init((uint)maxConnections, 0, true);
        HubRules = new HubRules();
        //HubControll = new HubController();
        //ServerHub = new GameRoom(HubRules, HubSettings, HubControll);

        _rooms.Add(0, ServerHub);
        Debug.Log("Hub has been setup");
    }

    #endregion
}
