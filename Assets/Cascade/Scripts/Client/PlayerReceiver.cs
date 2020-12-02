using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[RequireComponent(typeof(PlayerController))]
public class PlayerReceiver : NetworkBehaviour, IReceiverServerMessages
{
    #region Events
    //Room
    public event Action<string, eRoomType, uint, eRoomPlayers> ActOnEnteredRoom = delegate { };
    public event Action<string, eRoomType>                     ActOnExitedRoom = delegate { };
    //Lobby
    public event Action<LobbyData, LobbyPlayer> ActOnJoinedLobby = delegate { };
    public event Action<LobbyPlayer>            ActOnOtherJoinedLobby = delegate { };
    public event Action<LobbyPlayer>            ActOnOtherLeftLobby = delegate { };
    public event Action                         ActOnFoundGame = delegate { };
    public event Action                         ActOnLobbyTimedOut = delegate { };
    public event Action                         ActOnRemovedFromLobby = delegate { };
    //Game
    public event Action             ActOnGameOver = delegate { };
    public event Action             ActOnGameStarted = delegate { };
    public event Action<int, eColors[]> ActOnInitPlayer = delegate { };
    public event Action<int[][]>    ActOnBoardUpdate = delegate { };
    public event Action<eDicePlacers, ePlacerOrientation, int, int ,int> ActOnOtherTookAction = delegate { };


    #endregion
    ///////////////////////////////////////////////////////

    #region Init
    public bool NetworkInit => _networkInit;
    bool _networkInit = false;
    [Server]
    public void SetupNetworkCommunication(INetworkCommunicator comm)
    {
        if (comm == null)
            return;

        PlayerController player = GetComponent<PlayerController>();
        if(player == null)
        {
            Debug.Log("Error trying to init player's netComm. No player found on receiver obj");
            return;
        }

        _networkInit = true;
        player.SetupNetworkCommunication(comm);
    }

    [Server]
    public void RemoveNetworkCommunication()
    {
        PlayerController player = GetComponent<PlayerController>();
        if (player == null)
        {
            Debug.Log("Error trying to remove player's netComm. No player found on receiver obj");
            return;
        }

        _networkInit = false;
        player.RemoveNetworkCommunication();
    }


    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Game Messeges

    [Server]
    [TargetRpc]
    public void GameOver()
    {
        if (!hasAuthority)
            return;

        ActOnGameOver.Invoke();
    }

    [Server]
    [TargetRpc]
    public void GameStarted()
    {
        if (!hasAuthority)
            return;

        ActOnGameStarted.Invoke();
    }

    [Server]
    [TargetRpc]
    public void OtherTookAction(eDicePlacers type, ePlacerOrientation orientation, int value, int xx, int yy)
    {
        if (!hasAuthority)
            return;

        ActOnOtherTookAction.Invoke(type, orientation, value, xx, yy);
    }

    [Server]
    [TargetRpc]
    public void BoardUpdate(int[][] valueOwnerArray)
    {
        if (!hasAuthority)
            return;

        ActOnBoardUpdate.Invoke(valueOwnerArray);
    }

    [Server]
    public void InitializeGamePlayer(int playerid, eColors[] colors)
    {
        PlayerController Player = GetComponent<PlayerController>();
        if (Player == null)
        {
            Debug.Log("Error trying to initialize server game player. No player found on receiver obj");
            return;
        }

        ActOnInitPlayer.Invoke(playerid, colors);
        InitLocalPlayer(playerid, colors);
    }

    [TargetRpc]
    private void InitLocalPlayer(int player, eColors[] colors)
    {
        PlayerController Player = GetComponent<PlayerController>();

        if (Player == null)
        {
            Debug.Log("Error trying to initialize local game player. No player found on receiver obj");
            return;
        }

        ActOnInitPlayer.Invoke(player, colors);
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Room Messeges
    [Server]
    [TargetRpc]
    public void EnteredRoom(string roomName, eRoomType roomType, uint roomid, eRoomPlayers asPlayer)
    {
        if (!hasAuthority)
            return;

        ActOnEnteredRoom.Invoke(roomName, roomType, roomid, asPlayer);
    }

    [Server]
    [TargetRpc]
    public void ExitedRoom(string roomName, eRoomType roomType)
    {
        if (!hasAuthority)
            return;

        ActOnExitedRoom.Invoke(roomName, roomType);
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region LobbyMesseges
    [Server]
    [TargetRpc]
    public void JoinedLobby(LobbyData lobbyStat, LobbyPlayer myLobbyPlayer)
    {
        if (!hasAuthority)
            return;

        ActOnJoinedLobby.Invoke(lobbyStat, myLobbyPlayer);
        //LobbyManager?.OnJoinedLobby(lobbyStat, myLobbyPlayer);
    }

    [Server]
    [TargetRpc]
    public void OtherJoinedLobby(LobbyPlayer otherPlayer)
    {
        if (!hasAuthority)
            return;

        ActOnOtherJoinedLobby.Invoke(otherPlayer);
        //LobbyManager?.OnOtherJoinedLobby(otherPlayer);
    }

    [Server]
    [TargetRpc]
    public void OtherLeftLobby(LobbyPlayer otherPlayer)
    {
        if (!hasAuthority)
            return;

        ActOnOtherLeftLobby.Invoke(otherPlayer);
        //LobbyManager?.OnOtherLeftLobby(otherPlayer);
    }

    [Server]
    [TargetRpc]
    public void FoundGame()
    {
        if (!hasAuthority)
            return;

        ActOnFoundGame();
        //LobbyManager?.OnFoundGame();
    }

    [Server]
    [TargetRpc]
    public void LobbyTimedOut()
    {
        if (!hasAuthority)
            return;

        ActOnLobbyTimedOut.Invoke();
        //LobbyManager?.OnLobbyTimeout();
    }

    [Server]
    [TargetRpc]
    public void RemovedFromLobby()
    {
        if (!hasAuthority)
            return;
        ActOnRemovedFromLobby.Invoke();
        //LobbyManager?.OnRemovedFromLobby();
    }


    #endregion 


}
