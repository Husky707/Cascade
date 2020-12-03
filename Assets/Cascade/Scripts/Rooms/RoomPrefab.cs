using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPrefab : MonoBehaviour
{
    [SerializeField] PlayerController PlayerConnection = null;

    public GameBoard GameBoard => _gameBoard;
    [SerializeField] private GameBoard _gameBoard= null;

    public bool IsHeadless => _isHeadless;
    [SerializeField] private bool _isHeadless = false;

    private bool isInit = false;
    public void Init(PlayerController player = null)
    {
        if (isInit)
            return;
        isInit = true;

        if (PlayerConnection == null)
        {
            PlayerConnection = FindObjectOfType<PlayerController>();
            if (PlayerConnection == null)
                Debug.Log("Could not find Player to initialize the Room");
        }

        if (_gameBoard == null || IsHeadless)
        {
            _isHeadless = true;
            _gameBoard = null;
        }
        else
        {
            GameBoard.Init(PlayerConnection);
        }
    }

    public void DestroyRoom()
    {
        //Maybe do cool stuff. Idk
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

}
