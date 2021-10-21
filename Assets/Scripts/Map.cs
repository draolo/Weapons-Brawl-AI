using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Algorithms;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

[System.Serializable]
public enum TileType
{
    Empty,
    Block,
    OneWay
}

[System.Serializable]
public partial class Map : MonoBehaviour 
{
	
	/// <summary>
	/// The map's position in world space. Bottom left corner.
	/// </summary>
	public Vector3 position;
	
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
	static public int cTileSize = 1;
	
	/// <summary>
	/// The width of the map in tiles.
	/// </summary>
	public int mWidth = 367;
	/// <summary>
	/// The height of the map in tiles.
	/// </summary>
	public int mHeight = 77;

    //public MapRoomData mapRoomSimple;
    //public MapRoomData mapRoomOneWay;

    //public Camera gameCamera;
    public Bot player;
    bool[] inputs;
    bool[] prevInputs;

    int lastMouseTileX = -1;
    int lastMouseTileY = -1;

    public KeyCode goLeftKey = KeyCode.A;
    public KeyCode goRightKey = KeyCode.D;
    public KeyCode goJumpKey = KeyCode.Space;
    public KeyCode goDownKey = KeyCode.S;

    //public RectTransform sliderHigh;
    //public RectTransform sliderLow;

    public Tilemap tilemap;
    private Vector3Int tilemapOffset;

	public TileType GetTile(int x, int y) 
	{
        if (x < 0 || x >= mWidth
            || y < 0 || y >= mHeight)
            return TileType.Block;

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
		
		mPathFinder.Formula                 = HeuristicFormula.Manhattan;
		//if false then diagonal movement will be prohibited
        mPathFinder.Diagonals               = false;
		//if true then diagonal movement will have higher cost
        mPathFinder.HeavyDiagonals          = false;
		//estimate of path length
        mPathFinder.HeuristicEstimate       = 6;
        mPathFinder.PunishChangeDirection   = false;
        mPathFinder.TieBreaker              = false;
        mPathFinder.SearchLimit             = 1000000;
        mPathFinder.DebugProgress           = false;
        mPathFinder.DebugFoundPath          = false;
	}
	
	public void GetMapTileAtPoint(Vector2 point, out int tileIndexX, out int tileIndexY)
	{
        Vector3Int tileIndex = tilemap.WorldToCell(point);
        tileIndex-=tilemapOffset;
        tileIndexY = tileIndex.y;
        tileIndexX = tileIndex.x;
	}
	
	public Vector2i GetMapTileAtPoint(Vector2 point)
	{
        Vector3Int tileIndex = tilemap.WorldToCell(point);
        tileIndex -= tilemapOffset;
        return new Vector2i(tileIndex.x,tileIndex.y);
    }
	
	public Vector2 GetMapTilePosition(int tileIndexX, int tileIndexY)
	{
        Vector3Int tileIndex = new Vector3Int(tileIndexX, tileIndexY,0);
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
		var tilePos = GetMapTilePosition (tileIndexX, tileIndexY);
		
		return aabb.Overlaps(tilePos, new Vector2( (float)(cTileSize)/2.0f, (float)(cTileSize)/2.0f));
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
        /*else if (type == TileType.OneWay) //no oneway platform
        {
            mGrid[x, y] = 1;
            tilesSprites[x, y].enabled = true;

            tilesSprites[x, y].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            tilesSprites[x, y].transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            tilesSprites[x, y].sprite = mDirtSprites[25];
        }*/
        else
        {
            mGrid[x, y] = 1;
            Vector3Int tileposition = new Vector3Int(x, y, 0) + tilemapOffset;
            tilemap.SetTile(tileposition, null);
        }

    }

