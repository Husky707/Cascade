using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tile))]
public class TileVisuals : MonoBehaviour
{



    Tile tile;

    // ------------------------------------------------------------------------------------------------
    #region Init
    private void Awake()
    {
        tile = GetComponent<Tile>();
    }


    private void OnEnable()
    {
        tile.TileTypeSet += OnTileTypeSet;
    }

    private void OnDisable()
    {
        tile.TileTypeSet -= OnTileTypeSet;
    }
    #endregion


    private void OnTileTypeSet(eTileType tileType)
    {

    }
}
