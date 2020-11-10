using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : NetworkBehaviour
{


    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("OnStartServer");
    }
}
