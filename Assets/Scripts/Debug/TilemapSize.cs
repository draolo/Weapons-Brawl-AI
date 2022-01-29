using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSize : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        Tilemap t= GetComponent<Tilemap>();
        Debug.Log(t.size);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
