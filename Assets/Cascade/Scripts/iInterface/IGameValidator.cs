
public interface IGameValidator
{

    bool ValidatePlacement(eColors color, TileData tile);
    bool ValidateAbilitySelection(eColors forColor, eDicePlacers type);
    bool ValidatePlayRequest(int player, eRoomType type);

}
