public interface IReceiverServerMessages : IReceiverGameMessages, IReceiveLobbyMessages, IReceiveRoomMessages, IReceiveNetworkMessages
{
}

public interface IReceiveRoomMessages
{
    void EnteredRoom(string roomName, eRoomType roomType, uint roomid, eRoomPlayers asPlayer);
    void ExitedRoom(string roomName, eRoomType roomType);
}


public interface IReceiverGameMessages
{
    void GameOver();
    void GameStarted();

    void OtherTookAction(eDicePlacers type, ePlacerOrientation orientation, int value, int xx, int yy);
    void BoardUpdate(int[][] valueOwnerArray);

    void InitializeGamePlayer(int player, eColors[] colors);
}

public interface IReceiveLobbyMessages
{
    void JoinedLobby(LobbyData lobbyStat, LobbyPlayer myLobbyPlayer);
    void RemovedFromLobby();
    void OtherJoinedLobby(LobbyPlayer otherPlayer);
    void OtherLeftLobby(LobbyPlayer otherPlayer);
    void FoundGame();
    void LobbyTimedOut();
}

public interface IReceiveNetworkMessages
{
    void SetupNetworkCommunication(INetworkCommunicator comm);
    void RemoveNetworkCommunication();

}


