using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HubRoom : Room
{
    ServerController Server = null;
    ServerLobbyManager LobbyManager = null;

    public HubRoom(ServerController server, string name, uint id, eRoomType type, RoomSettings settings)
             :base(name, id, type, settings)
    {
        Server = server;
        LobbyManager = RoomObject.AddComponent<ServerLobbyManager>();

    }



    //////////////////////////////////////////////////////////////////////////////
    #region Lobby Overrides
    public override void RequestPlay(NetworkIdentity identity, eRoomType type)
    {
        if (!HasReceiver(identity))
            return;

        NetworkConnection connection = identity.connectionToClient;
        if (LobbyManager.OnPlayerRequestEnterLobby(connection, type))
        {
            RemoveObserver(connection.connectionId);
            LobbyManager.AddPlayerToLobby(connection, type);
        }
        else
            Debug.Log("Player request to join a lobbby of type " + type.ToString() + " was denied");
    }

    public override void RequestCreateRoom(NetworkIdentity con, eRoomType type)
    {
        if (!HasReceiver(con))
            return;

        Debug.Log("Request Create Room Not implemented");
    }
    #endregion
}
