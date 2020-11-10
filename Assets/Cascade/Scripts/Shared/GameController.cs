using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : INetworkCommunicator, IGameValidator, IResolveGameRequests
{

    public event Action<int> TurnChange = delegate { };

    public IGameRule Rules => _rules;
    private IGameRule _rules = null;
    public GameBoard Board => _board;
    private GameBoard _board = null;
    public GameData GameState => _gameState;
    private GameData _gameState = null;

    public bool GameIsActive => _gameIsActive;
    private bool _gameIsActive = false;

    #region Init
    public GameController CreateGame(eRoomType type)
    {
        return this;
    }

    #endregion

    public void BeginGame()
    {
        NextTurn();
        _gameIsActive = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Validation
    public virtual bool ValidatePlacement(eColors color, TileData target)
    {
        if (!GameIsActive)
            return false;

        return Rules.ValidatePlacement(color, target);
    }

    public virtual bool ValidateAbilitySelection(eColors color, eDicePlacers type)
    {
        if (!GameIsActive)
            return false;

        return Rules.ValidateAbilitySelection(color, type);
    }

    public virtual bool ValidatePlayRequest(int player, eRoomType type)
    {
        return true;
    }
    #endregion


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Requests
    public virtual void RequestPlacement(eColors color, TileData target)
    {
        if (!ValidatePlacement(color, target))
            return;
        //Place dice
    }
    public virtual void RequestAbilitySelection(eColors color, eDicePlacers type)
    {
        if (!ValidateAbilitySelection(color, type))
            return;

        GameState.
    }


    #endregion


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Network Requests
    public virtual void RequestPlacement(NetworkIdentity identity, uint roomid, uint xx, uint yy)
    {
        throw new System.NotImplementedException();
    }

    public virtual void RequestAbilitySelection(NetworkIdentity identity, uint roomid, eDicePlacers type)
    {
        throw new System.NotImplementedException();
    }

    public virtual void RequestPlay(NetworkIdentity identity, eRoomType type)
    {
        throw new System.NotImplementedException();
    }

    public virtual void RequestCreateRoom(NetworkIdentity identity, eRoomType type)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
