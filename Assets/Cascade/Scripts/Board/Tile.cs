using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Rendering;
using System.Data;

[RequireComponent(typeof(Collider))]
public class Tile : MonoBehaviour, ITargetable
{
    public static event Action<TileData> TileTargeted = delegate { };

    public TileData Data => _data;
    private TileData _data;

    private bool isIinit = false;

    public void Init(uint xx, uint yy, eTileType type = eTileType.Basic)
    {
        if (isIinit)
            return;

        isIinit = true;
        _data = new TileData(xx, yy, type);
    }

    private void OnMouseUpAsButton()
    {
        Targeted();
    }

    public void Targeted()
    {
        TileTargeted.Invoke(Data);
    }
}



public struct TileData
{
    public TileData(uint _x, uint _y, eTileType Type = eTileType.Basic, eColors Color = eColors.Noone, int Player = 0, uint Value = 0)
    {
        xx = _x;
        yy = _y;
        _type = Type;
        _color = Color;
        _player = Player;
        _value = Value;
    }

    public uint x => xx;
    private uint xx;

    public uint y => yy;
    private uint yy;

    public eTileType type => _type;
    private eTileType _type;

    public void SetType(eTileType newType)
    {
        _type = newType;
    }

    public eColors color => _color;
    private eColors _color;

    public void ChangeColor(eColors newOwner)
    {
        _color = newOwner;
    }

    public int player => _player;
    private int _player;
    public void ChangePlayer(int newPlayer)
    {
        _player = newPlayer;
    }

    public uint value => _value;
    private uint _value;

    public void ChangeValue(uint newValue)
    {
        _value = newValue;
    }

}