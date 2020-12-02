using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerHubReceiver : ServerReceiver
{
    ServerLobbyManager LobbyManager = null;

    [Server]
    public void Init(Dictionary<int, NetworkConnection> observers, ServerLobbyManager manager)
    {
        Init(observers);
        LobbyManager = manager;
    }


    //////////////////////////////////////////////////////////////////////////////
    #region Lobby Overrides
    public override void RequestPlay(NetworkIdentity identity, eRoomType type)
    {
        if (!HasReceiver(identity))
            return;

        if (LobbyManager.OnPlayerRequestEnterLobby(identity.connectionToClient, type))
        {
        }
    }

    public override void RequestCreateRoom(NetworkIdentity con, eRoomType type)
    {
        if (!HasReceiver(con))
            return;

        Debug.Log("Request Create Room Not implemented");
    }
    #endregion
}
