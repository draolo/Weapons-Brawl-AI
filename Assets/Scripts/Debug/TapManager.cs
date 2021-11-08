using UnityEngine;

public class TapManager : MonoBehaviour
{
    public Bot player;
    public Map map;

    int lastMouseTileX = -1;
    int lastMouseTileY = -1;

    void Awake()
    {
        player.mMap = map;
        Debug.Log("setted");
        player.BotInit();
    }


    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Mouse0))
            lastMouseTileX = lastMouseTileY = -1;
        var mousePosInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int mouseTileX, mouseTileY;
        map.GetMapTileAtPoint(mousePosInWorld, out mouseTileX, out mouseTileY);





        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("mouse: " + mousePosInWorld);
            Debug.Log("go to: " + mouseTileX + "  " + mouseTileY);
            player.GoToPosition(new Vector2i(mouseTileX, mouseTileY));
        }

        if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse2))
        {
            if (mouseTileX != lastMouseTileX || mouseTileY != lastMouseTileY || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse2))
            {
                if (!map.IsNotEmpty(mouseTileX, mouseTileY))
                    map.SetTile(mouseTileX, mouseTileY, Input.GetKey(KeyCode.Mouse1) ? TileType.Block : TileType.OneWay);
                else
                    map.SetTile(mouseTileX, mouseTileY, TileType.Empty);

                lastMouseTileX = mouseTileX;
                lastMouseTileY = mouseTileY;

                Debug.Log(mouseTileX + "  " + mouseTileY);
            }
        }
    }

}

