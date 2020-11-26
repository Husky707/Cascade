using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GameRoom : Room
{
    public GameRoom(string name, uint id, eRoomType type, RoomSettings settings, GameController controller)
              :base(name, id, type, settings)
    {

        _roomRules = controller.Rules;
        _roomGame = controller;
    }


    public Dictionary<int, int> Players => _players;
    private Dictionary<int, int> _players = new Dictionary<int, int>();
    public List<int> Spectators => _spectators;
    private List<int> _spectators = new List<int>();

    public GameController RoomGame => _roomGame;
    private GameController _roomGame = null;
    public IGameRule RoomRules => _roomRules;
    private IGameRule _roomRules = null;

    public bool GameIsActive => RoomGame.GameIsActive;


    ///////////////////////////////////////////////////////////////////////////////////
    #region Public Input Methods

    [Server]
    public void AddPlayer(NetworkConnection conn, int requestedSpot = -1)
    {
        if (!IsReceivingObservers())
            return;

        if (!IsReceivingPlayers())
        {
            Debug.Log("Player denied.");
            return;
        }

        AddObserver(conn);
        OnAddPlayer(conn.connectionId, requestedSpot);
    }

    [Server]
    public void AddSpectator(NetworkConnection conn)
    {
        if (!IsReceivingObservers())
            return;

        if (!IsReceivingSpectators())
        {
            Debug.Log("Spectator denied.");
            return;
        }

        AddObserver(conn);
        OnAddSpectator(conn.connectionId);
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////////
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


    ///////////////////////////////////////////////////////////////////////////////////
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

    [Server]
    public override void RemoveObserver(int connId)
    {
        base.RemoveObserver(connId);

        if (Players.ContainsKey(connId))
        {
            Players.Remove(connId);
            OnPlayerRemoved(connId);
        }

        if(Spectators.Contains(connId))
        {
            Spectators.Remove(connId);
            OnSpectatorRemoved(connId);
        }
    }

    [Server]
    public override int[] RemoveObservers()
    {
        int[] removed = base.RemoveObservers();
        foreach(int connId in removed)
        {
            if (Players.ContainsKey(connId))
            {
                Players.Remove(connId);
                OnPlayerRemoved(connId);
            }

            if (Spectators.Contains(connId))
            {
                Spectators.Remove(connId);
                OnSpectatorRemoved(connId);
            }
        }

        return removed;
    }

    [Server]
    protected virtual void OnPlayerRemoved(int id)
    {

    }

    [Server]
    protected virtual void OnSpectatorRemoved(int id)
    {

    }

    #endregion

    #endregion
}
