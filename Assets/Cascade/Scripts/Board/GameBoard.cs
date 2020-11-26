using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public bool CreateOnInit => _creatOnInit;
    [SerializeField] private bool _creatOnInit = true;
    public GameObject TilePrefab => _tilePrefab;
    [SerializeField] private GameObject _tilePrefab = null;
    public Transform BoardRoot => _boardRootTransform;
    [SerializeField] private Transform _boardRootTransform = null;
    public BoardLayout Layout => _layout;
    [SerializeField]private BoardLayout _layout;


    public GameObject[] GameTiles => _gameTiles;
    private GameObject[] _gameTiles;

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

        if (_boardRootTransform == null)
            _boardRootTransform = transform;
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

    /////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public void CreateGameBoard()
    {
        CaluclateBoardDimentions();
        SpawnTiles();
    }

    private void SpawnTiles()
    {
        int numRows = (int)Layout.Rows;
        int numCols = (int)Layout.Columns;
        _gameTiles = new GameObject[numCols * numRows];

        for(int i = 0; i< numRows; i++)
        {
            for(int j = 0; j < numCols; j++)
            {
                //Create the object
                GameObject obj = Instantiate(TilePrefab);
                obj.transform.position = GetTileSpawnPosition(i, j);
                obj.transform.SetParent(BoardRoot, false);
                _gameTiles[(i * numCols) + j] = obj;

                //Make sure prefab has Tile component
                Tile tile = obj.GetComponent<Tile>();
                if(!tile)
                {
                    Debug.Log("Tile Prefab does not contain a Tile component on root. Could not init it's data");
                    return;
                }
                //Initialize the TileData
                int index = ((i * numCols) + j);
                tile.Init((uint)index, (uint)i, (uint)j, Layout.RowData[index]);
            }
        }
    }

    private void ClearTiles()
    {
        int count = GameTiles.Length;
        for (int i = 0; i < count; i++)
        {
            if (Application.isEditor)
                DestroyImmediate(_gameTiles[i].gameObject);
            else
                Destroy(_gameTiles[i].gameObject);
        }
        _gameTiles = new GameObject[0];
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Board Creation Methods

    private float totalWidth = 0;
    private float totalHeight = 0;
    private float widthOffset = 0;
    private float heightOffset = 0;

    Vector3 cornerstonePos;

    private void CaluclateBoardDimentions()
    {
        Tile tile = TilePrefab.GetComponent<Tile>();
        if(tile == null)
        {
            Debug.Log("Cannot build gameboard. Cannot Find Tile in tile prefab");
            return;
        }

        widthOffset = (tile.Width + (2 * tile.HorizontalMargin));
        heightOffset = (tile.Height + (2 * tile.VerticalMargin));

        totalHeight = Layout.Rows * heightOffset;
        totalWidth = Layout.Columns *widthOffset;

        cornerstonePos.x = -((totalWidth / 2f) - (widthOffset / 2f));
        cornerstonePos.y = BoardRoot.position.y;
        cornerstonePos.z = ((totalHeight / 2f) - (heightOffset / 2f));
    }

    private Vector3 GetTileSpawnPosition(int index)
    {
        int[] coords = IndexToCoords(index);
        return GetTileSpawnPosition(coords[0], coords[1]);
    }
    private Vector3 GetTileSpawnPosition(int xi, int yi)
    {
        Vector3 pos = new Vector3();
        pos.y = BoardRoot.position.y;

        pos.x = cornerstonePos.x + (xi * widthOffset);
        pos.z = cornerstonePos.z - (yi * heightOffset);

        return pos;
    }
    
    private int[] IndexToCoords(int index)
    {
        int[] coords = new int[2];
        coords[1] = (int)Mathf.Floor(index / Layout.Columns);
        coords[2] = (int)(index - (coords[1] * Layout.Columns));

        return coords;
    }


    #endregion

    /////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Editor Preveiw
    private bool isPreveiwed = false;
    public void Preveiw()
    {
        if (Application.isPlaying || UnityEditor.EditorApplication.isPlaying == true)
            return;

        if (isPreveiwed) return;
        isPreveiwed = true;

        CreateGameBoard();
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
