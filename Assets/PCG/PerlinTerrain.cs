using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Terrain))]

public class PerlinTerrain : MonoBehaviour {

	public int subsampling = 50;
	public bool makeItFlat = false;

	void Start () {
		Terrain t = GetComponent<Terrain> ();
		TerrainData td = t.terrainData;

		int x = td.heightmapResolution;
		int y = td.heightmapResolution;

		if (makeItFlat) {
			td.SetHeights (0, 0, new float [y, x]);
			return;
		}

		int xCut = subsampling * ((int)Mathf.Floor (x / subsampling));
		int yCut = subsampling * ((int)Mathf.Floor (y / subsampling));

		float [,] h = new float [y, x];
		Vector2 [,] slopes = new Vector2 [y, x];

		// first, set up the slopes and height at lattice points
		for (int i = 0; i <= xCut; i += subsampling) {
			for (int j = 0; j <= yCut; j += subsampling) {
				slopes [j, i] = Random.insideUnitCircle;
				h [j, i] = 0;
			}
		}

		// now let's start with the good stuff
		// THIS CODE IS SUB-OPTIMAL!
		for (int i = 0; i < xCut; i += 1) {
			for (int j = 0; j < yCut; j += 1) {

				// find the neighbouring lattice points
				int floorI = subsampling * ((int) Mathf.Floor ((float) i / subsampling));
				int floorJ = subsampling * ((int) Mathf.Floor ((float) j / subsampling));
				int ceilI  = subsampling * ((int) Mathf.Ceil  ((float) i / subsampling));
				int ceilJ  = subsampling * ((int) Mathf.Ceil  ((float) j / subsampling));

				// calculate the four contribution to height
				float h1 = Vector2.Dot (slopes [floorJ, floorI], new Vector2 (i, j) - new Vector2(floorI, floorJ));
				float h2 = Vector2.Dot (slopes [floorJ, ceilI], new Vector2 (i, j) - new Vector2 (ceilI, floorJ));
				float h3 = Vector2.Dot (slopes [ceilJ, floorI], new Vector2 (i, j) - new Vector2 (floorI, ceilJ));
				float h4 = Vector2.Dot (slopes [ceilJ, ceilI], new Vector2 (i, j) - new Vector2 (ceilI, ceilJ));

				// calculate relative position inside the square
				float u = ((float) i - floorI) / subsampling;
				float v = ((float) j - floorJ) / subsampling;

				// interpolate by lerping first horizontally (for each couple) and then vertically
				float l1 = Mathf.Lerp (h1, h2, slope(u));
				float l2 = Mathf.Lerp (h3, h4, slope(u));
				float finalH = Mathf.Lerp (l1, l2, slope(v));

				h [j, i] = finalH;
			}
		}

		td.SetHeights (0, 0, Normalize(h, x, y));
	}

	private static float slope (float x) {
		return -2f * Mathf.Pow (x, 3) + 3f * Mathf.Pow (x, 2);
	}

	private float [,] Normalize (float [,] m, int x, int y) {
		float max, min;
		max = float.MinValue;
		min = float.MaxValue;
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				if (m [j, i] < min) min = m [j, i];
				if (m [j, i] > max) max = m [j, i];
			}
		}
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				m [j, i] = (m [j, i] - min) / (max - min);
			}
		}
		return m;
	}

}
