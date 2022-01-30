using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Algorithms;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[System.Serializable]
public enum TileType
{
    Empty,
    Block,
    OneWay
}

[System.Serializable]
public class Map : MonoBehaviour
{
    public UnityEvent mapModificationEvent;

    /// <summary>
    /// The base tile sprite prefab that populates the map.
    /// Assigned in the inspector.
    /// </summary>
    public TileBase tilePrefab;

    /// <summary>
    /// The path finder.
    /// </summary>
    public PathFinderFast mPathFinder;

    /// <summary>
    /// The nodes that are fed to pathfinder.
    /// </summary>
    [HideInInspector]
    public byte[,] mGrid;

    /// <summary>
    /// The map's tile data.
    /// </summary>
    [HideInInspector]
    private TileType[,] tiles;

    /// <summary>
    /// The map's sprites.
    /// </summary>
    private TileBase[,] tilesSprites;

    /// <summary>
    /// The size of a tile in square.
    /// </summary>
    public static int cTileSize = 1;

    /// <summary>
    /// The width of the map in tiles.
    /// </summary>
    public int mWidth = 367;

    /// <summary>
    /// The height of the map in tiles.
    /// </summary>
    public int mHeight = 77;

    public Tilemap tilemap;
    private Vector3Int tilemapOffset;

    public TileType GetTile(int x, int y)
    {
        if (x < 0 || x >= mWidth || y < 0 || y >= mHeight)
        {
            return TileType.Block;
        }
        return tiles[x, y];
    }

    public bool IsOneWayPlatform(int x, int y)
    {
        if (x < 0 || x >= mWidth
            || y < 0 || y >= mHeight)
            return false;

        return (tiles[x, y] == TileType.OneWay);
    }

    public bool IsGround(int x, int y)
    {
        if (x < 0 || x >= mWidth
           || y < 0 || y >= mHeight)
            return false;

        return (tiles[x, y] == TileType.OneWay || tiles[x, y] == TileType.Block);
    }

    public bool IsObstacle(int x, int y)
    {
        if (x < 0 || x >= mWidth
            || y < 0 || y >= mHeight)
            return true;

        return (tiles[x, y] == TileType.Block);
    }

    public bool IsNotEmpty(int x, int y)
    {
        if (x < 0 || x >= mWidth
            || y < 0 || y >= mHeight)
            return true;

        return (tiles[x, y] != TileType.Empty);
    }

    public void InitPathFinder()
    {
        mPathFinder = new PathFinderFast(mGrid, this);

        mPathFinder.Formula = HeuristicFormula.Manhattan;
        //if false then diagonal movement will be prohibited
        mPathFinder.Diagonals = false;
        //if true then diagonal movement will have higher cost
        mPathFinder.HeavyDiagonals = false;
        //estimate of path length
        mPathFinder.HeuristicEstimate = 6;
        mPathFinder.PunishChangeDirection = false;
        mPathFinder.TieBreaker = false;
        mPathFinder.SearchLimit = 1000000;
        mPathFinder.DebugProgress = false;
        mPathFinder.DebugFoundPath = false;
    }

    public void GetMapTileAtPoint(Vector2 point, out int tileIndexX, out int tileIndexY)
    {
        Vector3Int tileIndex = tilemap.WorldToCell(point);
        tileIndex -= tilemapOffset;
        tileIndexY = tileIndex.y;
        tileIndexX = tileIndex.x;
    }

    public Vector2i GetMapTileAtPoint(Vector2 point)
    {
        Vector3Int tileIndex = tilemap.WorldToCell(point);
        tileIndex -= tilemapOffset;
        return new Vector2i(tileIndex.x, tileIndex.y);
    }

    public Vector2 GetMapTilePosition(int tileIndexX, int tileIndexY)
    {
        Vector3Int tileIndex = new Vector3Int(tileIndexX, tileIndexY, 0);
        tileIndex += tilemapOffset;
        return tilemap.GetCellCenterWorld(tileIndex);
    }

    public Vector2 GetMapTilePosition(Vector2i tileCoords)
    {
        Vector3Int tileIndex = new Vector3Int(tileCoords.x, tileCoords.y, 0);
        tileIndex += tilemapOffset;
        return tilemap.GetCellCenterWorld(tileIndex);
    }

