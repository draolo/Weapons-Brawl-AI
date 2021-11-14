using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour {

    public Tilemap map;

    void Start () {
        map = GetComponent<Tilemap>();
    }


    public void CmdDestroyTile(int x, int y, int z)
    {
        Vector3Int pos = new Vector3Int(x, y, z);
        map.SetTile(pos, null);
        RpcDestroyTile( x,  y,  z);
    }


    public void RpcDestroyTile(int x, int y, int z)
    {
        Vector3Int pos = new Vector3Int(x, y, z);
        map.SetTile(pos, null);
    }

}
