using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCreator : MonoBehaviour
{
    private CascadeGame Game;
    [SerializeField] eRoomType GameType = eRoomType.Hub;
    public GameCreator(eRoomType gameType)
    {
        //Create the game
    }

}
