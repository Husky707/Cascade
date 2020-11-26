using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerLobbyManager : MonoBehaviour, IManageLobby
{




    private void Update()
    {

    }

    #region Input Events
    public void OnPlayerRequestLobby(eRoomType targetLobby)
    {

    }

    #endregion

    #region IManageLobby
    public void OnFoundGame()
    {
        throw new System.NotImplementedException();
    }

    public void OnJoinedLobby(LobbyData lobby, LobbyPlayer myPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnLobbyTimeout()
    {
        throw new System.NotImplementedException();
    }

    public void OnOtherJoinedLobby(LobbyPlayer player)
    {
        throw new System.NotImplementedException();
    }

    public void OnOtherLeftLobby(LobbyPlayer player)
    {
        throw new System.NotImplementedException();
    }

    public void OnRemovedFromLobby()
    {
        throw new System.NotImplementedException();
    }

    #endregion

}
