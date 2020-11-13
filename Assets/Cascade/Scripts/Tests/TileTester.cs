using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileTester : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text = null;

    private void OnEnable()
    {
        Tile.TileTargeted += OnTileTargeted;
    }

    private void OnTileTargeted(TileData data)
    {

        text.text = "Targeted " + data.type.ToString();
    }
}
