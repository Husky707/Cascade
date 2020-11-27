using System.Collections;
using System.Collections.Generic;
using Mirror;

public class Room 
{
    //int: ID created by Mirror for each NetConn
    protected Dictionary<int, NetworkConnection> Observers = new Dictionary<int, NetworkConnection>();
    public int NumObservers => Observers.Count;

    public string Name => _name;
    protected string _name = "Default Room";
    public uint RoomId => _roomId;
    protected uint _roomId = 0;
    public eRoomType Type => _type;
    protected eRoomType _type = eRoomType.Void;

    protected RoomSettings ObserverSettings = null;

    public bool RoomIsActive => IsRoomActive();
    protected bool _roomIsActive = false;
    public bool IsEmpty => Observers.Count <= 0;

    ///////////////////////////////////////////////////////////////////////////////////
    #region Init
    public Room(string name, uint id, eRoomType type, RoomSettings settings)
    {
        _name = name;
        _roomId = id;
        _type = type;
        ObserverSettings = settings;

        _roomIsActive = true;
    }

    #endregion

    public void SendTest()
    {
        foreach (int id in Observers.Keys)
            Observers[id].identity.GetComponent<PlayerMessenger>().RPCTest();
    }


    ///////////////////////////////////////////////////////////////////////////////////
    [Server]
    public bool IsRoomActive()
    {
        if (_roomIsActive == false)
            return false;

        return true;
    }

    [Server]
    public bool HasObserver(int target)
    {
        if (Observers.ContainsKey(target))
            return true;

        return false;
    }

    [Server]
    protected bool IsReceivingObservers()
    {
        if (Observers.Count >= ObserverSettings.MaxObservers)
            return false;
        if (!RoomIsActive)
            return false;

        return true;
    }

    /// ///////////////////////////////////////////////////////////////////////////////////
    #region Add/Remove Observers
    [Server]
    public bool AddObserver(NetworkConnection conn)
    {

        return IsReceivingObservers() && OnAddObserver(conn);
    }

    [Server]
    private bool OnAddObserver(NetworkConnection conn)
    {

        int id = conn.connectionId;
        if (Observers.ContainsKey(id))
        {
            UnityEngine.Debug.Log("Room already contains player with id " + id.ToString());
            return false;
        }

        Observers.Add(id, conn);
        return true;
    }

    [Server]
    public virtual void RemoveObserver(int connId)
    {
        if (!Observers.ContainsKey(connId))
            return;

        Observers.Remove(connId);
    }

    [Server]
    public virtual int[] RemoveObservers()
    {
        int[] removed = new int[Observers.Count];
        int i = 0;
        foreach (int id in Observers.Keys)
        {
            removed[i] = id;
            i++;
        }

        Observers.Clear();

        return removed;
    }
    #endregion
}
