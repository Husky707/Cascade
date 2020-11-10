using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class PlayerTargeter
{
    public event Action<Tile> NewTarget = delegate { };
    public PlayerTargeter(bool active = true)
    {
        Tile.TileTargeted += this.OnTileTargeted;
        isActive = active;
    }

    public bool isActive = true;

    private void OnTileTargeted(Tile target)
    {
        if (isActive)
            NewTarget.Invoke(target);
    }

    public void OnDestroy()
    {
        isActive = false;
        Tile.TileTargeted -= this.OnTileTargeted;
    }
}
