using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRoomSettings", menuName = "RoomSettings")]
public class RoomSettings : ScriptableObject
{
    public RoomSettings(uint observers, uint spectators)
    {
        _MaxObservers = observers;
        _MaxSpectators = spectators;
    }

    public uint MaxObservers => _MaxObservers;
    [SerializeField] private uint _MaxObservers = 8;

    public uint MaxSpectators => _MaxSpectators;
    [SerializeField] private uint _MaxSpectators = 4;

    public void Init(uint observerLimit, uint spectatorLimit)
    {
        _MaxObservers = observerLimit;
        _MaxSpectators = spectatorLimit;
    }
}
