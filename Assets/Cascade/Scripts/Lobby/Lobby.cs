using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lobby : MonoBehaviour, IManageLobby, IAmLobby
{
    public UnityEvent OtherJoinedLobby;
    public UnityEvent OtherLeftLobby;
    public UnityEvent FoundGame;

    //-----------------------------------------
    [SerializeField] GameObject visuals = null;

    public LobbyData Data => _data;
    private LobbyData _data;

    [SerializeField] private eLobbyType _lobbyType = eLobbyType.Void;

    public float QueueTime => _queueTime;
    private float _queueTime = 0f;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Init
    private void Awake()
    {
        visuals.SetActive(false);
    }

    bool isInit = false;
    public void InitLobby(eRoomType roomTarget, LobbyPlayer[] players)
    {
        if (isInit)
            return;

        isInit = true;
        _data = new LobbyData(_lobbyType, roomTarget, players[0]);

        //Add Existing players
        int playerCount = players.Length;
        for (int i = 1; i < playerCount; i++)
            _data.AddPlayer(players[i]);
    }
    #endregion

    private void Update()
    {
        _queueTime += Time.deltaTime;
        if(_queueTime >= 120f)
        {
            _queueTime = 0f;
            Debug.Log("You have been in queue for a long time. Much wow!");
        }
        
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Lobby Interface
    public void OnJoinedLobby(LobbyData lobbyData, LobbyPlayer myPlayer)
    {
        if (!isInit)
            Debug.Log("Lobby was not initialized. Please initialize before joining players");

        if(_data.HasPlayer(myPlayer))
        {
            Debug.Log("Trouble setting up lobby. My player was not added to the lobby");
            _data.AddPlayer(myPlayer);
        }

        visuals.SetActive(true);
    }

    public void OnOtherJoinedLobby(LobbyPlayer player)
    {
        if(_data.HasPlayer(player))
        {
            Debug.Log("The lobby already contrains this player");
            return;
        }

        _data.AddPlayer(player);
        OtherJoinedLobby.Invoke();
    }

    public void OnOtherLeftLobby(LobbyPlayer player)
    {
        if(!_data.HasPlayer(player) )
        {
            Debug.Log("Tried to remove a player that was not in this lobby");
            return;
        }

        _data.RemovePlayer(player);
        OtherLeftLobby.Invoke();
    }

    public void OnFoundGame()
    {
        //Begin Enter game animation
        visuals.SetActive(false);
        Destroy(gameObject);
    }

    public void OnLobbyTimeout()
    {
        //Timeout popup. Discord link
        visuals.SetActive(false);
        Destroy(gameObject);
    }

    public void OnRemovedFromLobby()
    {
        //Return to Hub scene
        visuals.SetActive(false);
        Destroy(gameObject);
    }
    #endregion
}
