using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerReceiver : NetworkBehaviour
{

    [SerializeField] PlayerController Player = null;
    [SerializeField] LocalLobbyManager LobbyManager = null;
    [SerializeField] LocalRoomManager RoomManager = null;

    #region Init



    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Game Messeges

    [Server]
    [TargetRpc]
    public void GameOver()
    {
        if (!hasAuthority)
            return;
    }

    [Server]
    [TargetRpc]
    public void GameStarted()
    {
        if (!hasAuthority)
            return;
    }

    [Server]
    [TargetRpc]
    public void OtherTookAction(eDicePlacers type, int xx, int yy)
    {
        if (!hasAuthority)
            return;
    }

    [Server]
    [TargetRpc]
    public void BoardUpdate(int[][] valueOwnerArray)
    {
        if (!hasAuthority)
            return;
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Room Messeges
    [Server]
    [TargetRpc]
    public void EnteredRoom()
    {
        if (!hasAuthority)
            return;
    }

    [Server]
    [TargetRpc]
    public void ExitedRoom()
    {
        if (!hasAuthority)
            return;
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

        LobbyManager?.OnJoinedLobby(lobbyStat, myLobbyPlayer);
    }

    [Server]
    [TargetRpc]
    public void OtherJoinedLobby(LobbyPlayer otherPlayer)
    {
        if (!hasAuthority)
            return;

        LobbyManager?.OnOtherJoinedLobby(otherPlayer);
    }

    [Server]
    [TargetRpc]
    public void OtherLeftLobby(LobbyPlayer otherPlayer)
    {
        if (!hasAuthority)
            return;

        LobbyManager?.OnOtherLeftLobby(otherPlayer);
    }

    [Server]
    [TargetRpc]
    public void FoundGame()
    {
        if (!hasAuthority)
            return;

        LobbyManager?.OnFoundGame();
    }

    [Server]
    [TargetRpc]
    public void LobbyTimedOut()
    {
        if (!hasAuthority)
            return;

        LobbyManager?.OnLobbyTimeout();
    }

    [Server]
    [TargetRpc]
    public void RemovedFromLobby()
    {
        if (!hasAuthority)
            return;

        LobbyManager?.OnRemovedFromLobby();
    }


    #endregion 


}
