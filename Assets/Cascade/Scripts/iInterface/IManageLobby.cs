
public interface IManageLobby
{

    void OnJoinedLobby(LobbyData lobby, LobbyPlayer myPlayer);
    void OnOtherJoinedLobby(LobbyPlayer player);
    void OnOtherLeftLobby(LobbyPlayer player);
    void OnFoundGame();
    void OnLobbyTimeout();
    void OnRemovedFromLobby();
}
