using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class ServerLobbyManager : MonoBehaviour
{


    private LobbyRoom[] _currentLobbies;
    public int NumLobbies => _currentLobbies.Length;

    public bool AllowNewLobbies => _allowNewLobies;
    private bool _allowNewLobies = true;

    public bool AllowJoinLobies => _allowJoinLobies;
    private bool _allowJoinLobies = true;

    private List<eRoomType> _bannedLobbies = new List<eRoomType>();


    /////////////////////////////////////////////////////////////////////
    #region Init
    private void Awake()
    {
        _bannedLobbies.Add(eRoomType.Void);
    }

    private void OnEnable()
    {
        if (_currentLobbies == null || NumLobbies <= 0)
            return;

        foreach (LobbyRoom lobby in _currentLobbies)
            AddEventListeners(lobby);
    }

    private void OnDisable()
    {
        if (_currentLobbies == null || NumLobbies <= 0)
            return;

        foreach (LobbyRoom lobby in _currentLobbies)
            RemoveEventListeners(lobby);
    }

    private void AddEventListeners(LobbyRoom target)
    {
        if (target == null || !HasLobby(target))
            return;

        target.LobbyClosed += OnLobbyClosed;
    }

    private void RemoveEventListeners(LobbyRoom target)
    {
        if (target == null || !HasLobby(target))
            return;

        target.LobbyClosed -= OnLobbyClosed;
    }
    #endregion


    private void Update()
    {
        if (_currentLobbies == null)
            return;

        int count = _currentLobbies.Length;
        for(int i = 0; i < count; i++)
        {
            _currentLobbies[i].Tic();
        }
    }

    public bool PlayerIsInAnyLobby(int targetPlayer)
    {
        if (_currentLobbies == null || _currentLobbies.Length <= 0)
            return false;

        foreach (LobbyRoom lobby in _currentLobbies)
            if (lobby.HasObserver(targetPlayer))
                return true;

        return false;
    }

    public bool PlayerIsInLobby(int playerid, int lobbyid)
    {
        if (_currentLobbies == null || _currentLobbies.Length <= 0)
            return false;

        foreach(LobbyRoom lobby in _currentLobbies)
        {
            if(lobby.RoomId == lobbyid)
            {
                if (lobby.HasObserver(playerid))
                    return true;
                else return false;
            }
        }

        return false;
    }

    private bool HasLobby(LobbyRoom target)
    {
        for (int i = 0; i < NumLobbies; i++)
        {
            if (_currentLobbies[i] == target)
                return true;
        }
        return false;
    }

    /////////////////////////////////////////////////////////////////////
    #region Input Events
    public void OnPlayerRequestEnterLobby(NetworkConnection player, eRoomType targetLobby)
    {
        if (!AllowJoinLobies || targetLobby == eRoomType.Void)
            return;

        if(LobbyIsBanned(targetLobby))
        {
            Debug.Log("Player " + player.connectionId.ToString() + " tried to enter a banned lobby. Lobby: " + targetLobby.ToString());
            return;
        }

        LobbyRoom lobby = GetLobbyByRoomType(targetLobby);
        //Does lobby with target exits
        if(lobby == null)
        {
            //No: Create new lobby
            if (!AllowNewLobbies)
                return;

            lobby = NewLobby(targetLobby);
            lobby.PlayerJoinedLobby(player);
            return;
        }
        //Yes: add to that lobby
        else
        {
            lobby.PlayerJoinedLobby(player);
        }

    }

    public void OnPlayerRequestExitLobby(int playerid, eRoomType targetLobby)
    {           OnPlayerRequestExitLobby(playerid, GetLobbyByRoomType(targetLobby));
    }
    public void OnPlayerRequestExitLobby(int playerid, int lobbyid)
    {           OnPlayerRequestExitLobby(playerid, GetLobbyByID(lobbyid));
    }
    private void OnPlayerRequestExitLobby(int playerid, LobbyRoom lobby)
    {
        //Is the player in the targetLobby?
        if (lobby == null)
        {
            //The target lobby does not exist
            return;
        }
        else
        {
            if (lobby.HasObserver(playerid))
            {
                lobby.PlayerExitedLobby(playerid);
            }
            else
            {
                //Did not find observer in provided room. 
            }
        }
    }

    //Timeout, disconnect, server shutdown. i.e. Not player request
    public void OnPlayerExitLobby(int playerid, int lobbyid)
    {
        if (_currentLobbies == null || NumLobbies <= 0)
            return;

        LobbyRoom lobby = GetLobbyByID(lobbyid);
        if(lobby != null && lobby.HasObserver(playerid))
        {
            lobby.PlayerExitedLobby(playerid);
        }
    }

    private void OnLobbyClosed(LobbyRoom target)
    {
        if (!HasLobby(target))
            return;

        RemoveEventListeners(target);
        RemoveLobbyFromArray(target);
        target = null;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////
    #region Control Methods
    public void CloseLobbies()
    {
        if (_currentLobbies == null || NumLobbies <= 0)
            return;

        int count = NumLobbies;
        for(int i = 0; i < count; i++)
        {
            RemoveEventListeners(_currentLobbies[i]);
            _currentLobbies[i].Close();
            _currentLobbies[i] = null;
        }

        lobbyIds = new List<int>();
        _currentLobbies = new LobbyRoom[0];
    }

    public void CloseLobby(LobbyRoom target)
    {
        if (_currentLobbies == null || NumLobbies <= 0)
            return;
        if (!HasLobby(target))
            return;

        RemoveEventListeners(target);
        RemoveLobbyFromArray(target);
        target.Close();
    }

    public void DissableNewLobbies()
    {
        _allowNewLobies = false;
    }

    public void EnableNewLobbies()
    {
        _allowNewLobies = true;
    }

    public void DisableJoinLobbies()
    {
        _allowJoinLobies = false;
        CloseLobbies();
    }

    public void EnableJoinLobbies()
    {
        _allowJoinLobies = true;
    }
    public void BanLobbies(List<eRoomType> types)
    {
        if (types == null)
            return;

        foreach(eRoomType type in types)
            BanLobby(type);
    }
    public void BanLobby(eRoomType type)
    {
        if(!_bannedLobbies.Contains(type))
            _bannedLobbies.Add(type);
    }

    public void UnbanLobby(eRoomType type)
    {
        if (_bannedLobbies.Contains(type))
            _bannedLobbies.Remove(type);
    }

    #endregion

    /////////////////////////////////////////////////////////////////////
    #region Methods
    private LobbyRoom NewLobby(eRoomType gameType)
    {
        if (!AllowNewLobbies)
            return null;

        string name = gameType.ToString() + " Lobby";
        uint id = (uint)GenerateLobbyID();
        RoomSettings settings = new RoomSettings();

        Debug.Log("Need to set room settings. Using temp values");
        settings.Init(2, 2);

        LobbyRoom newLobby = new LobbyRoom(name, id, gameType, settings);
        AddLobbyToArray(newLobby);
        AddEventListeners(newLobby);

        return newLobby;
    }

    private bool LobbyIsBanned(eRoomType target)
    {
        return _bannedLobbies.Contains(target);
    }

    #endregion

    /////////////////////////////////////////////////////////////////////
    #region Helpers

    /// /////////////////////////////////////////////////////////////////////
    /// Array Helpers
    private void AddLobbyToArray(LobbyRoom newLobby)
    {
        if (_currentLobbies == null)
        {
            _currentLobbies = new LobbyRoom[1] { newLobby };
            return;
        }

        int len = NumLobbies;
        Array.Resize(ref _currentLobbies, len + 1);
        _currentLobbies[len] = newLobby;
    }

    private void RemoveLobbyFromArray(LobbyRoom target)
    {
        if (_currentLobbies == null)
            return;

        int targetIndex = GetIndexOfLobby(target);
        //Lobby does not exist
        if (targetIndex == -1)
            return;

        if(targetIndex == NumLobbies - 1)
        {
            _currentLobbies[NumLobbies - 1] = null;
            Array.Resize(ref _currentLobbies, NumLobbies - 1);
        }
        else if(targetIndex == 0)
        {
            _currentLobbies[0] = _currentLobbies[NumLobbies - 1];
            Array.Resize(ref _currentLobbies, NumLobbies - 1);
        }
        else
        {
            _currentLobbies[targetIndex] = _currentLobbies[NumLobbies - 1];
            Array.Resize(ref _currentLobbies, NumLobbies - 1);
        }
    }

    private int GetIndexOfLobby(LobbyRoom target)
    {
        for(int i = 0; i < NumLobbies; i++)
        {
            if (_currentLobbies[i] == target)
                return i;
        }
        return -1;
    }

    /////////////////////////////////////////////////////////////////////
    List<int> lobbyIds = new List<int>();
    private int GenerateLobbyID()
    {
        int newID;
        do
        {
            newID = UnityEngine.Random.Range(1, 200000);
        }
        while (lobbyIds.Contains(newID));
        lobbyIds.Add(newID);

        return newID;
    }

    private LobbyRoom GetLobbyByRoomType(eRoomType target)
    {
        if (_currentLobbies == null || NumLobbies <= 0)
            return null;

        foreach (LobbyRoom lobby in _currentLobbies)
        {
            if (lobby.Type == target)
                return lobby;
        }

        return null;
    }

    private LobbyRoom GetLobbyByID(int target)
    {
        if (_currentLobbies == null || NumLobbies <= 0)
            return null;

        foreach (LobbyRoom lobby in _currentLobbies)
            if (lobby.RoomId == target)
                return lobby;

        return null;
    }

    private LobbyRoom GetLobbyByPlayer(int playerid)
    {
        if (_currentLobbies == null || NumLobbies <= 0)
            return null;

        foreach(LobbyRoom lobby in _currentLobbies)
        {
            if (lobby.HasObserver(playerid))
                return lobby;
        }

        return null;
    }


    #endregion

}
