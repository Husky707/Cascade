using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GameRoom 
{

    public GameRoom(RoomSettings settings, GameController controller)
    {
        //Name
        //ID
        _roomRules = controller.Rules;
        //_type = settings.
        ObserverSettings = settings;
        _roomGame = controller;
    }

    private Dictionary<int, NetworkConnection> Observers = new Dictionary<int, NetworkConnection>();

    public Dictionary<int, int> Players => _players;
    private Dictionary<int, int> _players = new Dictionary<int, int>();
    public List<int> Spectators => _spectators;
    private List<int> _spectators = new List<int>();

    public string Name => _name;
    private string _name = "Default Room";
    public uint RoomId => roomId;
    private uint roomId = 0;
    public eRoomType Type => _type;
    private eRoomType _type= eRoomType.Hub;
    public GameController RoomGame => _roomGame;
    private GameController _roomGame = null;
    public IGameRule RoomRules => _roomRules;
    private IGameRule _roomRules = null;

    private RoomSettings ObserverSettings = null;

    public bool RoomIsActive => IsRoomActive();
    private bool _roomIsActive = true;

    public bool GameIsActive => RoomGame.GameIsActive;
    public bool IsEmpty => Observers.Count <= 0;


    public void SendTest()
    {
        foreach(int id in Observers.Keys)
            Observers[id].identity.GetComponent<PlayerMessenger>().RPCTest();
    }


    #region Public Input Methods

    [Server]
    public void AddObserver(NetworkConnection conn)
    {
        if (IsReceivingObservers())
            OnAddObserver(conn);
    }

    [Server]
    public void AddPlayer(NetworkConnection conn, int requestedSpot = -1)
    {
        if (IsReceivingObservers() && IsReceivingPlayers())
            OnAddPlayer(conn.connectionId, requestedSpot);
    }

    [Server]
    public void AddSpectator(NetworkConnection conn)
    {
        if(!IsReceivingObservers() || !IsReceivingSpectators())
        {
            Debug.Log("Spectator denied");
            return;
        }

        OnAddSpectator(conn.connectionId);
    }

    #endregion

    #region Public Data Methods

    [Server]
    public bool IsRoomActive()
    {
        if (_roomIsActive == false)
            return false;

        return true;
    }

    #region PlayerData

    [Server]
    public bool RoomContainsPlayer(int targetid)
    {
        if (!Observers.ContainsKey(targetid))
            return false;

        return true;
    }

    [Server]
    public int GetIdRole(int target)
    {

        if (Players.ContainsKey(target))
            return Players[target];
        else if (Spectators.Contains(target))
            return 0;

        return -1;
    }

    [Server]
    public bool RoleIsPlayer(int role)
    {
        if (role >= 1)
            return true;
        return false;
    }

    [Server]
    public bool IdIsPlayer(int id)
    {
        if (!Observers.ContainsKey(id))
            return false;

        return RoleIsPlayer(GetIdRole(id));
    }
    #endregion

    #endregion


    #region Helpers

    #region Add Clients
    [Server]
    private void OnAddSpectator(int id)
    {
        if (Spectators.Contains(id))
        {
            Debug.Log("Spectator is already spectating");
            return;
        }

        Spectators.Add(id);
    }

    [Server]
    private void OnAddPlayer(int id, int requestedPos)
    {
        if (Players.ContainsKey(id))
        {
            Debug.Log("This room already contains the player with id " + id.ToString());
            return;
        }

        if(requestedPos > 0)
        {
            if (CanAddPlayer(requestedPos))
            {
                _players.Add(id, requestedPos);
                return;
            }
            else
            {
                Debug.Log("Requested player position is taken. Finding an alternative");
            }
        }

        int toPos = GetAvailablePlayerPosition();
        if (toPos <= 0)
        {
            Debug.Log("Failed to add player. No available positions");
        }

        _players.Add(id, toPos);
    }

    [Server]
    private int GetAvailablePlayerPosition()
    {
        int numPlayers = RoomGame.Rules.Settings.NumPlayers;
        for (int i = 0; i <= numPlayers; i++)
        {
            bool iAvailable = true;
            foreach(int pos in Players.Values)
            {
                if (pos == i)
                {
                    iAvailable = false;
                    break;
                }
            }
            if(iAvailable)
            {
                return i;
            }
        }

        Debug.Log("Could not find an open player position");
        return -1;
    }

    [Server]
    private void OnAddObserver(NetworkConnection conn)
    {

        int id = conn.connectionId;
        if(Observers.ContainsKey(id))
        {
            Debug.Log("Room already contains player with id " + id.ToString());
            return;
        }

        Observers.Add(id, conn);
    }

    [Server]
    private bool IsReceivingObservers()
    {
        if (Observers.Count >= ObserverSettings.MaxObservers)
            return false;
        if (!RoomIsActive)
            return false;

        return true;
    }

    [Server]
    private bool IsReceivingPlayers()
    {
        if (Players.Count >= RoomGame.Rules.Settings.NumPlayers)
            return false;

        return true;
    }

    [Server]
    private bool IsReceivingSpectators()
    {
        if (Spectators.Count >= ObserverSettings.MaxSpectators)
            return false;

        return true;
    }

    [Server]
    private bool CanAddPlayer(int position = -1)
    {
        int numPlayers = Players.Count;
        if (numPlayers >= RoomGame.Rules.Settings.NumPlayers)
            return false;

        if(position > -1)
        {
            foreach(int takenPos in Players.Values)
            {
                if (position == takenPos)
                    return false;
            }
        }

        return true;
    }
    #endregion

    #region Remove CLients

    [Server]
    public void RemoveObserver(int connId)
    {
        if (!Observers.ContainsKey(connId))
            return;

        Observers.Remove(connId);

        if (Players.ContainsKey(connId))
        {
            //OnRemovePlayer(connId);
        }
    }

    [Server]
    public List<int> RemoveObservers()
    {
        List<int> removed = new List<int>();
        foreach (int id in Observers.Keys)
            removed.Add(id);

        Observers.Clear();
        RemoveSpectators();
        RemovePlayers();

        return removed;
    }

    [Server]
    public List<int> RemoveSpectators()
    {
        List<int> removed = new List<int>();
        foreach (int id in Spectators)
            removed.Add(id);

        Spectators.Clear();
        foreach (int id in removed)
        {
            RemoveObserver(id);
        }

        Debug.Log("Need to message spectators to exit room");
        return removed;
    }

    [Server]
    public List<int> RemovePlayers()
    {
        List<int> removed = new List<int>();
        foreach (int id in Players.Keys)
            removed.Add(id);

        Players.Clear();
        foreach (int id in removed)
            RemoveObserver(id);

        Debug.Log("Handle player removal");

        return removed;
    }

    private void OnRemovePlayer()
    {

    }

    private void OnRemoveSpectator()
    {

    }
    #endregion

    #endregion
}
