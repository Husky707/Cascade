
public interface IGameRule : IGameValidator
{
    GameData Data { get; set; }
    GameSettings Settings { get; set; }

    bool ValidateCanPlaceDie(TileDataSet areaInfo);
    bool ValidateCanPlaceAbility(TileDataSet[] areaInfo);

    EffectedTileData GetEffectOnTile(TileData effector, TileData target);
    EffectedTileData GetPlacedDieEffect(TileData target);
    void SetCascadeScore(CascadeEventData data);
    

}
