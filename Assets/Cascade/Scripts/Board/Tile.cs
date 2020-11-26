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
    public event Action<eTileType> TileTypeSet = delegate{ };

    public float Width => _width;
    [SerializeField] private float _width = 1f;
    public float Height => _height;
    [SerializeField] private float _height = 1f;
    public float VerticalMargin => _verticalMargin;
    [SerializeField] private float _verticalMargin = 0.1f;
    public float HorizontalMargin => _horizontalMargin;
    [SerializeField] private float _horizontalMargin = 0.1f;

    public TileData Data => _data;
    private TileData _data;

    private bool isIinit = false;

    public void Init(uint indexx, uint xx, uint yy, eTileType type = eTileType.Basic)
    {
        if (isIinit)
            return;

        isIinit = true;
        _data = new TileData(indexx, xx, yy, type);
        TileTypeSet.Invoke(type);
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
    public event Action DataChanged;

    public TileData(uint index, uint _x, uint _y, eTileType Type = eTileType.Basic, eColors Color = eColors.Noone, int Player = 0, uint Value = 0)
    {
        _index = index;
        xx = _x;
        yy = _y;
        _type = Type;
        _color = Color;
        _player = Player;
        _value = Value;

        DataChanged = delegate { };
    }

    #region Core Data

    public uint index => _index;
    private uint _index;
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

    #endregion

    #region Mutable Data
    public eColors color => _color;
    private eColors _color;

    public void ChangeColor(eColors newOwner)
    {
        _color = newOwner;
        DataChanged.Invoke();
    }

    public int player => _player;
    private int _player;
    public void ChangePlayer(int newPlayer)
    {
        _player = newPlayer;
        DataChanged.Invoke();
    }

    public uint value => _value;
    private uint _value;

    public void ChangeValue(uint newValue)
    {
        _value = newValue;
        DataChanged.Invoke();
    }


    public void ChangeData(TileData newData)
    {
        if (newData.index != index)
            Debug.Log("WARNING: Tiles do not have same index");

        ChangeOwnership(newData.player, newData.color, newData.value);
    }
    public void ChangeOwnership(int player, eColors color, uint value)
    {
        _player = player;
        _color = color;
        _value = value;
        DataChanged.Invoke();
    }
    #endregion
}