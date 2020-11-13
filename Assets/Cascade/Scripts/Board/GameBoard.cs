using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public bool CreateOnInit => _creatOnInit;
    [SerializeField] private bool _creatOnInit = true;
    public GameObject TilePrefab => _tilePrefab;
    [SerializeField] private GameObject _tilePrefab = null;
    public BoardLayout Layout => _layout;
    [SerializeField]private BoardLayout _layout;


    public Tile[] GameTiles => _gameTiles;
    private Tile[] _gameTiles;
    private PlayerController playerConnection = null;
    private bool isInit = false;

    /// /////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Init
    private void Awake()
    {
        if(isPreveiwed)
        {
            isPreveiwed = false;
            ClearPreveiw();
        }
    }
    public void Init(PlayerController player)
    {
        if (isInit)
            return;

        if (playerConnection == null)
        {
            playerConnection = FindObjectOfType<PlayerController>();
            if (playerConnection == null)
            {
                Debug.Log("Could not find Player to initialize the gameboard");
                return;
            }
        }

        isInit = true;
        playerConnection = player;
        if (CreateOnInit)
            CreateGameBoard();
    }
    #endregion
    public void CreateGameBoard()
    {
        SpawnTiles();
    }

    private void SpawnTiles()
    {

    }

    private void ClearTiles()
    {
        int count = GameTiles.Length;
        for (int i = 0; i < count; i++)
        {
            Destroy(_gameTiles[i].gameObject);
        }
        _gameTiles = new Tile[0];
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Editor Preveiw
    private bool isPreveiwed = false;
    public void Preveiw()
    {
        if (Application.isPlaying || UnityEditor.EditorApplication.isPlaying == true)
            return;

        if (isPreveiwed) return;
        isPreveiwed = true;

        SpawnTiles();
    }

    public void ClearPreveiw()
    {
        if (isInit && Application.isPlaying)
            return;

        if (!isPreveiwed) return;
        isPreveiwed = false;

        ClearTiles();
    }
    #endregion
}
