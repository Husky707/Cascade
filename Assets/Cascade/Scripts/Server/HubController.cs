using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubController : GameController
{


    [Server]
    public override void RequestPlay(NetworkIdentity identity, eRoomType type)
    {
        Debug.Log("I have received your request to play and I deny you");
    }

    [Server]
    public override void RequestCreateRoom(NetworkIdentity identity, eRoomType type)
    {        return;    }
    public override void RequestAbilitySelection(NetworkIdentity identity, uint roomid, eDicePlacers type)
    { return; }
    public override void RequestPlacement(NetworkIdentity identity, uint roomid, uint xx, uint yy)
    { return; }
}
