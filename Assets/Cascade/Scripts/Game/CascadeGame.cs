using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCascadeGame", menuName = "Cascade Game")]
public class CascadeGame : ScriptableObject
{
    public eRoomType GameType => _gameType;
    [SerializeField] private eRoomType _gameType = eRoomType.Void;
    public string GameModeName => _gameModeName;
    [SerializeField] private string _gameModeName = "...";

    public GameRules Rules => _rules;
    [SerializeField] private GameRules _rules = null;

    public GameSettings GameSettings => _gameSettings;
    [SerializeField] private GameSettings _gameSettings = null;

    public RoomSettings RoomSettings => _roomSettings;
    [SerializeField] private RoomSettings _roomSettings = null;

    //Client only visuals
    public GameBoard Board => _board;
    [SerializeField] private GameBoard _board= null;

    public GameObject Prefab => _prefab;
    [SerializeField] private GameObject _prefab = null;
}
