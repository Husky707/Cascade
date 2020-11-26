using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class PlayerTargeter
{
    public event Action<TileData> NewTargetAquired = delegate { };
    public PlayerTargeter(bool active = true)
    {
        Tile.TileTargeted += this.OnTileTargeted;
        isActive = active;
    }

    public bool isActive = true;

    private void OnTileTargeted(TileData target)
    {
        if (isActive)
            NewTargetAquired.Invoke(target);
    }

    public void OnDestroy()
    {
        isActive = false;
        Tile.TileTargeted -= this.OnTileTargeted;
    }
}
