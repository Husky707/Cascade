
using Mirror;

public interface INetworkCommunicator
{
    //Bridge between Server and Client methods

    /////////////////////////////////////////////////////////////////////////////
    ///Gameplay
    void RequestPlacement(NetworkIdentity identity, uint xx, uint yy);
    void RequestAbilityTypeSelection(NetworkIdentity identity,  eDicePlacers type);
    void RequestAbilityValueSelection(NetworkIdentity identity, uint value);
    void RequestAbilityOrientationSelection(NetworkIdentity identity, ePlacerOrientation ePlacerOrientation);

    /////////////////////////////////////////////////////////////////////////////
    ///Lobby
    void RequestPlay(NetworkIdentity identity, eRoomType type);
    void RequestCreateRoom(NetworkIdentity identity, eRoomType type);//Probably need to add room settings to this


    /////////////////////////////////////////////////////////////////////////////
}
