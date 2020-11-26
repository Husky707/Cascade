using System.Collections.Generic;
using UnityEngine;

public class PlacementHandler
{
    GameData Data;
    IGameRule Rules;
    GameBoard Board;
    private bool handleDataOnly = true;
    private bool isInit = false;

    #region Init
    public void UpdateHandler(GameData data)
    {        Data = data;    }

    public PlacementHandler(GameData data, IGameRule rules, GameBoard board = null)
    {
        Data = data;
        Rules = rules;
        Board = board;
        if (board != null)
            handleDataOnly = false;
        else 
            handleDataOnly = true;

        isInit = true;
    }
    #endregion


    public void HandlePlacementRequest(GameData data, uint targetTileIndex)
    {
        if(!isInit) { Debug.Log("Must Init Placement Handler."); return; }
        UpdateHandler(data);

        TileDataSet[] TargetArea = GetTargetArea(targetTileIndex);

        if (!CanPlaceAbility(TargetArea))
        {
            Debug.Log("handle denied placement");
            return;
        }

        CascadeEventData CascadeData = new CascadeEventData();
        List<EffectedTileData> EffectedTiles;
        EffectedTiles = GatherCascadeData(TargetArea);
        CascadeData = GetTotalCascade(EffectedTiles);
        EffectedTiles = ApplyCascadeData(EffectedTiles, CascadeData);

        ApplyCascadeToData(EffectedTiles);
        ApplyCascadeToBoard(EffectedTiles);

    }

    public void HandlePlacementCommand(GameData data, uint targetTileIndex)
    {
        /*
        UpdateHandler(data);

        if (!CanPlaceAbility(targetTileIndex))
        {
            Debug.Log("Data may be out of sync. Command placement denied");
            return;
        }
        */
        HandlePlacementRequest(data, targetTileIndex);
    }

    #region Apply Cascade
    private void ApplyCascadeToData(List<EffectedTileData> effectList)
    {
        Debug.Log("Player changing their data");
        foreach(EffectedTileData eachTile in effectList)
        {
            Data.AlterBoardTile(eachTile.newData.index, eachTile.newData);
        }

    }

    private void ApplyCascadeToBoard(List<EffectedTileData> effectList)
    {
        if (handleDataOnly || Board == null)
            return;

        Debug.Log("TODO: Apply data changes to the gameboard");
    }

    #endregion

    #region Validate Placement
    private bool CanPlaceDie(TileDataSet targetArea)
    {
        return Rules.ValidateCanPlaceDie(targetArea);
    }
    private bool CanPlaceDie(uint targetIndex)
    {
        TileDataSet targetArea = new TileDataSet(Data, Board, targetIndex);
        return CanPlaceDie(targetArea);
    }

    private bool CanPlaceAbility(uint targetIndex)
    {
        TileDataSet[] abilityArea = GetTargetArea(targetIndex);
        return CanPlaceAbility(abilityArea);
    }
    private bool CanPlaceAbility(TileDataSet[] abilityArea)
    {
        if (Rules.Settings.MustPlaceEntireAbility && !CanPlaceAllInAbility(abilityArea))
            return false;

        return Rules.ValidateCanPlaceAbility(abilityArea);
    }

    private bool CanPlaceAllInAbility(TileDataSet[] targetArea)
    {
        foreach(TileDataSet target in targetArea)
        {
            if (!CanPlaceDie(target))
                return false;
        }
        return true;
    }
    #endregion

    #region Gather Cascade Data
    private List<EffectedTileData> GatherCascadeData(TileDataSet[] targetArea)
    {
        int areaLength = targetArea.Length;
        List<TileDataSet> effectedAreas = new List<TileDataSet>(areaLength);
        List<EffectedTileData> effectedTiles = new List<EffectedTileData>(areaLength * 9);
        HashSet<uint> noDupes = new HashSet<uint>();

        //Romove tiles that could not be placed on
        for(int i = 0; i < areaLength; i++)
        {
            if(targetArea[i].isNull == true || !Rules.ValidateCanPlaceDie(targetArea[i]))
                continue;

            effectedAreas.Add(targetArea[i]);
        }

        //GetEffects die effects
        for(int i = 0; i < effectedAreas.Count; i++)
        {
            //Placed die
            TileDataSet curSet = targetArea[i];
            EffectedTileData placedDieEffect = Rules.GetPlacedDieEffect(curSet.TargetTile);
            effectedTiles.Add(placedDieEffect);
            noDupes.Add(placedDieEffect.newData.index);

            TileData actorTile = placedDieEffect.newData;
            //Start at 1. O index is placed die  (already resolved)
            for (int j = 1; j < 9; j++)
            {
                if (curSet.isNull == true || curSet.boardIndex[j] == -1)
                    continue;

                EffectedTileData newData = Rules.GetEffectOnTile(actorTile, curSet.tileGroup[j]);
                if (newData.EffectToResolve == eDiceActions.Noone)
                    continue;

                //Remove already effected tiles
                if (noDupes.Contains(newData.newData.index))
                    continue;

                effectedTiles.Add(newData);
                noDupes.Add(newData.newData.index);
            }
        }

        return effectedTiles;
    }

