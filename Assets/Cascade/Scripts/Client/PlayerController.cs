using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerMessenger))]
[RequireComponent(typeof(PlayerReceiver))]
public class PlayerController : MonoBehaviour
{

    private PlayerTargeter Targeter = new PlayerTargeter();
    private PlayerMessenger Messenger = null;
    private PlayerReceiver Receiver = null;

    private GameController Game = null;
    private INetworkCommunicator ServerComm = null;
    private bool isInit = false;

    public uint RoomID => _roomID;
    private uint _roomID = 0;
    public int Player => _player;
    private int _player = 0;
    public eColors[] Colors => _colors;
    private eColors[] _colors;

    private eColors CurrentColor => Colors[curColorIndex];
    public int curColorIndex { get => _colindex; private set { _colindex = value; if (value < 0 || value >= Colors.Length) _colindex = 0; } }
    private int _colindex = 0;

    public eDicePlacers CurrentPlacer => _placer;
    private eDicePlacers _placer = eDicePlacers.Single;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Init

    private void Awake()
    {
        Receiver = GetComponent<PlayerReceiver>();
        Messenger = GetComponent<PlayerMessenger>();
    }

    public void Init(INetworkCommunicator server, eRoomType gameType, int player, uint room)
    {
        isInit = true;

        ServerComm = server;
        _player = player;
        _roomID = room;
        //Game =  CreateGame(GameByType(gameType));

        InitMessenger();
    }

    public void InitMessenger()
    {
        if (!isInit || Messenger == null)
            return;

        Messenger.DirectMessenger(ServerComm, RoomID, Player);
        Debug.Log("Messenger is ready");
    }

    private void OnEnable()
    {
        if (isInit)
            Targeter.NewTargetAquired += RequestPlacement;
    }

    private void OnDisable()
    {
        if (isInit)
            Targeter.NewTargetAquired -= RequestPlacement;
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Outgoing Requests
    public void RequestPlacement(TileData onTile)
    {
        //Debug.Log("Aquired a target. Making a placemetn request");

        if (Game.GameState.CurrentTurn != CurrentColor)
            return;

        if(Game.ValidatePlacement(CurrentColor, onTile))
        {
            Game.RequestPlacement(Player, CurrentColor, onTile.index);
            Messenger.RequestPlacement(onTile.x, onTile.y);
            Debug.Log("Player " + Player.ToString() + " placed a die");
        }
    }

    public void RequestAbilityTypeSelection(eDicePlacers type)
    {
        if(Game.ValidateAbilityTypeSelection(CurrentColor, type))
        {
            Game.RequestAbilityTypeSelection(Player, CurrentColor, type);
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
