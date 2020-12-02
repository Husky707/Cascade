using UnityEngine;
using System;
using Mirror;


[RequireComponent(typeof(PlayerMessenger))]
[RequireComponent(typeof(PlayerReceiver))]
[RequireComponent(typeof(PlayerAnouncer))]
[RequireComponent(typeof(LocalLobbyManager))]
public class PlayerController : MonoBehaviour
{

    private PlayerTargeter Targeter = new PlayerTargeter();
    private PlayerMessenger Messenger = null;

    private GameController Game = null;
    private bool playerInit = false;
    private bool roomInit = false;

    public uint RoomID => _roomID;
    private uint _roomID = 0;
    public eRoomType RoomType => _roomType;
    private eRoomType _roomType = eRoomType.Void;

    public int Player => _player;
    private int _player = 0;
    public eRoomPlayers RoomPlayer => _roomPlayer;
    private eRoomPlayers _roomPlayer = eRoomPlayers.Void;
    public eColors[] Colors => _colors;
    private eColors[] _colors = null;

    private eColors CurrentColor => Colors[curColorIndex];
    public int curColorIndex { get => _colindex; private set { _colindex = value; if (value < 0 || value >= Colors.Length) _colindex = 0; } }
    private int _colindex = 0;

    public eDicePlacers CurrentPlacer => _placer;
    private eDicePlacers _placer = eDicePlacers.Single;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Init

    private void Awake()
    {
        Messenger = GetComponent<PlayerMessenger>();
    }

    private void InitPlayerData(int player, eColors[] playerColors)
    {
        if (player == 0 || player == -1)
            Debug.Log("GamePlayer assienged as spectator or observer");

        playerInit = true;

        _player = player;
        _colors = playerColors;
    }

    private void InitRoom(eRoomType roomType, uint roomid, eRoomPlayers roomPlayer)
    {
        if (roomType == eRoomType.Void || roomPlayer == eRoomPlayers.Void)
            return;

        if (roomPlayer == eRoomPlayers.Void || roomType == eRoomType.Void)
        {
            Debug.Log("Trying to initialize room player with void values");
            return;
        }

        roomInit = true;
        _roomPlayer = roomPlayer;
        _roomType = roomType;
        _roomID = roomid;
    }

    [Server]
    private void InitMessenger(INetworkCommunicator netComm)
    {
        if (netComm == null)
        {
            Debug.Log("Requested player to init messenger with a null NetworkCommunicator");
            return;
        }

        Messenger.SetupNetworkCommunication(netComm);
    }

    private void OnEnable()
    {
        Targeter.NewTargetAquired += RequestPlacement;

        //Receiver events
        PlayerReceiver Receiver = GetComponent<PlayerReceiver>();
        //Room
        Receiver.ActOnEnteredRoom += OnEnteredRoom;
        Receiver.ActOnExitedRoom += OnExitedRoom;
        //Game
        Receiver.ActOnGameStarted += OnGameStarted;
        Receiver.ActOnGameOver += OnGameOver;
        Receiver.ActOnOtherTookAction += OnOtherTookAction;
        Receiver.ActOnInitPlayer += InitPlayerData;
    }

    private void OnDisable()
    {
        Targeter.NewTargetAquired -= RequestPlacement;

        //Receiver events
        PlayerReceiver Receiver = GetComponent<PlayerReceiver>();
        //Room
        Receiver.ActOnEnteredRoom -= OnEnteredRoom;
        Receiver.ActOnExitedRoom -= OnExitedRoom;
        //Game
        Receiver.ActOnGameStarted -= OnGameStarted;
        Receiver.ActOnGameOver -= OnGameOver;
        Receiver.ActOnOtherTookAction -= OnOtherTookAction;
        Receiver.ActOnInitPlayer -= InitPlayerData;
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Outgoing Requests
    public void RequestPlacement(TileData onTile)
    {
        if (!playerInit)
            return;
        //Debug.Log("Aquired a target. Making a placement request");

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
        if (!playerInit)
            return;

        if (Game.ValidateAbilityTypeSelection(CurrentColor, type))
        {
            Game.RequestAbilityTypeSelection(Player, CurrentColor, type);
            Messenger.RequestAbilityTypeSelection(type);
            Debug.Log("Player " + Player.ToString() + " changed their placer to " + type.ToString());
        }
    }

    public void RequestPlay(eRoomType type)
    {
        if (!roomInit)
            return;

        //If currently in game
        if(Game != null)
        {
            if (Game.ValidatePlayRequest(Player, type))
            {
                Messenger.RequestPlay(type);
            }
            //else game denied request
        }
        else
        {
            Messenger.RequestPlay(type);
        }

    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Receiver Event Reactions
    //Room Events
    private void OnEnteredRoom(string roomName, eRoomType roomType, uint roomid, eRoomPlayers asPlayer)
    {
        InitRoom(roomType, roomid, asPlayer);
        Debug.Log(Player.ToString() + " entered room " + roomName);
    }

    private void OnExitedRoom(string roomName, eRoomType roomType)
    {
        RemoveRoom(roomType);
        Debug.Log(Player.ToString() + " exited room " + roomName);
    }

    //Game Events
    private void OnGameOver()
    {

    }

    private void OnGameStarted()
    {

    }

    private void OnOtherTookAction(eDicePlacers placer, ePlacerOrientation oreintation, int value, int xx, int yy)
    {

    }

    private void OnBoardUpdate(int[][] valueOwnerArray)
    {

    }


    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Private Methods

    public void ChangeActiveColor(eColors toColor)
    {

    }

    private void RemoveRoom(eRoomType roomType)
    {
        if(roomType != RoomType)
        {
            Debug.Log("Player removing a room is does not have");
            return;
        }

        RemovePlayer();

        roomInit = false;
        _roomPlayer = eRoomPlayers.Void;
        _roomID = 0;
        _roomType = eRoomType.Void;

        Debug.Log("Room Removed");
    }

    public void RemovePlayer()
    {
        playerInit = false;
        _player = -1;
        _colors = null;
        _colindex = 0;
    }

    [Server]
    public void SetupNetworkCommunication(INetworkCommunicator netCom)
    {
        InitMessenger(netCom);
    }

    [Server]
    public void RemoveNetworkCommunication()
    {
        Messenger.RemoveNetworkCommunication();
    }

    #endregion
}
