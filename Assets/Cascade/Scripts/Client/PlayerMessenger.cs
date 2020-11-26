using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UIElements;


public class PlayerMessenger : NetworkBehaviour
{
    //All Possible Messegas to send to the Server

    //Null unless on server
    private INetworkCommunicator ServerLink = null;
    public uint RoomID => _roomID;
    private uint _roomID = 0;

    public int Player => _player;
    private int _player = 0;

    public void DirectMessenger(INetworkCommunicator server, uint roomId, int player)
    {
        _roomID = roomId;
        _player = player;
        ServerLink = server;
    }

    [Client]
    public void RequestPlacement(uint xx, uint yy)
    {
        if (!hasAuthority)
            return;

            OnRequestPlacement(xx, yy);
    }

    [Client]
    public void RequestAbilitySelection(eDicePlacers type)
    {
        if (!hasAuthority)
            return;

        OnRequestAbilitySelection(type);
    }

    [Client]
    public void RequestPlay(eRoomType type)
    {
        if (!hasAuthority)
            return;

        if (RoomID == 0)
            OnRequestPlay(type);
    }

    [Client]
    public void RequestCreateRoom(eRoomType type)
    {
        if (!hasAuthority)
            return;
        return;
    }



    [Command]
    private void OnRequestPlacement(uint xx, uint yy)
    {
        ServerLink.RequestPlacement(this.netIdentity, RoomID, xx, yy);
    }

    [Command]
    private void OnRequestAbilitySelection(eDicePlacers type)
    {
        ServerLink.RequestAbilityTypeSelection(this.netIdentity, RoomID, type);
    }

    [Command]
    private void OnRequestPlay(eRoomType type)
    {
        if (ServerLink == null)
        {
            Debug.Log("Server Link is null. Cannot send messages");
            return;
        }

        ServerLink.RequestPlay(this.netIdentity, type);
    }

    [TargetRpc]
    public void SetUpPlayer(uint roomid, int player)
    {
        _roomID = roomid;
        _player = player;
        Debug.Log("Player " + player.ToString() + " has been setup in room " + roomid.ToString());
    }

    [TargetRpc]
    public void RPCTest()
    {
        Debug.Log("Omg Messege received");
    }
}
