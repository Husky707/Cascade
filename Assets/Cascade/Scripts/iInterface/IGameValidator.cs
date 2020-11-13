
public interface IGameValidator
{

    bool ValidatePlacement(eColors color, TileData tile);
    bool ValidateAbilityTypeSelection(eColors forColor, eDicePlacers type);
    bool ValidateAbilityValueSelection(eColors eColors, uint diceValue);
    bool ValidatePlayRequest(int player, eRoomType type);

}
