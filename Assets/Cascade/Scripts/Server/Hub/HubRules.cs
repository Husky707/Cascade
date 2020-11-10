
using System.Windows.Markup;

public class HubRules : IGameRule
{

    public eRoomType GameType { get { return eRoomType.Hub; } set => value = eRoomType.Hub; }
    public uint PlayerCount { get { return 0; } set => value = 0; }

    public bool ValidateTileClick(int player, uint xx, uint yy)
    {
        return false;
    }
    public bool ValidateAbilitySelection(int player, eDicePlacers abilityType)
    {
        return false;
    }


}
