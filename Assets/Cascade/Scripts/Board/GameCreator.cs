using UnityEngine;

public class GameCreator : MonoBehaviour
{

    [SerializeField] private CascadeGame[] CascadeGames;

    public CascadeGame GetGame(eRoomType target)
    {
        if(CascadeGames == null)
        {
            Debug.Log("Game creator not set up correctly");
            return null;
        }

        foreach(CascadeGame type in CascadeGames)
        {
            if (type.GameType == target)
                return type;
        }

        return null;
    }

}
