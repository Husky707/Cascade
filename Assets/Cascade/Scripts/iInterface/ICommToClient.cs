
using System.Collections.Generic;

public interface ICommToClient
{
    PlayerReceiver GetClient(int targetid);
    //return Observers[id].identity.GetComponent<PlayerReceiver>();
}
