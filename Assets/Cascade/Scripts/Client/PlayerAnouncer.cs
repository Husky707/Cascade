using UnityEngine;
using Mirror;
using System;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnouncer : NetworkBehaviour
{

    public static event Action<NetworkIdentity> PlayerUpdated = delegate { };

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        PlayerUpdated?.Invoke(base.netIdentity);
    }

    private void OnDestroy()
    {
        if (base.isLocalPlayer)
            PlayerUpdated?.Invoke(null);
    }
}