    private CascadeEventData GetTotalCascade(List<EffectedTileData> effectData)
    {
        CascadeEventData cascadeData = new CascadeEventData();
        //Get total value
        foreach(EffectedTileData each in effectData )
            cascadeData.IncreaseCascade(each);

        return cascadeData;
    }

    private List<EffectedTileData> ApplyCascadeData(List<EffectedTileData> effected, CascadeEventData cascadeData)
    {
        List<EffectedTileData> effectData = effected;
        //Update each effectted member
        int len = effectData.Count;
        for (int j = 0; j < len; j++)
        {
            EffectedTileData data = effectData[j];
            data.EffectIntinsity = cascadeData.Intensity;
            effectData[j] = data;
        }

        return effectData;
    }

    #endregion

    #region Helpers

    /// ////////////////////////////////////////////////////////////////////////////
    #region Target areas
    private TileDataSet[] GetTargetArea(uint anchorTarget)
    {
        if (anchorTarget >= Data.BoardState.Length)
            return null;

        eDicePlacers curPlacer = Data.CurrentPlacers[Data.CurrentTurn];
        ePlacerOrientation curDir = Data.CurrentPlacerOrientation[Data.CurrentTurn];
        TileDataSet[] returnSet = new TileDataSet[1];
        if (curPlacer == eDicePlacers.Single)
        {
             returnSet[0] = new TileDataSet(Data, Board, anchorTarget);
            return returnSet;
        }
        switch (curPlacer)
        {
            case eDicePlacers.Corner:
                return GetCornerArea(anchorTarget, curDir);
            case eDicePlacers.Diagonal:
                return GetDiagonalArea(anchorTarget, curDir);
            case eDicePlacers.Line:
                return GetLineArea(anchorTarget, curDir);
        }

        return null;
    }

    private TileDataSet[] GetCornerArea(uint anchor, ePlacerOrientation orientation)
    {
        int[] anchorCoords = IndexToCoords(anchor);
        int anchorY = anchorCoords[1];
        int anchorX = anchorCoords[0];

        TileDataSet[] tileSet = new TileDataSet[3];
        tileSet[0] = new TileDataSet(Data, Board, anchor);

        switch(orientation)
        {
            case ePlacerOrientation.Quad1:
                {
                    //Top
                    if (IsWithinBounds(anchorX, anchorY - 1))
                        tileSet[1] = new TileDataSet(Data, Board, CoordsToindex(anchorX, anchorY - 1));
                    else
                        tileSet[1] = new TileDataSet(true);
                    //Right
                    if (IsWithinBounds(anchorX + 1, anchorY))
                        tileSet[2] = new TileDataSet(Data, Board, CoordsToindex(anchorX + 1, anchorY));
                    else
                        tileSet[2] = new TileDataSet(true);

                    return tileSet;
                }
            case ePlacerOrientation.Quad2:
            {
                    //Top
                    if (IsWithinBounds(anchorX, anchorY - 1))
                        tileSet[1] = new TileDataSet(Data, Board, CoordsToindex(anchorX, anchorY - 1));
                    else
                        tileSet[1] = new TileDataSet(true);
                    //Left
                    if (IsWithinBounds(anchorX - 1, anchorY))
                        tileSet[2] = new TileDataSet(Data, Board, CoordsToindex(anchorX - 1, anchorY));
                    else
                        tileSet[2] = new TileDataSet(true);
                    return tileSet;
            }
            case ePlacerOrientation.Quad3:
                {
                    //Bottom
                    if (IsWithinBounds(anchorX, anchorY + 1))
                        tileSet[1] = new TileDataSet(Data, Board, CoordsToindex(anchorX, anchorY + 1));
                    else
                        tileSet[1] = new TileDataSet(true);
                    //Left
                    if (IsWithinBounds(anchorX - 1, anchorY))
                        tileSet[2] = new TileDataSet(Data, Board, CoordsToindex(anchorX - 1, anchorY));
                    else
                        tileSet[2] = new TileDataSet(true);
                    return tileSet;
                }
            case ePlacerOrientation.Quad4:
                {
                    //Bottom
                    if (IsWithinBounds(anchorX, anchorY + 1))
                        tileSet[1] = new TileDataSet(Data, Board, CoordsToindex(anchorX, anchorY + 1));
                    else
                        tileSet[1] = new TileDataSet(true);
                    //Right
                    if (IsWithinBounds(anchorX + 1, anchorY))
                        tileSet[2] = new TileDataSet(Data, Board, CoordsToindex(anchorX + 1, anchorY));
                    else
                        tileSet[2] = new TileDataSet(true);
                    return tileSet;
                }
        }
        return null;
    }

