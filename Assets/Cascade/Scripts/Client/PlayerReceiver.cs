using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerReceiver : NetworkBehaviour
{

    [SerializeField] PlayerController Player = null;

    #region Game Messeges
    [TargetRpc]
    public void GameOver()
    {
        if (!hasAuthority)
            return;
    }

    [TargetRpc]
    public void GameStarted()
    {
        if (!hasAuthority)
            return;
    }

    [TargetRpc]
    public void OtherTookAction(eDicePlacers type, int xx, int yy)
    {
        if (!hasAuthority)
            return;
    }

    [TargetRpc]
    public void BoardUpdate(int[][] valueOwnerArray)
    {
        if (!hasAuthority)
            return;
    }

    #endregion

    #region Room Messeges
    [TargetRpc]
    public void EnteredRoom()
    {
        if (!hasAuthority)
            return;
    }

    [TargetRpc]
    public void ExitedRoom()
    {
        if (!hasAuthority)
            return;
    }
    #endregion

    #region LobbyMesseges
    [TargetRpc]
    public void JoinedLobby()
    {
        if (!hasAuthority)
            return;
    }

    [TargetRpc]
    public void OtherJoinedLobby()
    {
        if (!hasAuthority)
            return;
    }

    [TargetRpc]
    public void OtherLeftLobby()
    {
        if (!hasAuthority)
            return;
    }

    [TargetRpc]
    public void ExitedLobby(bool toGame)
    {
        if (!hasAuthority)
            return;
    }

    #endregion 


}
