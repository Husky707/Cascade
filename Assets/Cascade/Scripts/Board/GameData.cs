using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GameData 
{
    #region Init
    public GameData(BoardLayout layout, uint numPlayers, uint numColors)
    {
        InitGameBoard(layout);
        _score = new int[numPlayers];
    }

    private void InitGameBoard(BoardLayout layout)
    {
        layout.InitData();
        int cols = (int)layout.Columns;
        int rows = (int)layout.Rows;
        _boardState = new TileData[layout.Columns * layout.Rows];

        for(int yy = 0; yy < rows; yy++)
        {
            for(int xx = 0; xx < cols; xx++)
            {
                BoardState[(yy * cols) + xx] = new TileData((uint)xx, (uint)yy, layout.RowData[(yy * cols) + xx]);
            }
        }
    }
    #endregion

    public TileData[] BoardState => _boardState;
    private TileData[] _boardState;
    public bool GameOver => _gameOver;
    private bool _gameOver = false;

    public int[] Score => _score;
    private int[] _score = new int[6];

    public int NumPlayers = 2;
    public eColors CurrentTurn => _currentTurn;
    private eColors _currentTurn = eColors.Noone;
    public int CurrentPlayer => _currentPlayer;
    private int _currentPlayer = 0;
    public Dictionary<eColors, eDicePlacers> CurrentPlacers => _currentPlacers;
    private Dictionary<eColors, eDicePlacers> _currentPlacers = new Dictionary<eColors, eDicePlacers>(7);
    public Dictionary<eColors, int> UsedSpecials => _usedSpecials;
    private Dictionary<eColors, int> _usedSpecials = new Dictionary<eColors, int>(7);


    public void IncrementScore(int player)
    {
        SetScore(player, Score[player] + 1);
    }
    public void SetScore(int player, int value)
    {
        if (player >= Score.Length || player < 0)
            return;

        _score[player] = value;
    }

}