    public void Start()
    {

        //var mapRoom = mapRoomOneWay;
        mRandomNumber = new System.Random();

        Application.targetFrameRate = 60;
        
        inputs = new bool[(int)KeyInput.Count];
        prevInputs = new bool[(int)KeyInput.Count];

        //set the position
        position = transform.position;

        mHeight = tilemap.cellBounds.size.y;
        mWidth= tilemap.cellBounds.size.x;
        
        tiles = new TileType[mWidth, mHeight];
        tilesSprites = new TileBase[mWidth, mHeight];
        TileBase[] actualTiles=tilemap.GetTilesBlock(tilemap.cellBounds);
        Debug.Log("firstTile " +(actualTiles[367]==null));
        tilemapOffset = tilemap.cellBounds.min;
        Debug.Log(tilemap.cellBounds);
        Debug.Log(mWidth);
        Debug.Log(mHeight);
        Debug.Log(actualTiles.Length);
        mGrid = new byte[Mathf.NextPowerOfTwo((int)mWidth), Mathf.NextPowerOfTwo((int)mHeight)];
        InitPathFinder();

        for (int y = 0; y < mHeight; ++y)
        {
            for (int x=0;x<mWidth;++x)
            {
                int index = y * mWidth + x;
                tilesSprites[x,y]=actualTiles[index];
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

        Debug.Log("tile: "+tiles[0,1]);


        for (int y = 0; y < mHeight; ++y)
        {
            tiles[1, y] = TileType.Block;
            tiles[mWidth - 2, y] = TileType.Block;
        }

        for (int x = 0; x < mWidth; ++x)
        {
            tiles[x, 1] = TileType.Block;
            tiles[x, mHeight - 2] = TileType.Block;
        }

        /*for (int y = 2; y < mHeight - 2; ++y)
        {
            for (int x = 2; x < mWidth - 2; ++x)
            {
                if (y < mHeight/4)
                    SetTile(x, y, TileType.Block);
            }
        }*/

        player.BotInit(inputs, prevInputs);
        player.mMap = this;
        //player.mPosition = new Vector2(2 * Map.cTileSize, (mHeight / 2) * Map.cTileSize + player.mAABB.HalfSizeY);
    }

    void Update()
    {
        inputs[(int)KeyInput.GoRight] = Input.GetKey(goRightKey);
        inputs[(int)KeyInput.GoLeft] = Input.GetKey(goLeftKey);
        inputs[(int)KeyInput.GoDown] = Input.GetKey(goDownKey);
        inputs[(int)KeyInput.Jump] = Input.GetKey(goJumpKey);

        if (Input.GetKeyUp(KeyCode.Mouse0))
            lastMouseTileX = lastMouseTileY = -1;

        //Vector3 mousePos = Input.mousePosition;
        //Vector2 cameraPos = Camera.main.transform.position;
        var mousePosInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int mouseTileX, mouseTileY;
        GetMapTileAtPoint(mousePosInWorld, out mouseTileX, out mouseTileY);
       // Vector2 bottomLeft = (Vector2)sliderLow.position + sliderLow.rect.min;
        //Vector2 topRight = (Vector2)sliderHigh.position + sliderHigh.rect.max;

        if (Input.GetKeyDown(KeyCode.Tab))
            Debug.Break();

        //Debug.Log(mousePos + "   " + bottomLeft + "     " + topRight);

 
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("mouse: " + mousePosInWorld);
            Debug.Log("go to: "+mouseTileX + "  " + mouseTileY);
            player.TappedOnTile(new Vector2i(mouseTileX, mouseTileY));
        }

        if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse2))
        {
            if (mouseTileX != lastMouseTileX || mouseTileY != lastMouseTileY || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse2))
            {
                if (!IsNotEmpty(mouseTileX, mouseTileY))
                    SetTile(mouseTileX, mouseTileY, Input.GetKey(KeyCode.Mouse1) ? TileType.Block : TileType.OneWay);
                else
                    SetTile(mouseTileX, mouseTileY, TileType.Empty);

                lastMouseTileX = mouseTileX;
                lastMouseTileY = mouseTileY;

                Debug.Log(mouseTileX + "  " + mouseTileY);
            }
        }
    }

    System.Random mRandomNumber;
  
   /* 
    void AutoTile(TileType type, int x, int y, int rand4NeighbourTiles, int rand3NeighbourTiles,
        int rand2NeighbourPipeTiles, int rand2NeighbourCornerTiles, int rand1NeighbourTiles, int rand0NeighbourTiles)
    {
        if (x >= mWidth || x < 0 || y >= mHeight || y < 0)
            return;

        if (tiles[x, y] != TileType.Block)
            return;

        int tileOnLeft = tiles[x - 1, y] == tiles[x, y] ? 1 : 0;
        int tileOnRight = tiles[x + 1, y] == tiles[x, y] ? 1 : 0;
        int tileOnTop = tiles[x, y + 1] == tiles[x, y] ? 1 : 0;
        int tileOnBottom = tiles[x, y - 1] == tiles[x, y] ? 1 : 0;

        float scaleX = 1.0f;
        float scaleY = 1.0f;
        float rot = 0.0f;
        int id = 0;

        int sum = tileOnLeft + tileOnRight + tileOnTop + tileOnBottom;

        switch (sum)
        {
            case 0:
                id = 1 + mRandomNumber.Next(rand0NeighbourTiles);

                break;
            case 1:
                id = 1 + rand0NeighbourTiles + mRandomNumber.Next(rand1NeighbourTiles);

                if (tileOnRight == 1)
                    scaleX = -1;
                else if (tileOnTop == 1)
                    rot = -1;
                else if (tileOnBottom == 1)
                {
                    rot = 1;
                    scaleY = -1;
                }

                break;
            case 2:

                if (tileOnLeft + tileOnBottom == 2)
                {
                    id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + rand2NeighbourPipeTiles
                        + mRandomNumber.Next(rand2NeighbourCornerTiles);
                }
                else if (tileOnRight + tileOnBottom == 2)
                {
                    id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + rand2NeighbourPipeTiles
                        + mRandomNumber.Next(rand2NeighbourCornerTiles);
                    scaleX = -1;
                }
                else if (tileOnTop + tileOnLeft == 2)
                {
                    id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + rand2NeighbourPipeTiles
                        + mRandomNumber.Next(rand2NeighbourCornerTiles);
                    scaleY = -1;
                }
                else if (tileOnTop + tileOnRight == 2)
                {
                    id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + rand2NeighbourPipeTiles
                        + mRandomNumber.Next(rand2NeighbourCornerTiles);
                    scaleX = -1;
                    scaleY = -1;
                }
                else if (tileOnTop + tileOnBottom == 2)
                {
                    id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + mRandomNumber.Next(rand2NeighbourPipeTiles);
                    rot = 1;
                }
                else if (tileOnRight + tileOnLeft == 2)
                    id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + mRandomNumber.Next(rand2NeighbourPipeTiles);

                break;
            case 3:
                id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + rand2NeighbourPipeTiles
                    + rand2NeighbourCornerTiles + mRandomNumber.Next(rand3NeighbourTiles);

                if (tileOnLeft == 0)
                {
                    rot = 1;
                    scaleX = -1;
                }
                else if (tileOnRight == 0)
                {
                    rot = 1;
                    scaleY = -1;
                }
                else if (tileOnBottom == 0)
                    scaleY = -1;

                break;

            case 4:
                id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + rand2NeighbourPipeTiles
                    + rand2NeighbourCornerTiles + rand3NeighbourTiles + mRandomNumber.Next(rand4NeighbourTiles);

                break;
        }

        tilesSprites[x, y].transform.localScale = new Vector3(scaleX, scaleY, 1.0f);
        tilesSprites[x, y].transform.eulerAngles = new Vector3(0.0f, 0.0f, rot * 90.0f);
        tilesSprites[x, y].sprite = mDirtSprites[id - 1];
    }
    */

    void FixedUpdate()
    {
        player.BotUpdate();
    }
}
