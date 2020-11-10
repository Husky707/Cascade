
public interface IGameRule : IGameValidator
{
    GameData Data { get; set; }
    eRoomType GameType { get; set; }
    bool AllowAbilities { get; set; }

}
