using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerController))]
public class PlayerMessenger : NetworkBehaviour
{
    //All Possible Messegas to send to the Server

    public INetworkCommunicator ServerLink { get { if (!isServer) return null; return _serverLink; }}
    private INetworkCommunicator _serverLink = null;

    #region Init
    [Server]
    public void SetupNetworkCommunication(INetworkCommunicator netCom)
    {
        if (netCom == null)
            return;

        _serverLink = netCom;
    }

    [Server]
    public void RemoveNetworkCommunication()
    {
        if (_serverLink == null)
            return;

        _serverLink = null;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    #region Clientside Calls

    ////////////////////////////////////////////////////////////////////////////////
    #region Gameplay Requests

    [Client]
    public void RequestPlacement(uint xx, uint yy)
    {
        if (!hasAuthority)
            return;

            OnRequestPlacement(xx, yy);
    }

    [Client]
    public void RequestAbilityTypeSelection(eDicePlacers type)
    {
        if (!hasAuthority)
            return;

        OnRequestAbilityTypeSelection(type);
    }

    [Client]
    public void RequestAbilityValueSelection(uint value)
    {
        if (!hasAuthority)
            return;

        OnRequestAbilityValueSelection(value);
    }

    [Client]
    public void RequestAbilityOrientationSelection(ePlacerOrientation orientation)
    {
        if (!hasAuthority)
            return;

        OnRequestAbilityOrientationSelection(orientation);
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////
    #region Lobby Requests
    [Client]
    public void RequestPlay(eRoomType type)
    {
        if (!hasAuthority)
            return;

        OnRequestPlay(type);
    }

    [Client]
    public void RequestCreateRoom(eRoomType type)
    {
        if (!hasAuthority)
            return;

        Debug.Log("Creating rooms not implemented");
        return;
    }

    #endregion

    #endregion


    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    #region Send To Server

    /////////////////////////////////////////////////////////////////////////////
    #region Gameplay commands

    [Command]
    private void OnRequestPlacement(uint xx, uint yy)
    {
        if (_serverLink == null)
        {
            Debug.Log("Server Link is null. Cannot send messages");
            return;
        }

        _serverLink.RequestPlacement(this.netIdentity, xx, yy);
    }

    [Command]
    private void OnRequestAbilityTypeSelection(eDicePlacers type)
    {
        if (_serverLink == null)
        {
            Debug.Log("Server Link is null. Cannot send messages");
            return;
        }

        _serverLink.RequestAbilityTypeSelection(this.netIdentity, type);
    }

    [Command]
    private void OnRequestAbilityValueSelection(uint value)
    {
        if (_serverLink == null)
        {
            Debug.Log("Server Link is null. Cannot send messages");
            return;
        }

        _serverLink.RequestAbilityValueSelection(this.netIdentity, value);
    }

    [Command]
    private void OnRequestAbilityOrientationSelection(ePlacerOrientation orientation)
    {
        if (_serverLink == null)
        {
            Debug.Log("Server Link is null. Cannot send messages");
            return;
        }

        _serverLink.RequestAbilityOrientationSelection(this.netIdentity, orientation);
    }
    #endregion

    /////////////////////////////////////////////////////////////////////////////
    #region Lobby Commands
    [Command]
    private void OnRequestPlay(eRoomType type)
    {
        if (_serverLink == null)
        {
            Debug.Log("Server Link is null. Cannot send messages");
            return;
        }

        Debug.Log("Sending play request to server");
        _serverLink.RequestPlay(this.netIdentity, type);
    }

    #endregion

    #endregion



    /////////////////////////////////////////////////////////////////////////////




}
