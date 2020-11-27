using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class LobbyRoom : Room, ICommToClient
{
    public event Action<LobbyRoom> LobbyClosed = delegate { };

    public LobbyData Data => _data;
    LobbyData _data;

    public float UpTime => _upTime;
    private float _upTime = 0f;

    private uint _numToStart = 2;


    private Dictionary<int, LobbyPlayer> _lobbyPlayers = new Dictionary<int, LobbyPlayer>();
    private float _maxLobbyTime = 180f;

    /////////////////////////////////////////////////////////////////////
    #region Init
    public LobbyRoom(string name, uint id, eRoomType type, RoomSettings settings, float lobbyLifetime = 180f)
              : base(name, id, type, settings)
    {
        _maxLobbyTime = lobbyLifetime;
        _numToStart = settings.MaxObservers;
        GenerateLobbyData();
    }

    private LobbyData GenerateLobbyData()
    {
        LobbyData data = new LobbyData(GenerateLobbyType(), Type);

        if(_lobbyPlayers!= null)
         foreach(LobbyPlayer player in _lobbyPlayers.Values)
            data.AddPlayer(player);

        _data = data;
        return data;
    }

    private eLobbyType GenerateLobbyType()
    {
        return eLobbyType.Clasic1v1;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////
    public void Tic()
    {
        _upTime += Time.deltaTime;
        if(_upTime >= _maxLobbyTime)
        {
            _upTime = 0f;
            LobbyTimedOut();
        }
    }

    public void PlayerJoinedLobby(NetworkConnection conn)
    {
        OnPlayerJoinedLobby(conn);
    }

    public void PlayerExitedLobby(int id)
    {
        OnPlayerLeftLobby(id);
    }

    public void Close()
    {
        //Notify observers
        foreach(int id in Observers.Keys)
        {
            PlayerReceiver client = GetClient(id);
            if (client == null)
                continue;

            client.RemovedFromLobby();
        }

        _roomIsActive = false;
        _lobbyPlayers = null;
        RemoveObservers();

        LobbyClosed.Invoke(this);
    }


    /////////////////////////////////////////////////////////////////////
    private void LobbyTimedOut()
    {
        //Notify players
        foreach (int id in Observers.Keys)
            GetClient(id)?.LobbyTimedOut();

        Close();
    }

    private void OnPlayerJoinedLobby(NetworkConnection conn)
    {
        if (!RoomIsActive || conn == null)
            return;

        if (!AddObserver(conn))
            return;

        //Create player
        LobbyPlayer newPlayer = GenerateLobbyPlayer();
        int netid = conn.connectionId;

        //Add to room data
        _lobbyPlayers.Add(netid, newPlayer);
        _data.AddPlayer(newPlayer);

        //Notify player
        GetClient(netid)?.JoinedLobby(Data, newPlayer);

        //Notify others
        foreach(int id in Observers.Keys)
        {
            if (id != netid)
                GetClient(id)?.OtherJoinedLobby(newPlayer);
        }

        TryFindGame();
    }

    private void OnPlayerLeftLobby(int id)
    {
        if (!Observers.ContainsKey(id))
            return;

        //Notify removed client
        GetClient(id)?.RemovedFromLobby();
        
        //Remove their observer
        RemoveObserver(id);

        if(NumObservers <= 0)
        {
            Debug.Log("Last player has left. Closing lobby " + Name);
            Close();
        }
        else
        {
            //Notify each other
            foreach (int obs in Observers.Keys)
                GetClient(obs)?.OtherLeftLobby(_lobbyPlayers[id]);

            //Remove their lobbyPlayer
            if (_lobbyPlayers.ContainsKey(id))
            {
                _data.RemovePlayer(_lobbyPlayers[id]);
                _lobbyPlayers.Remove(id);
            }
        }

    }

    private void TryFindGame()
    {
        if (!RoomIsActive || Observers == null)
            return;

        if(NumObservers > _numToStart)
        {
            Debug.Log("More players in the lobby than is required for the game");
            return;
        }

        if (NumObservers != _numToStart)
            return;

        foreach (int id in Observers.Keys)
            GetClient(id)?.FoundGame();
        //Still need to assign player rolls according to rules

        Close();
    }

    /////////////////////////////////////////////////////////////////////
    #region Helpers

    [Server]
    public PlayerReceiver GetClient(int targetid)
    {
        if (!Observers.ContainsKey(targetid))
            return null;

        return Observers[targetid].identity.GetComponent<PlayerReceiver>();
    }

    private List<int> uniqueIds = new List<int>();
    private LobbyPlayer GenerateLobbyPlayer()
    {
        int newID;
        do
            newID = UnityEngine.Random.Range(0, 2000000);
        while (uniqueIds.Contains(newID));

        return new LobbyPlayer(newID);
    }

    private NetworkConnection LobbyPlayerToNetConn(LobbyPlayer target)
    {
        int id = LobbyPlayerToNetPlayer(target);
        if (id == -1)
            return null;

        if (!Observers.ContainsKey(id))
            return null;

        return Observers[id];
    }

    public LobbyPlayer NetPlayerToLobbyPlayer(int playerid)
    {
        if (!HasObserver(playerid) || !_lobbyPlayers.ContainsKey(playerid))
            return default;

        return _lobbyPlayers[playerid];
    }

    private int LobbyPlayerToNetPlayer(LobbyPlayer player)
    {
        if (_lobbyPlayers == null || !_lobbyPlayers.ContainsValue(player))
            return -1;

        foreach (int id in _lobbyPlayers.Keys)
            if (_lobbyPlayers[id].ID == player.ID)
                return id;

        return -1;
    }

    #endregion

}
