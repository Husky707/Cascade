using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewGameRules" , menuName = "GameSetup/GameRules")]
public class GameRules : ScriptableObject , IGameRule
{

    public GameData Data => _data;
    private GameData _data = null;
    public GameSettings Settings => _settings;
    private GameSettings _settings = null;

    protected bool _isInit = false;
    public void InitRules(GameData data, GameSettings settings)
    {
        _isInit = true;
        _data = data;
        _settings = settings;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Helpers

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region IGameRule Interface

    public virtual EffectedTileData GetEffectOnTile(TileData effector, TileData target)
    {
        Debug.Log("This ruleset does not implement 'GetEffectOnTile'");
        return default;
    }

    public virtual EffectedTileData GetPlacedDieEffect(TileData target)
    {
        Debug.Log("This ruleset does not implement 'GetPlacedDieEffect'");
        return default;
    }

    public virtual void SetCascadeScore(CascadeEventData data)
    {
        Debug.Log("This ruleset does not implement 'SetCascadeScore'");
        return;
    }

    public virtual bool ValidateAbilityTypeSelection(eColors forColor, eDicePlacers type)
    {
        Debug.Log("This ruleset does not implement 'ValidateAbilityTypeSelection'");
        return false;
    }

    public virtual bool ValidateAbilityValueSelection(eColors eColors, uint diceValue)
    {
        Debug.Log("This ruleset does not implement 'ValidateAbilityValueSelection'");
        return false;
    }

    public virtual bool ValidateCanPlaceAbility(TileDataSet[] areaInfo)
    {
        Debug.Log("This ruleset does not implement 'ValidateCanPlaceAbility'");
        return false;
    }

    public virtual bool ValidateCanPlaceDie(TileDataSet areaInfo)
    {
        Debug.Log("This ruleset does not implement 'ValidateCanPlaceDie'");
        return false;
    }

    public virtual bool ValidatePlacement(eColors color, TileData tile)
    {
        Debug.Log("This ruleset does not implement 'ValidatePlacement'");
        return false;
    }

    public virtual bool ValidatePlayRequest(int player, eRoomType type)
    {
        Debug.Log("This ruleset does not implement 'ValidatePlayRequest'");
        return false;
    }
    #endregion
}
