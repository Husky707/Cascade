using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using JetBrains.Annotations;

public class GameData
{

    #region Events
    public static event Action GameStarted = delegate { };
    public static event Action GameEnded = delegate { };
    public static event Action<int, int, int> ScoreChanged = delegate { };//player , to, from
    public static event Action<eColors, eColors> ColorTurnChanged = delegate { };//To, from
    public static event Action<int, int> PlayerTurnChanged = delegate { };//To, from
    public static event Action<eColors, eDicePlacers> PlacerChanged = delegate { };
    public static event Action<eColors, ePlacerOrientation> PlacerOrientationChanged = delegate { };
    public static event Action<eColors, eDicePlacers, uint> PlacerUsed = delegate { };
    public static event Action<int> RoundComplete = delegate { };

    public event Action<eColors, eColors> DiceCaptured = delegate { };//captured color, captured by
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Getters
    public bool PlayerOwnsColor(int player, eColors targetColor)
    {
        return ColorOwnership[targetColor] == player;
    }


    #endregion 

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Init
    public GameData(BoardLayout layout, GameSettings settings)
    {
        int numPlayers = settings.NumPlayers;
        int numColors = settings.ColorSettings.Length;

        InitGameBoard(layout);
        _score = new int[numPlayers];
        _numPlayers = (uint)numPlayers;

        List<eColors> gameColors = new List<eColors>(numColors);
        foreach (ColorAssignments color in settings.ColorSettings)
            gameColors.Add(color.Color);

        InitColorLists(gameColors);
        InitInnerDicts(gameColors);

        _colorOwnership = new Dictionary<eColors, int>(numColors);
        for(int i = 0; i < numColors; i++)
            _colorOwnership.Add(settings.ColorSettings[i].Color, settings.ColorSettings[i].PlayerAssignment);

        _turnOrder = settings.TurnOrder;
        _currentTurn = _turnOrder[0];
        _currentPlayer = ColorOwnership[_currentTurn];
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
                uint listIndex = (uint)((yy * cols) + xx);
                _boardState[(yy * cols) + xx] = new TileData(listIndex, (uint)xx, (uint)yy, layout.RowData[listIndex]);
            }
        }
    }

    private void InitColorLists(List<eColors> availableColors)
    {
        int count = availableColors.Count;
        _numLostToCapture = new Dictionary<eColors, uint>(count);
        _numOpponentsCaptured = new Dictionary<eColors, uint>(count);
        _currentPlacers = new Dictionary<eColors, eDicePlacers>(count);
        _currentPlacerOrientation = new Dictionary<eColors, ePlacerOrientation>(count);
        _currentPlacerValue = new Dictionary<eColors, uint>(count);

        foreach(eColors color in availableColors)
        {
            _numOpponentsCaptured.Add(color, 0);
            _numLostToCapture.Add(color, 0);
            _currentPlacers.Add(color, eDicePlacers.Single);
            _currentPlacerOrientation.Add(color, ePlacerOrientation.Quad1);
            _currentPlacerValue.Add(color, 0);
        }
    }

    private void InitInnerDicts(List<eColors> gameColors)
    {
        int count = gameColors.Count;
        _usedSpecialValues = new Dictionary<eColors, Dictionary<uint, uint>>(count);
        _usedSpecials = new Dictionary<eColors, Dictionary<eDicePlacers, uint>>(count);
        foreach(eColors eachColor in gameColors)
        {

            Dictionary<eDicePlacers, uint> newDict = new Dictionary<eDicePlacers, uint>();
            Dictionary<uint, uint> newDict2 = new Dictionary<uint, uint>(7);
            foreach(eDicePlacers eachPlacer in (eDicePlacers[])Enum.GetValues(typeof(eDicePlacers)))
            {
                newDict.Add(eachPlacer, 0);
            }
            for(int i = 0; i < 7; i++)
            {
                newDict2.Add((uint)i, 0);
            }
            _usedSpecials.Add(eachColor, newDict);
            _usedSpecialValues.Add(eachColor, newDict2);
        }

    }



    #endregion

    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public TileData[] BoardState => _boardState;
    private TileData[] _boardState;
    public bool GameOver => _gameOver;
    private bool _gameOver = false;

    public int[] Score => _score;
    private int[] _score = new int[6];
    public Dictionary<eColors, uint> NumOpponentsCaptured => _numOpponentsCaptured;
    private Dictionary<eColors, uint> _numOpponentsCaptured;
    public Dictionary<eColors, uint> NumLostToCapture => _numLostToCapture;
    private Dictionary<eColors, uint> _numLostToCapture;

    public uint NumPlayers => _numPlayers;
    private uint _numPlayers = 2;

    public int GameRoundCount { get => _roundCount; private set { _roundCount = value; RoundComplete.Invoke(value); } }
    private int _roundCount = 0;
    public int turnIndex { get => _turnIndex; private set { _turnIndex = value; if (_turnIndex >= TurnOrder.Length) { _turnIndex = 0; _roundCount++; } } }
    private int _turnIndex = 0;
    public eColors[] TurnOrder => _turnOrder;
    private eColors[] _turnOrder;
    public eColors CurrentTurn => _currentTurn;
    private eColors _currentTurn = eColors.Noone;
    public int CurrentPlayer => _currentPlayer;
    private int _currentPlayer = 0;
    public Dictionary<eColors, int> ColorOwnership => _colorOwnership;
    private Dictionary<eColors, int> _colorOwnership;
    public Dictionary<eColors, eDicePlacers> CurrentPlacers => _currentPlacers;
    private Dictionary<eColors, eDicePlacers> _currentPlacers;
    public Dictionary<eColors, ePlacerOrientation> CurrentPlacerOrientation => _currentPlacerOrientation;
    private Dictionary<eColors, ePlacerOrientation> _currentPlacerOrientation;
    public Dictionary<eColors, uint> CurrentPlacerValue => _currentPlacerValue;//Zero implies unknown
    private Dictionary<eColors, uint> _currentPlacerValue;
    public Dictionary<eColors, Dictionary<uint, uint>> UsedSpecialValues => _usedSpecialValues;
    private Dictionary<eColors, Dictionary<uint, uint>> _usedSpecialValues;
    public Dictionary<eColors, Dictionary<eDicePlacers, uint>> UsedSpecialTypes => _usedSpecials;
    private Dictionary<eColors, Dictionary<eDicePlacers, uint>> _usedSpecials;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///Setters
    public void IncrementScore(int player)
    {
        SetScore(player, Score[player] + 1);
    }
    public void SetScore(int player, int value)
    {
        if (player >= Score.Length || player < 0)
            return;

        int prev = _score[player];
        _score[player] = value;

        ScoreChanged.Invoke(player, value, prev);
    }

    public void SetGameOver(bool state)
    {
        _gameOver = state;
        if (state)
            GameStarted.Invoke();
        else
            GameEnded.Invoke();
    }

    public void SetTurnToNext()
    {
        SetTurnTo(turnIndex + 1) ;
    }
    public void SetTurnTo(int turn)
    {
        eColors prevCol = TurnOrder[turnIndex];
        int prevPlayer = ColorOwnership[prevCol];
        turnIndex = turn;
        
        _currentTurn = TurnOrder[turnIndex];
        ColorTurnChanged.Invoke(TurnOrder[turnIndex], prevCol);

        int newPlayer = ColorOwnership[TurnOrder[turnIndex]];
        if (prevPlayer != newPlayer)
            PlayerTurnChanged.Invoke(newPlayer, prevPlayer);
    }

    public void SetPlacerType(eColors forColor, eDicePlacers type)
    {
        _currentPlacers[forColor] = type;
        PlacerChanged.Invoke(forColor, type);
    }

    public void SetPlacerValue(eColors forColor, uint value)
    {
        if (value == 0 || value > 6)
            return;
        _currentPlacerValue[forColor] = value;
    }

    public void SetPlacerOrientation(eColors forColor, ePlacerOrientation direction)
    {
        _currentPlacerOrientation[forColor] = direction;
        PlacerOrientationChanged.Invoke(forColor, direction);
    }

    public void SetPlayerSpecialTypeCount(eColors forColor,eDicePlacers type, int newTotal)
    {
        UsedSpecialTypes[forColor][type] = (uint)newTotal;
        PlacerUsed.Invoke(forColor, type, (uint)newTotal);
    }
    public void IncrementPlayerSpecialTypeCount(eColors forColor, eDicePlacers type)
    {
        SetPlayerSpecialTypeCount(forColor, type, (int)UsedSpecialTypes[forColor][type] + 1);
    }

    public void SetPlayerSpecialValueCount(eColors forColor, uint value, uint count)
    {
        UsedSpecialValues[forColor][value] = count;
    }
    public void IncrementPlayerSpecialValueCount(eColors forColor, uint value)
    {
        SetPlayerSpecialValueCount(forColor, value, UsedSpecialValues[forColor][value] + (uint)1);
    }

    public void AlterBoardTile(uint targetIndex, TileData newData)
    {
        if (targetIndex != newData.index)
            Debug.Log("Missmatching index in altering board data");

        _boardState[targetIndex] = newData;
    }

}

