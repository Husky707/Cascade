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
        _isActive = active;
    }

    public bool isActive => _isActive;
    private bool _isActive = false;

    private void OnTileTargeted(TileData target)
    {
        if (_isActive)
            NewTargetAquired.Invoke(target);
    }

    public void OnDestroy()
    {
        _isActive = false;
        Tile.TileTargeted -= this.OnTileTargeted;
    }
}
