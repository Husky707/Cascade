using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLayout", menuName = "GameSetup/BoardLayout")]
public class BoardLayout : ScriptableObject
{

    public uint Columns => _columns;
    [SerializeField] private uint _columns = 12;

    public uint Rows => _rows;
    [SerializeField] private uint _rows = 12;

    [SerializeField] private eColors[] _colors;
    [SerializeField] private int[] colorAssignment;

    [SerializeField] private int[] homeX;
    [SerializeField] private int[] homeY;
    [SerializeField] private eTileType[] homeType;
    [SerializeField] private eColors[] homeColor;
    public eTileType[] RowData => _rowData;
    private eTileType[] _rowData;



    private Dictionary<int, eTileType> homeTiles = new Dictionary<int, eTileType>();
    private Dictionary<eColors, int> playerColors = new Dictionary<eColors, int>();
    public bool DataInit => dataInit;
    private bool dataInit = false;

    [SerializeField] public Texture baseTexture;
    [SerializeField] public Texture OnTexture;

    private void Awake()
    {
        InitData();
    }
    public void InitData()
    {
        SetHomeLocations();
        SetColorOwnership();
        dataInit = true;
    }

    //Set by editor script
    public void SetRowSpecials(bool[][] data)
    {
        _rowData = new eTileType[Columns * Rows];
        for(int yy = 0; yy < Rows; yy++)
        {

            for(int xx = 0; xx < Columns; xx++)
            {
                eTileType type = eTileType.Basic; 
                if(data[yy][xx])
                    type = eTileType.Special;

                _rowData[(yy * Columns) + xx] = type;
            }
        }
    }
    public void SetHomeLocations()
    {
        homeTiles = new Dictionary<int, eTileType>();
        for (int i = 0; i < homeX.Length; i++)
        {
            homeTiles.Add(homeX[i] + (homeY[i] * (int)Columns) ,homeType[i]);
        }

        foreach(int tile in homeTiles.Keys)
        {
            RowData[tile] = homeTiles[tile];
        }
    }

    public void SetColorOwnership()
    {
        playerColors = new Dictionary<eColors, int>();
        for (int i = 0; i < _colors.Length; i++)
        {
            playerColors.Add(_colors[i], colorAssignment[i]);
        }
    }
}
