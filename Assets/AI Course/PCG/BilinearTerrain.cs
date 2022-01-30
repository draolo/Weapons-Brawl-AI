using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Terrain))]

public class BilinearTerrain : MonoBehaviour {

	public bool makeItFlat = false;
	public int subsampling = 10;

	void Start () {
		Terrain t = GetComponent<Terrain> ();
		TerrainData td = t.terrainData;
		int x = td.heightmapResolution;
		int y = td.heightmapResolution;

		if (makeItFlat) {
			td.SetHeights (0, 0, new float [y, x]); // default is 0f
			return;
		}

		float [,] h = new float [y, x];

		// build a lattice
		for (int i = 0; i < x; i += subsampling) {
			for (int j = 0; j < y; j += subsampling) {
				h [j, i] = Random.Range (0f, 1f);
			}
		}

		// cut out border effects to make code simpler
		int xCut = subsampling * ((int) Mathf.Floor (x / subsampling));
		int yCut = subsampling * ((int) Mathf.Floor (y / subsampling));

		// build bilinear interpolations
		// first direction i only on lattice points
		for (int i = 0; i < xCut; i += subsampling) { // avoid border effect
			for (int j = 0; j <= yCut; j += subsampling) {
				for (int k = 1; k < subsampling; k += 1) {
					h [j, i + k] = Mathf.Lerp (h [j, i], h [j, i + subsampling], (float)k / (float)subsampling);
				}
			}
		}
		// then direction j on all points to cover all grid
		for (int i = 0; i <= xCut; i += 1) {
			for (int j = 0; j < yCut; j += subsampling) {
				for (int k = 1; k < subsampling; k += 1) {
					h [j + k, i] = Mathf.Lerp (h [j, i], h [j + subsampling, i], (float)k / (float)subsampling);
				}
			}
		}

		td.SetHeights (0, 0, h);
	}
	
}
