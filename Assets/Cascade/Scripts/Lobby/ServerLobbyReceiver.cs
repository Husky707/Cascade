using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerLobbyReceiver : ServerReceiver
{
    ServerLobbyManager LobbyManager = null;
    Room ControllerRoom = null;

    [Server]
    public void Init(Dictionary<int, NetworkConnection> observers, ServerLobbyManager manager, Room controller)
    {
        Init(observers);
        LobbyManager = manager;
        ControllerRoom = controller;
    }


    //////////////////////////////////////////////////////////////////////////////
    #region Lobby Overrides
    public override void RequestPlay(NetworkIdentity identity, eRoomType type)
    {
        if (!HasReceiver(identity))
            return;

        if(LobbyManager.OnPlayerRequestEnterLobby(identity.connectionToClient, type))
        {
            ControllerRoom.RemoveObserver(identity.connectionToClient.connectionId);
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
