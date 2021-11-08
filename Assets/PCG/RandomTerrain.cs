using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Terrain))]

public class RandomTerrain : MonoBehaviour {

	public bool makeItFlat = false;

	void Start () {
		Terrain t = GetComponent<Terrain> ();
		TerrainData td = t.terrainData;
		int x = td.heightmapResolution;
		int y = td.heightmapResolution;
		float [,] h = new float[y, x];

		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				h [j, i] = makeItFlat ? 0f : Random.Range (0f, 1f);
			}
		}

		td.SetHeights (0, 0, h);
	}
	
}



