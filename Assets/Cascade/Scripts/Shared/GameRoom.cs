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
        if (controller == null)
            Debug.Log("Set up GameRoom with a null controller. Prepare for errors");
        if (controller.Rules == null)
            Debug.Log("Its the rules. They're null");

        _roomRules = controller.Rules;
        _roomGame = controller;
    }


    public Dictionary<int, int> Players => _players;//netId, player#
    private Dictionary<int, int> _players = new Dictionary<int, int>();
    public List<int> Spectators => _spectators;
    private List<int> _spectators = new List<int>();

    public GameController RoomGame => _roomGame;
    private GameController _roomGame = null;
    public IGameRule RoomRules => _roomRules;
    private IGameRule _roomRules = null;

    public bool GameIsActive => RoomGame.GameIsActive;


    ///////////////////////////////////////////////////////////////////////////////////
    #region Private Methods

    private void TryBeginGame()
    {
        int count = Players.Values.Count;
        uint numNeeded = RoomGame.GameState.NumPlayers;
        if (count < numNeeded)
        {
            Debug.Log("Not enough players ready to start game");
            return;
        }
        else if(count > numNeeded)
        {
            Debug.Log("More players were added than allowed by game rules");
            return;
        }

        if (count == numNeeded)
        {
            Debug.Log("Server Starting game");
            //Send game start message to server game controller
            RoomGame.BeginGame();
            //Send game start to each player
            foreach(int id in Players.Keys)
            {
                GetClient(id).GameStarted();
            }

        }

    }

    [Server]
    protected void InitializeGamePlayer(int id, int playerNumber, eColors[] colors = null, PlayerReceiver player = null)
    {
        if (Observers == null)
            return;

        if (!Observers.ContainsKey(id))
        {
            Debug.Log("Trying to add a player to a room it is not observering");
            return;
        }

        if (player == null)
            player = GetClient(id);

        if (player == null)
        {
            Debug.Log("Trouble finding player receiver over the network. Cannot create player data");
            return;
        }

        if (colors == null)
            colors = GetPlayerColors(playerNumber);
        if(colors == null)
        {
            Debug.Log("Trouble getting players colors. Player " + playerNumber.ToString() + " in room " + Name);
            return;
        }

        player.InitializeGamePlayer(playerNumber, colors);
    }

    [Server] 
    private eColors[] GetPlayerColors(int playerNumber)
    {

        if (playerNumber <= 0)
        {
            Debug.Log("Spectators and null players do not have a color");
            eColors[] col =  { eColors.Noone };
            return col;
        }

        //Build color array
        int count = (int)RoomRules.Data.NumPlayers;
        if(playerNumber > count)
        {
            Debug.Log("Player number exceeds player count");
            eColors[] col =  { eColors.Noone };
            return col;
        }

        List<eColors> listColors = new List<eColors>();
        foreach(eColors color in RoomRules.Data.ColorOwnership.Keys)
        {
            if (RoomRules.Data.ColorOwnership[color] == playerNumber)
                listColors.Add(color);
        }

        if(listColors.Count <= 0)
        {
            Debug.Log("Player has no colors assaigend");
            return null;
        }

        return listColors.ToArray();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////
    #region Gameplay Requests

    public override void RequestPlacement(NetworkIdentity identity, uint xx, uint yy)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestPlacement is not a valid command in this room");
    }

    public override void RequestAbilityOrientationSelection(NetworkIdentity identity, ePlacerOrientation ePlacerOrientation)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestAbilityOreintation is not a valid command in this room");
    }

    public override void RequestAbilityTypeSelection(NetworkIdentity identity, eDicePlacers type)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestAbilityTypeSelection is not a valid command in this room");
    }

    public override void RequestAbilityValueSelection(NetworkIdentity identity, uint value)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestAbilityValueSelection is not a valid command in this room");
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////
    #region Public Input Methods

    [Server]
    public bool AddPlayer(NetworkConnection conn, int requestedSpot = -1)
    {
        if (!IsReceivingObservers())
            return false;

        if (!IsReceivingPlayers())
        {
            Debug.Log("Player denied.");
            return false;
        }

        if (!AddObserver(conn))
        {
            if(!HasObserver(conn))
                return false;
        }
        return OnAddPlayer(conn.connectionId, requestedSpot);
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
    private bool OnAddPlayer(int id, int requestedPos = 0)
    {
        if (Players.ContainsKey(id))
        {
            Debug.Log("Room " + Name + "already contains the player with id " + id.ToString());
            return false;
        }


        if (requestedPos <= 0 || !CanAddPlayer(requestedPos))
        {
            requestedPos = GetAvailablePlayerPosition();
            if(!CanAddPlayer(requestedPos))
            {
                Debug.Log("Failed to add player. No available positions");
                return false;
            }
        }

        _players.Add(id, requestedPos);
        PlayerReceiver client = GetClient(id);
        InitializeGamePlayer(id, requestedPos, null, client);
        TryBeginGame();
        return true;
    }

    [Server]
    private int GetAvailablePlayerPosition()
    {
        int numPlayers = RoomGame.Rules.Settings.NumPlayers;
        for (int i = 1; i <= numPlayers; i++)
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
        if (Players != null && Players.Count >= RoomGame.Rules.Settings.NumPlayers)
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
