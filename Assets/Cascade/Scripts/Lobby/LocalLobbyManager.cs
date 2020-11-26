using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalLobbyManager : MonoBehaviour, IManageLobby
{
    [SerializeField] LobbyPrefabManager lobbyprefabs = null;
    [SerializeField] RectTransform Canvas = null;


    Lobby _currentLobby = null;

    ////////////////////////////////////////////////////////////////////////////////////
    #region Methods
    private void CreateLobby(LobbyData lobbyData, LobbyPlayer myPlayer)
    {
        if(lobbyprefabs == null)
        {
            Debug.Log("You forgot to assign the lobby prefab manager");
            return;
        }

        
        GameObject lobbyObj = Instantiate(lobbyprefabs.GetPrefab(lobbyData.LobbyType));
        lobbyObj.transform.SetParent(Canvas, false);

        Lobby lobby = lobbyObj.GetComponent<Lobby>();
        if(lobby == null)
        {
            Debug.Log("Lobby prefab " + lobbyObj.name + " is missing the Lobby component");
            _currentLobby = null;
            return;
        }

        _currentLobby = lobby;
        lobby.InitLobby(lobbyData.TargetRoom, lobbyData.GetPlayers());
    }

    private void DestroyLobby()
    {
        _currentLobby = null;
        //Lobby destroy's itself as part of its responce
    }

    #endregion

    /// ////////////////////////////////////////////////////////////////////////////////////
    #region Incoming Events
    public void OnFoundGame()
    {
        _currentLobby.OnFoundGame();
        DestroyLobby();
    }

    public void OnJoinedLobby(LobbyData lobbyData, LobbyPlayer myPlayer)
    {
        if(_currentLobby != null)
        {
            Debug.Log("Player is already in a lobby. Create new lobby?");
        }

        CreateLobby(lobbyData, myPlayer);
        _currentLobby.OnJoinedLobby(lobbyData, myPlayer);
    }

    public void OnOtherJoinedLobby(LobbyPlayer player)
    {
        if(_currentLobby == null)
        {
            Debug.Log("Receiving lobby events for a lobby that does not exist. Event 'OtherJoinedLobby'");
            return;
        }

        _currentLobby.OnOtherJoinedLobby(player);
    }


    public void OnOtherLeftLobby(LobbyPlayer player)
    {
        if (_currentLobby == null)
        {
            Debug.Log("Receiving lobby events for a lobby that does not exist. Event 'OtherLeftLobby'");
            return;
        }

        _currentLobby.OnOtherLeftLobby(player);
    }
    public void OnLobbyTimeout()
    {
        if (_currentLobby == null)
        {
            Debug.Log("Receiving lobby events for a lobby that does not exist. Event 'LobbyTimeout'");
            return;
        }

        _currentLobby.OnLobbyTimeout();
        DestroyLobby();
    }

    public void OnRemovedFromLobby()
    {
        if (_currentLobby == null)
        {
            Debug.Log("Receiving lobby events for a lobby that does not exist. Event 'RemovedFromLobby'");
            return;
        }

        _currentLobby.OnRemovedFromLobby();
        DestroyLobby();
    }

    #endregion
}
