using System.Collections;
using System.Collections.Generic;

public struct LobbyData
{
    public eLobbyType LobbyType => _lobbyType;
    eLobbyType _lobbyType;

    public eRoomType TargetRoom => _targetType;
    eRoomType _targetType;

    public int NumPlayers => _currentPlayers.Count;
    List<LobbyPlayer> _currentPlayers;

    public LobbyData(eLobbyType lobbyType, eRoomType roomType, LobbyPlayer firstPlayer)
    {
        _lobbyType = lobbyType;
        _targetType = roomType;
        
        _currentPlayers = new List<LobbyPlayer>(1);
        _currentPlayers.Add(firstPlayer);
    }
    public LobbyData(eLobbyType lobbyType, eRoomType roomType)
    {
        _lobbyType = lobbyType;
        _targetType = roomType;

        _currentPlayers = new List<LobbyPlayer>(0);
    }

    public LobbyPlayer[] GetPlayers()
    {
        if (_currentPlayers == null || _currentPlayers.Count == 0)
            return null;

        LobbyPlayer[] players = new LobbyPlayer[NumPlayers];
        for(int i = 0; i < NumPlayers; i++)
            players[i] = _currentPlayers[i];

        return players;
    }

    public LobbyPlayer GetPlayer(int index)
    {
        if (_currentPlayers.Count > index)
            return _currentPlayers[index];

        return default;
    }

    public bool HasPlayer(LobbyPlayer target)
    {
        return _currentPlayers.Contains(target);
    }

    public void AddPlayer(LobbyPlayer player)
    {
        if(!_currentPlayers.Contains(player))
            _currentPlayers.Add(player);
    }

    public void RemovePlayer(LobbyPlayer player)
    {
        if (!_currentPlayers.Contains(player))
            UnityEngine.Debug.Log("Lobby data did not have the player. Can not remove them");

        _currentPlayers.Remove(player);
    }
}
