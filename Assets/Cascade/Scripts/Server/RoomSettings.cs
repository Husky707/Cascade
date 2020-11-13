using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRoomSettings", menuName = "GameSetup/RoomSettings")]
public class RoomSettings : ScriptableObject
{
    public void Init(uint observers, uint spectators, bool isHeadless = false)
    {
        _MaxObservers = observers;
        _MaxSpectators = spectators;
        _isHeadless = isHeadless;
    }

    public bool IsHeadless => _isHeadless;
    [SerializeField] private bool _isHeadless = false;
    public uint MaxObservers => _MaxObservers;
    [SerializeField] private uint _MaxObservers = 8;

    public uint MaxSpectators => _MaxSpectators;
    [SerializeField] private uint _MaxSpectators = 4;

}
