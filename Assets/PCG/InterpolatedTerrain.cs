using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate float[,] Interpolator (int x, int y, int subSampling);

[RequireComponent (typeof (Terrain))]

public class InterpolatedTerrain : MonoBehaviour {

	public enum Interpolations { Flat, NoInterpolation, BilinearInterpolation, BicubicInterpolation };
	public Interpolator [] myInterpolators = { Flat, NoInterpolation, BilinearInterpolator, BicubicInterpolator };

	public Interpolations interpolator = Interpolations.Flat;
	public int subsampling = 10;

	void Start () {
		Terrain t = GetComponent<Terrain> ();
		TerrainData td = t.terrainData;
		int x = td.heightmapResolution;
		int y = td.heightmapResolution;

		td.SetHeights (0, 0, myInterpolators [(int)interpolator] (x, y, subsampling));
	}

	private static float [,] Flat (int x, int y, int subsampling) {
		return new float [y, x];
	}

	private static float [,] NoInterpolation (int x, int y, int subsampling) {
		float [,] h = new float [y, x];
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				h [j, i] = Random.Range (0f, 1f);
			}
		}
		return h;
	}

	private static float [,] BilinearInterpolator (int x, int y, int subsampling) {
		float [,] h = new float [y, x];

		// build a lattice
		for (int i = 0; i < x; i += subsampling) {
			for (int j = 0; j < y; j += subsampling) {
				h [j, i] = Random.Range (0f, 1f);
			}
		}

		// cut out border effects to make code simpler
		int xCut = subsampling * ((int)Mathf.Floor (x / subsampling));
		int yCut = subsampling * ((int)Mathf.Floor (y / subsampling));

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

		return h;
	}

	private static float [,] BicubicInterpolator (int x, int y, int subsampling) {
		float [,] h = new float [y, x];

		// build a lattice
		for (int i = 0; i < x; i += subsampling) {
			for (int j = 0; j < y; j += subsampling) {
				h [j, i] = Random.Range (0f, 1f);
			}
		}

		// cut out border effects to make code simpler
		int xCut = subsampling * ((int)Mathf.Floor (x / subsampling));
		int yCut = subsampling * ((int)Mathf.Floor (y / subsampling));

		// build bilinear interpolations
		// first direction i only on lattice points
		for (int i = 0; i < xCut; i += subsampling) { // avoid border effect
			for (int j = 0; j <= yCut; j += subsampling) {
				for (int k = 1; k < subsampling; k += 1) {
					h [j, i + k] = Mathf.Lerp (h [j, i], h [j, i + subsampling], slope((float)k / (float)subsampling)); // change here!
				}
			}
		}
		// then direction j on all points to cover all grid
		for (int i = 0; i <= xCut; i += 1) {
			for (int j = 0; j < yCut; j += subsampling) {
				for (int k = 1; k < subsampling; k += 1) {
					h [j + k, i] = Mathf.Lerp (h [j, i], h [j + subsampling, i], slope((float)k / (float)subsampling)); // change here!
				}
			}
		}

		return h;
	}

	private static float slope (float x) {
		return -2f * Mathf.Pow (x, 3) + 3f * Mathf.Pow (x, 2);
	}
	
}
