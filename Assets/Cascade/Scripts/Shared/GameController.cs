using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : INetworkCommunicator, IGameValidator, IResolveGameRequests
{

    public IGameRule Rules => _rules;
    private IGameRule _rules = null;
    public GameBoard Board => _board;
    private GameBoard _board = null;
    public GameData GameState => _gameState;
    private GameData _gameState = null;

    public bool GameIsActive => _gameIsActive;
    private bool _gameIsActive = false;

    PlacementHandler GameHandler => _gameHandler;
    private PlacementHandler _gameHandler = null;

    private bool isHeadless = false;
    private bool isInit = false;

    #region Init
    public GameController(IGameRule rules, GameData data, GameBoard board = null)
    {
        _board = board;
        _gameState = data;
        _rules = rules;
        _rules.InitRules(data, data.Settings);

        _gameHandler = new PlacementHandler(_gameState, _rules, _board);

        if (board == null)
            isHeadless = true;
        isInit = true;
    }
    #endregion

    public void BeginGame()
    {
        if (!isInit)
        {
            Debug.Log("Please initialize the Game Controller before starting the game");
            return ;
        }

        _gameIsActive = true;
        GameState.SetTurnToNext();
    }

    private bool LocalValidation(int player, eColors color)
    {
        if (player <= 0 || !PlayerOwnsColor(player, color) || !GameIsActive)
            return false;

        return true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Local Requests
    public virtual void RequestPlacement(int player, eColors color, uint targetTileId)
    {
        if (!LocalValidation(player, color))
            return;

        TileData targetData = GetTileFromCoords(targetTileId);
        if (!ValidatePlacement(color, targetData))
            return;

        GameHandler.HandlePlacementRequest(GameState, targetTileId);
        GameState.SetTurnToNext();
    }
    public virtual void RequestAbilityTypeSelection(int player, eColors color, eDicePlacers type)
    {
        if (!LocalValidation(player, color))
            return;

        if (!ValidateAbilityTypeSelection(color, type))
            return;

        GameState.SetPlacerType(color, type);
    }

    public virtual void RequestAbilityValueSelection(int player, eColors color, uint value)
    {
        if (!LocalValidation(player, color))
            return;

        if (!ValidateAbilityValueSelection(color, value))
            return;

        GameState.SetPlacerValue(color, value);
    }

    public virtual void RequestAbilityOrientationSelection(int player, eColors forColor, ePlacerOrientation orientation)
    {
        if (!LocalValidation(player, forColor))
            return;

        //Currently rules don't care

        GameState.SetPlacerOrientation(forColor, orientation);
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Validation
    public virtual bool ValidatePlacement(eColors color, TileData target)
    {
        if (!GameIsActive)
            return false;

        return Rules.ValidatePlacement(color, target);
    }

    public virtual bool ValidateAbilityTypeSelection(eColors color, eDicePlacers type)
    {
        if (!GameIsActive)
            return false;

        return Rules.ValidateAbilityTypeSelection(color, type);
    }

    public virtual bool ValidateAbilityValueSelection(eColors color, uint value)
    {
        if (!GameIsActive)
            return false;

        return Rules.ValidateAbilityValueSelection(color, value);
    }

    public virtual bool ValidatePlayRequest(int player, eRoomType type)
    {
        return true;
    }
    #endregion


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Helpers
    public bool PlayerOwnsColor(int player, eColors targetColor)
    {
        foreach(ColorAssignments eachRule in Rules.Settings.ColorSettings)
        {
            if (eachRule.Color == targetColor)
            {
                if (eachRule.PlayerAssignment == player)
                    return true;
                else
                    return false;
            }
        }
        return false;
    }

    private TileData GetTileFromCoords(uint tileId)
    {
        if (tileId >= GameState.BoardState.Length)
        {
            Debug.Log("Bad tile ID. Could not find tile data for id " + tileId.ToString());
            return default;
        }

        return GameState.BoardState[tileId];
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Network Requests
    public virtual void RequestPlacement(NetworkIdentity identity, uint xx, uint yy)
    {
        return;
    }

    public virtual void RequestAbilityTypeSelection(NetworkIdentity identity, eDicePlacers type)
    {
        return;
    }
    public virtual void RequestAbilityValueSelection(NetworkIdentity identity, uint value)
    {
        return;
    }
    public virtual void RequestAbilityOrientationSelection(NetworkIdentity identity, ePlacerOrientation ePlacerOrientation)
    {
        return;
    }

    public virtual void RequestPlay(NetworkIdentity identity, eRoomType type)
    {
        return;
    }

    public virtual void RequestCreateRoom(NetworkIdentity identity, eRoomType type)
    {
        return;
    }


    #endregion
}
