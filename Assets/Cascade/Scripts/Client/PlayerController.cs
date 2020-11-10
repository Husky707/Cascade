using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private PlayerTargeter Targeter = new PlayerTargeter();
    [SerializeField] PlayerReceiver Receiver = null;
    [SerializeField] PlayerMessenger Messenger = null;

    private GameController Game = null;
    private INetworkCommunicator ServerComm = null;
    private bool isInit = false;

    public uint RoomID => _roomID;
    private uint _roomID = 0;

    public int Player => _player;
    private int _player = 0;

    public eDicePlacers CurrentPlacer => _placer;
    private eDicePlacers _placer = eDicePlacers.Single;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Init

    public void Init(INetworkCommunicator server, eRoomType gameType, int player, uint room)
    {
        isInit = true;

        _player = player;
        _roomID = room;
        //Game =  CreateGame(GameByType(gameType));

        InitMessenger();
    }

    public void InitMessenger()
    {
        if (!isInit || Messenger == null)
            return;

        Messenger.DirectMessenger(ServerComm, RoomID, Player, Game.Rules);
        Debug.Log("Messenger is ready");
    }

    private void OnEnable()
    {
        if (isInit)
            Targeter.NewTarget += RequestPlacement;
    }

    private void OnDisable()
    {
        if (isInit)
            Targeter.NewTarget -= RequestPlacement;
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Outgoing Requests
    public void RequestPlacement(Tile onTile)
    {
        if (Game.CurrentTurn != Player)
            return;

        if(Game.ValidatePlacement(Player, onTile))
        {
            Game.RequestPlacement(Player, onTile);
            Messenger.RequestPlacement(onTile.Column, onTile.Row);
            Debug.Log("Player " + Player.ToString() + " placed a die");
        }
    }

    public void RequestAbilitySelection(eDicePlacers type)
    {
        if(Game.ValidateAbilitySelection(Player, type))
        {
            Game.RequestAbilitySelection(Player, type);
            Messenger.RequestAbilitySelection(type);
            Debug.Log("Player " + Player.ToString() + " changed their placer to " + type.ToString());
        }
    }

    public void RequestPlay(eRoomType type)
    {
        if(Game.ValidatePlayRequest(Player, type))
        {
            Messenger.RequestPlay(type);
        }
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
