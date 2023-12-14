using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdatableData, IDataPersistance
{
	public float noiseScale;

	public int octaves;
	[Range(0, 1)]
	public float persistance;
    [Range(0,3)]
	public float persistanceMultiplierMountains;

	public float lacunarity;

	public int seed;

	public Vector2 offset;

    void OnValidate()
	{
		if (lacunarity < 1)
		{
			lacunarity = 1;
		}
		if (octaves < 1)
		{
			octaves = 1;
		}
	}

	public void LoadData(GameData data)
	{
		//this.seed = data.WorldSeed;
	}

	public void SaveData(GameData data)
	{
		//data.WorldSeed = this.seed;
	}
}