    public bool CollidesWithMapTile(AABB aabb, int tileIndexX, int tileIndexY)
    {
        var tilePos = GetMapTilePosition(tileIndexX, tileIndexY);

        return aabb.Overlaps(tilePos, new Vector2((float)(cTileSize) / 2.0f, (float)(cTileSize) / 2.0f));
    }

    public bool AnySolidBlockInRectangle(Vector2 start, Vector2 end)
    {
        return AnySolidBlockInRectangle(GetMapTileAtPoint(start), GetMapTileAtPoint(end));
    }

    public bool AnySolidBlockInStripe(int x, int y0, int y1)
    {
        int startY, endY;

        if (y0 <= y1)
        {
            startY = y0;
            endY = y1;
        }
        else
        {
            startY = y1;
            endY = y0;
        }

        for (int y = startY; y <= endY; ++y)
        {
            if (GetTile(x, y) == TileType.Block)
                return true;
        }

        return false;
    }

    public bool AnySolidBlockInRectangle(Vector2i start, Vector2i end)
    {
        int startX, startY, endX, endY;

        if (start.x <= end.x)
        {
            startX = start.x;
            endX = end.x;
        }
        else
        {
            startX = end.x;
            endX = start.x;
        }

        if (start.y <= end.y)
        {
            startY = start.y;
            endY = end.y;
        }
        else
        {
            startY = end.y;
            endY = start.y;
        }

        for (int y = startY; y <= endY; ++y)
        {
            for (int x = startX; x <= endX; ++x)
            {
                if (GetTile(x, y) == TileType.Block)
                    return true;
            }
        }

        return false;
    }

    public void SetTile(int x, int y, TileType type)
    {
        if (x < 0 || x >= mWidth || y < 0 || y >= mHeight)
            return;

        tiles[x, y] = type;

        if (type == TileType.Block)
        {
            mGrid[x, y] = 0;
            Vector3Int tileposition = new Vector3Int(x, y, 0) + tilemapOffset;
            tilemap.SetTile(tileposition, tilePrefab);
        }
        else
        {
            mGrid[x, y] = 1;
            Vector3Int tileposition = new Vector3Int(x, y, 0) + tilemapOffset;
            tilemap.SetTile(tileposition, null);
        }
    }

    public void Awake()
    {
        Application.targetFrameRate = 60;
        if (mapModificationEvent == null)
        {
            mapModificationEvent = new UnityEvent();
        }
        mHeight = tilemap.cellBounds.size.y;
        mWidth = tilemap.cellBounds.size.x;

        tiles = new TileType[mWidth, mHeight];
        tilesSprites = new TileBase[mWidth, mHeight];
        TileBase[] actualTiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        tilemapOffset = tilemap.cellBounds.min;
        mGrid = new byte[Mathf.NextPowerOfTwo((int)mWidth), Mathf.NextPowerOfTwo((int)mHeight)];
        InitPathFinder();

        for (int y = 0; y < mHeight; ++y)
        {
            for (int x = 0; x < mWidth; ++x)
            {
                int index = y * mWidth + x;
                tilesSprites[x, y] = actualTiles[index];
                if (actualTiles[index] == null)
                {
                    tiles[x, y] = TileType.Empty;
                    mGrid[x, y] = 1;
                }
                else
                {
                    tiles[x, y] = TileType.Block;
                    mGrid[x, y] = 0;
                }
            }
        }
    }

    public void DestroyCircle(Vector3 position, int explosionRadius)
    {
        foreach (var p in new BoundsInt(-explosionRadius, -explosionRadius, 0, 2 * explosionRadius + 1, 2 * explosionRadius + 1, 1).allPositionsWithin)
        {
            int x = p[0];
            int y = p[1];
            if (x * x + y * y - explosionRadius * explosionRadius < 0)
            {
                position.z = 0; // A volte diventa -1 a caso quindi lo forzo a 0 io
                Vector3 destroyPos = position + p;
                int destroyX, destroyY;
                GetMapTileAtPoint(destroyPos, out destroyX, out destroyY);
                SetTile(destroyX, destroyY, TileType.Empty);
            }
        }
        mapModificationEvent.Invoke();
    }
}