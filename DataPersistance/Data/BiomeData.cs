using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BiomeData : UpdatableData
{
    public BiomeType[] biomes;
	public GameObject ExtractableOreIndicator;
	public int ExtractableOreTilesCount;

    [Range(0.1f,0.45f)]
	public float BiomeFalloffHeightMin;
	[Range(0.6f, 0.9f)]
	public float BiomeFalloffHeightMax;
}
[System.Serializable]
public struct BiomeType
{
	public string name;
	public float minHeight;
	public float maxHeight;
	public int placementPriority;
	public GameObject Indicator;
	public NoiseData BiomeNoiseData;
	
	[Header("BiomeSpawnObjects")]
	public BiomeObjects[] objects;
}

[System.Serializable]
public struct BiomeObjects
{
	public GameObject Object;
	public float RarityMin;
	public float RarityMax;
	public NoiseData NoiseData;

	public int TotalSpawnAttempts;
}
