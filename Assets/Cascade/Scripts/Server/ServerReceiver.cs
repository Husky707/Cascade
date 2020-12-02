using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ServerReceiver : NetworkBehaviour, INetworkCommunicator
{
    Dictionary<int, NetworkConnection> Observers = null;
    Room LinkedRoom = null;
    //INetworkCommunicator FallbackComm = null;

    ////////////////////////////////////////////////////////////////
    #region Init 
    bool isInit = false;
    [Server]
    public virtual void Init(Dictionary<int, NetworkConnection> observers)
    {
        if (isInit)
            return;

        isInit = true;

        LinkClients(observers);
    }

    [Server]
    public void LinkClients(Dictionary<int, NetworkConnection> observers)
    {
        Observers = observers;
    }

    #endregion

    [Server]
    protected virtual bool HasReceiver(NetworkIdentity identity)
    {
        if(Observers == null)
        {
            Debug.Log("Need to initialize server receiver");
            return false;
        }

        if (identity == null)
            return false;

        int id = identity.connectionToClient.connectionId;
        if (!Observers.ContainsKey(id))
        {
            Debug.Log("Unknown player requested play. ID: " + id.ToString());
            HandleUnknownPlayer(identity);
            return false;
        }

        return true;
    }

    [Server]
    protected virtual void HandleUnknownPlayer(NetworkIdentity identity)
    {
        Debug.Log("TODO: Notify Hub of missing player message. Find player. DO stuff");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Gameplay Requests

    /////////////////////////////////////////////////////////////////////////////
    ///Abilities
    public virtual void RequestPlacement(NetworkIdentity identity, uint xx, uint yy)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestPlacement is not a valid command in this room");
    }

    public virtual void RequestAbilityOrientationSelection(NetworkIdentity identity, ePlacerOrientation ePlacerOrientation)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestAbilityOreintation is not a valid command in this room");
    }

    public virtual void RequestAbilityTypeSelection(NetworkIdentity identity, eDicePlacers type)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestAbilityTypeSelection is not a valid command in this room");
    }

    public virtual void RequestAbilityValueSelection(NetworkIdentity identity, uint value)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestAbilityValueSelection is not a valid command in this room");
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Lobby Requests

    public virtual void RequestPlay(NetworkIdentity identity, eRoomType type)
    {
        if (!HasReceiver(identity))
            return;

        Debug.Log("RequestPlay is not a valid command in this room");
    }

    public virtual void RequestCreateRoom(NetworkIdentity con, eRoomType type)
    {
        if (!HasReceiver(con))
            return;

        Debug.Log("RequestCreateReoom is not a valid command in this room");
    }

    #endregion


}
