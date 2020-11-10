
using Mirror;

public interface INetworkCommunicator
{
    //Bridge between Server and Client methods
    void RequestPlacement(NetworkIdentity identity, uint roomid, uint xx, uint yy);

    void RequestAbilitySelection(NetworkIdentity identity, uint roomid, eDicePlacers type);

    void RequestPlay(NetworkIdentity identity, eRoomType type);

    void RequestCreateRoom(NetworkIdentity identity, eRoomType type);//Probably need to add room settings to this

}
