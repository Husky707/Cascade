using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TestManager : NetworkManager
{

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        Debug.Log("Decide what to do with this new client");
    }
}
