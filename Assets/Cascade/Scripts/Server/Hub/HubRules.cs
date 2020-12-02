
using System.Windows.Markup;

public class HubRules : IGameRule
{

    public eRoomType GameType { get { return eRoomType.Hub; } set => value = eRoomType.Hub; }
    public uint PlayerCount { get { return 0; } set => value = 0; }

    public GameData Data { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool AllowAbilities { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public GameSettings Settings { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public bool ValidateTileClick(int player, uint xx, uint yy)
    {
        return false;
    }
    public bool ValidateAbilitySelection(int player, eDicePlacers abilityType)
    {
        return false;
    }

    public bool ValidatePlacement(eColors color, TileData tile)
    {
        throw new System.NotImplementedException();
    }

    public bool ValidateAbilityTypeSelection(eColors forColor, eDicePlacers type)
    {
        throw new System.NotImplementedException();
    }

    public bool ValidatePlayRequest(int player, eRoomType type)
    {
        throw new System.NotImplementedException();
    }

    public bool ValidateCanPlaceDie(GameData data, TileDataSet areaInfo)
    {
        throw new System.NotImplementedException();
    }

    public bool ValidateCanPlaceAbility(GameData data, TileDataSet[] areaInfo)
    {
        throw new System.NotImplementedException();
    }

    public bool ValidateCanPlaceDie(TileDataSet areaInfo)
    {
        throw new System.NotImplementedException();
    }

    public bool ValidateCanPlaceAbility(TileDataSet[] areaInfo)
    {
        throw new System.NotImplementedException();
    }

    public EffectedTileData GetEffectOnTile(TileData effector, TileData target)
    {
        throw new System.NotImplementedException();
    }

    public EffectedTileData GetPlacedDieEffect(TileData target)
    {
        throw new System.NotImplementedException();
    }

    public void SetCascadeScore(CascadeEventData data)
    {
        throw new System.NotImplementedException();
    }

    public bool ValidateAbilityValueSelection(eColors eColors, uint diceValue)
    {
        throw new System.NotImplementedException();
    }

    public void InitRules(GameData data, GameSettings settings)
    {
        throw new System.NotImplementedException();
    }
}