    private TileDataSet[] GetLineArea(uint anchor, ePlacerOrientation orientation)
    {
        int[] anchorCoords = IndexToCoords(anchor);
        int anchorY = anchorCoords[1];
        int anchorX = anchorCoords[0];

        TileDataSet[] tileSet = new TileDataSet[4];
        int xMod = 1; int yMod = -1;
        switch(orientation)
        {
            case ePlacerOrientation.Quad2:
                xMod = -1;
                break;
            case ePlacerOrientation.Quad3:
                xMod = -1;
                yMod = 1;
                break;
            case ePlacerOrientation.Quad4:
                yMod = 1;
                break;
        }//end switch

        for(int i = 0; i < 4; i++)
        {
            if(IsWithinBounds(anchorX + (xMod * i), anchorY + (yMod * i)))
                tileSet[i] = new TileDataSet(Data, Board, CoordsToindex(anchorX + (xMod * i), anchorY + (yMod * i)));
            else
                tileSet[i] = new TileDataSet(true);
        }

        return tileSet;
    }

    private TileDataSet[] GetDiagonalArea(uint anchor, ePlacerOrientation orientation)
    {
        int[] anchorCoords = IndexToCoords(anchor);
        int anchorY = anchorCoords[1];
        int anchorX = anchorCoords[0];

        TileDataSet[] tileSet = new TileDataSet[4];
        int xMod = 1; int yMod = 0;
        switch (orientation)
        {
            case ePlacerOrientation.Quad2:
                xMod = 0;
                yMod = -1;
                break;
            case ePlacerOrientation.Quad3:
                xMod = -1;
                break;
            case ePlacerOrientation.Quad4:
                yMod = 1;
                xMod = 0;
                break;
        }//end switch

        for (int i = 0; i < 4; i++)
        {
            if (IsWithinBounds(anchorX + (xMod * i), anchorY + (yMod * i)))
                tileSet[i] = new TileDataSet(Data, Board, CoordsToindex(anchorX + (xMod * i), anchorY + (yMod * i)));
            else
                tileSet[i] = new TileDataSet(true);
        }

        return tileSet;
    }
    #endregion
    ////////////////////////////////////////////////////////////////////////////
    ///
    private bool IsWithinBounds(uint indexPos)
    {
        int[] vec = IndexToCoords(indexPos);
        return IsWithinBounds(vec[0], vec[1]); }
    private bool IsWithinBounds(int xx, int yy)
    {
        int boundsX = (int)Board.Layout.Columns;
        int boundsY = (int)Board.Layout.Rows;
        if (xx >= boundsX || xx < 0 || yy < 0 || yy >= boundsY)
            return false;

        return true;
    }


    private int[] IndexToCoords(uint indexPos)
    {
        int yy = Mathf.FloorToInt(indexPos / Board.Layout.Columns);
        int xx = (int)indexPos - (yy * (int)Board.Layout.Columns);
        int[] vector = { xx, yy };
        return vector;
    }

    private uint CoordsToindex(int xx, int yy)
    {
        return (uint)((yy * (int)Board.Layout.Columns) + xx);
    }

    #endregion

}
public struct TileDataSet
    {
        public int[] boardIndex;
        public TileData[] tileGroup;

        public uint TargetIndex;
        public TileData TargetTile;

        public bool isNull;
        public TileDataSet(bool isnull)
        {
            isNull = true;
            boardIndex = null;
            tileGroup = null;
            TargetIndex = default;
            TargetTile = default;
        }

    public TileDataSet(GameData data, GameBoard board, uint targetIndex, bool isnull = false)
        {
            TargetIndex = targetIndex;
            TargetTile = data.BoardState[targetIndex];
            isNull = isnull;

            #region Get Local Tiles
            int col = (int)board.Layout.Columns;
            int rows = (int)board.Layout.Rows;
            int targetY, targetX;
            targetY = Mathf.FloorToInt(targetIndex / col);
            targetX = (int)targetIndex - (targetY * col);

            boardIndex = new int[9];
            tileGroup = new TileData[9];
            for(int i = 8 ; i >= 0; i--)
            {         boardIndex[i] = -1;            }
            int rowI = targetY - 1;
            int rowX = targetX - 1;
            for(int count = 0; count < 3; count++ , rowI++)
            {
                rowX = targetX - 1;
                for(int colCount = 0; colCount < 3; colCount++, rowX++)
                {
                    if (rowI < 0 || rowI >= rows || rowX < 0 || rowX >= col)
                    {
                        boardIndex[(3*count) + colCount] = -1;
                        tileGroup[(3*count) + colCount] = default;
                    }
                    else
                    {
                        boardIndex[(3*count) + colCount] = (rowI * col) + rowX;
                        tileGroup[(3 * count) + colCount] = (data.BoardState[(rowI * col) + rowX]);
                    }
                }

            }
            #endregion
        }

    }

public struct EffectedTileData
{
    public TileData oldData;
    public TileData newData;

    public int cascadeValueAdded;
    public int cascadeCountIncreased;

    //Generally for visual effect
    public eDiceActions EffectToResolve;
    public eCascadeIntensity EffectIntinsity;
}
