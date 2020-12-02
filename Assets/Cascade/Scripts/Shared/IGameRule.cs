
public interface IGameRule : IGameValidator
{
    GameData Data { get; }
    GameSettings Settings { get; }

    void InitRules(GameData data, GameSettings settings);
    bool ValidateCanPlaceDie(TileDataSet areaInfo);
    bool ValidateCanPlaceAbility(TileDataSet[] areaInfo);

    EffectedTileData GetEffectOnTile(TileData effector, TileData target);
    EffectedTileData GetPlacedDieEffect(TileData target);
    void SetCascadeScore(CascadeEventData data);
    

}
