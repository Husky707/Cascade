
public interface IResolveGameRequests 
{

    void RequestPlacement(int player, eColors color, uint tileId);
    void RequestAbilityTypeSelection(int player, eColors forColor, eDicePlacers type);
    void RequestAbilityValueSelection(int player, eColors forColor, uint value);
    void RequestAbilityOrientationSelection(int player, eColors forColr, ePlacerOrientation orientation);
    //void RequestPlay(int playerId, eRoomType playRoom);
}
