using UnityEngine;
using System;


[RequireComponent(typeof(PlayerReceiver))]
public class LocalLobbyManager : MonoBehaviour, IManageLobby
{

    public event Action EFoundGame = delegate { };
    public event Action<LobbyData, LobbyPlayer> EJoinedLobby = delegate { };
    public event Action EQuitLobby = delegate { };
    public event Action ELobbyTimeout = delegate { };

    public event Action<LobbyPlayer> EOtherJoinedLobby = delegate { };
    public event Action<LobbyPlayer> EOtherLeftLobby = delegate { };

    //////////////////////////////////////////////////////////
    [SerializeField] LobbyPrefabManager lobbyprefabs = null;
    [SerializeField] RectTransform Canvas = null;


    Lobby _currentLobby = null;
    ////////////////////////////////////////////////////////////////////////////////////
    #region Init
    private void OnEnable()
    {
        PlayerReceiver Receiver = GetComponent<PlayerReceiver>();
        if (Receiver == null) Debug.Log("Can't init Local lobby manager. Can't find Receiver");
        //Lobby Events
        Receiver.ActOnJoinedLobby += OnJoinedLobby;
        Receiver.ActOnRemovedFromLobby += OnRemovedFromLobby;
        Receiver.ActOnOtherJoinedLobby += OnOtherJoinedLobby;
        Receiver.ActOnOtherLeftLobby += OnOtherLeftLobby;
        Receiver.ActOnLobbyTimedOut += OnLobbyTimeout;
        Receiver.ActOnFoundGame += OnFoundGame;
    }

    private void OnDisable()
    {
        PlayerReceiver Receiver = GetComponent<PlayerReceiver>();
        if (Receiver == null) Debug.Log("Can't clear Local lobby manager. Can't find Receiver");
        //Lobby Events
        Receiver.ActOnJoinedLobby -= OnJoinedLobby;
        Receiver.ActOnRemovedFromLobby -= OnRemovedFromLobby;
        Receiver.ActOnOtherJoinedLobby -= OnOtherJoinedLobby;
        Receiver.ActOnOtherLeftLobby -= OnOtherLeftLobby;
        Receiver.ActOnLobbyTimedOut -= OnLobbyTimeout;
        Receiver.ActOnFoundGame -= OnFoundGame;
    }

    #endregion

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
        if(_currentLobby == null)
        {
            Debug.Log("Communicating with a lobby that has not been created. Msg: OnFoundGame");
        }    

        _currentLobby.OnFoundGame();
        EFoundGame.Invoke();
    }

    public void OnJoinedLobby(LobbyData lobbyData, LobbyPlayer myPlayer)
    {
        if(_currentLobby != null)
        {
            Debug.Log("Player is already in a lobby. Leaving lobby now.");
            OnRemovedFromLobby();
        }

        CreateLobby(lobbyData, myPlayer);
        _currentLobby.OnJoinedLobby(lobbyData, myPlayer);
        EJoinedLobby.Invoke(lobbyData, myPlayer);
    }

    public void OnOtherJoinedLobby(LobbyPlayer player)
    {
        if(_currentLobby == null)
        {
            Debug.Log("Receiving lobby events for a lobby that does not exist. Event 'OtherJoinedLobby'");
            return;
        }

        _currentLobby.OnOtherJoinedLobby(player);
        EOtherJoinedLobby.Invoke(player);
    }


    public void OnOtherLeftLobby(LobbyPlayer player)
    {
        if (_currentLobby == null)
        {
            Debug.Log("Receiving lobby events for a lobby that does not exist. Event 'OtherLeftLobby'");
            return;
        }

        _currentLobby.OnOtherLeftLobby(player);
        EOtherLeftLobby.Invoke(player);
    }
    public void OnLobbyTimeout()
    {
        if (_currentLobby == null)
        {
            Debug.Log("Receiving lobby events for a lobby that does not exist. Event 'LobbyTimeout'");
            return;
        }

        _currentLobby.OnLobbyTimeout();
        ELobbyTimeout.Invoke();
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
        EQuitLobby.Invoke();
        DestroyLobby();
    }

    #endregion
}
