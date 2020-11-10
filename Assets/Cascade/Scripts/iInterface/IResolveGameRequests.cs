
public interface IResolveGameRequests 
{

    void RequestPlacement(eColors color, TileData tile);
    void RequestAbilitySelection(eColors forColor, eDicePlacers type);
    //void RequestPlay(int playerId, eRoomType playRoom);
}
