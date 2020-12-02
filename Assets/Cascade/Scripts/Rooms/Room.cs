using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System;

public class Room : INetworkCommunicator, ICommToClient
{
    public static event Action<Room> RoomClosed = delegate { };
    public static event Action<Room> RoomOpened = delegate { };

    //int: ID created by Mirror for each NetConn
    protected Dictionary<int, NetworkConnection> Observers = new Dictionary<int, NetworkConnection>();
    public int NumObservers => Observers.Count;

    public GameObject RoomObject => _roomObject;
    protected GameObject _roomObject = null;
    public string Name => _name;
    protected string _name = "Default Room";
    public uint RoomId => _roomId;
    protected uint _roomId = 0;
    public eRoomType Type => _type;
    protected eRoomType _type = eRoomType.Void;

    protected RoomSettings ObserverSettings = null;

    public bool RoomIsActive => IsRoomActive();
    protected bool _roomIsActive = false;
    public bool IsEmpty => Observers.Count <= 0;

    ///////////////////////////////////////////////////////////////////////////////////
    #region Init
    public Room(string name, uint id, eRoomType type, RoomSettings settings)
    {
        _name = name;
        _roomId = id;
        _type = type;
        ObserverSettings = settings;

        _roomObject = new GameObject(_name + "_object");
        _roomObject.AddComponent(typeof(NetworkIdentity));

        _roomIsActive = true;
        RoomOpened.Invoke(this);
    }

    protected virtual void Close()
    {
        RoomClosed.Invoke(this);
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////
    #region Observer Data
    [Server]
    public bool IsRoomActive()
    {
        if (_roomIsActive == false)
            return false;

        return true;
    }

    [Server]
    public bool HasObserver(int target)
    {
        if (Observers.ContainsKey(target))
            return true;

        return false;
    }

    [Server]
    public bool HasObserver(NetworkConnection conn)
    {
        foreach (int id in Observers.Keys)
            if (Observers[id] == conn)
                return true;

        return false;
    }

    [Server]
    protected bool IsReceivingObservers()
    {
        if (Observers.Count >= ObserverSettings.MaxObservers)
            return false;
        if (!RoomIsActive)
            return false;

        return true;
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////
    #region Communication

    [Server]
    public PlayerReceiver GetClient(int targetid)
    {
        if (Observers == null || !Observers.ContainsKey(targetid))
            return null;

        return Observers[targetid].identity.GetComponent<PlayerReceiver>();
    }

    [Server]
    protected virtual bool HasReceiver(NetworkIdentity identity)
    {
        if (identity == null)
            return false;

        int id = identity.connectionToClient.connectionId;
        if (!HasObserver(id))
        {
            Debug.Log("Unknown player requested play. ID: " + id.ToString());
            HandleUnknownMessager(identity);
            return false;
        }

        return true;
    }

    [Server]
    protected virtual void HandleUnknownMessager(NetworkIdentity identity)
    {
        Debug.Log("TODO: Notify Hub of missing player message. Find player. DO stuff");
    }

    /////////////////////////////////////////////////////////////////////////////
    #region Gameplay Requests

    public virtual void RequestPlacement(NetworkIdentity identity, uint xx, uint yy)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestPlacement is not a valid command in this room");
    }

    public virtual void RequestAbilityOrientationSelection(NetworkIdentity identity, ePlacerOrientation ePlacerOrientation)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestAbilityOreintation is not a valid command in this room");
    }

    public virtual void RequestAbilityTypeSelection(NetworkIdentity identity, eDicePlacers type)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestAbilityTypeSelection is not a valid command in this room");
    }

    public virtual void RequestAbilityValueSelection(NetworkIdentity identity, uint value)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestAbilityValueSelection is not a valid command in this room");
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////
    #region Lobby Requests

    public virtual void RequestPlay(NetworkIdentity identity, eRoomType type)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestPlay is not a valid command in this room");
    }

    public virtual void RequestCreateRoom(NetworkIdentity con, eRoomType type)
    {
        if (!HasReceiver(con))
            return;

        Debug.Log("RequestCreateReoom is not a valid command in this room");
    }

    #endregion



    #endregion

    ///////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////
    #region Add/Remove Observers
    [Server]
    private void ConnectObserverMessenger(int id, PlayerReceiver player = null)
    {
        if (Observers == null)
            return;

        if(!Observers.ContainsKey(id))
        {
            Debug.Log("Trying to connect a client to a room it is not observering");
            return;
        }

        if(player == null)
            player = GetClient(id);

        if (player == null)
        {
            Debug.Log("Trouble finding player receiver over the network. Cannot link messenger to room " + Name);
            return;
        }

        player.SetupNetworkCommunication(this);
        Debug.Log("Client " + id + " is now linked to room " + Name);
    }

    [Server]
    protected void DisconnectObserverMessenger(int id)
    {
        if (Observers == null)
            return;

        if (!Observers.ContainsKey(id))
        {
            Debug.Log("Trying to disconnect a client from a room it is not observering");
            return;
        }

        PlayerReceiver player = GetClient(id);
        if(player == null)
        {
            Debug.Log("Trouble finding player messenger over the network. Cannot disconnect player messenger from room " + Name);
            return;
        }

        Debug.Log("TODO: Disconnec playerMessenger from room?");
    }

    [Server]
    public bool AddObserver(NetworkConnection conn)
    {

        return IsReceivingObservers() && OnAddObserver(conn);
    }

    [Server]
    private bool OnAddObserver(NetworkConnection conn)
    {
        if (conn == null)
            return false;

        int id = conn.connectionId;
        if (Observers.ContainsKey(id))
        {
            UnityEngine.Debug.Log("Room already contains player with id " + id.ToString());
            return false;
        }

        Observers.Add(id, conn);

        PlayerReceiver receiver = GetClient(id);
        ConnectObserverMessenger(id, receiver);
        receiver.EnteredRoom(Name, Type, RoomId, eRoomPlayers.Observer);

        return true;
    }

    [Server]
    public virtual void RemoveObserver(int connId)
    {
        if (!Observers.ContainsKey(connId))
            return;

        PlayerReceiver client = GetClient(connId);
        client?.ExitedRoom(Name, Type);
        client?.RemoveNetworkCommunication();
        Observers.Remove(connId);
    }

    [Server]
    public virtual int[] RemoveObservers()
    {
        int[] removed = new int[Observers.Count];
        int i = 0;
        foreach (int id in Observers.Keys)
        {
            removed[i] = id;
            i++;
        }

        Observers.Clear();

        return removed;
    }

    #endregion
}
