using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour, IDataPersistance
{
	public BiomeData biomeData;
	public GameObject BiomeIndicatorsTab;

	private BiomeObjectsHandler objectGenerator;

	private float[,] biomeTextureMapClay;
	private float[,] biomeTextureMapForest;
	private float[,] biomeTextureMapOak;

	private bool generatebiomes = false;
	[SerializeField]private int biomeSeed;
	private int mapChunkSize;

	public void GenerateBiomes(float[,] noiseMap, int mapChunkSize)
	{
		generatebiomes = true;

		this.mapChunkSize = mapChunkSize;
		int totalWorldSize = mapChunkSize * 2 -1; //-1 because inner edges of world are shared

		biomeTextureMapClay = new float[totalWorldSize, totalWorldSize];
		biomeTextureMapForest = new float[totalWorldSize, totalWorldSize];
		biomeTextureMapOak = new float[totalWorldSize, totalWorldSize];

		objectGenerator = GetComponent<BiomeObjectsHandler>();

		ClearBiomeIndicators();

		for (int b = 0; b < biomeData.biomes.Length; b++) //loop trough all the biomes an make a biome noisemap for that biome
		{
			float[,] biomeMap = Noise.GenerateNoiseMap(totalWorldSize, totalWorldSize,
				biomeSeed,
				biomeData.biomes[b].BiomeNoiseData.noiseScale, 
				biomeData.biomes[b].BiomeNoiseData.octaves, 
				biomeData.biomes[b].BiomeNoiseData.persistance, 
				biomeData.biomes[b].BiomeNoiseData.persistanceMultiplierMountains, 
				biomeData.biomes[b].BiomeNoiseData.lacunarity, 
				biomeData.biomes[b].BiomeNoiseData.offset, 
				Noise.NormalizeMode.Local);


				GeneratebiomeIndicators(b, biomeMap, noiseMap, totalWorldSize); //generate biometiles which are used in the objectGenrator to determine the object generation
		}
		GenerateExtractableOreIndicators(noiseMap, totalWorldSize); //different kind of biome generation. this one is not based on noisemap but around heightmap height

		//check if all biomes are present
		for (int a = 0; a < biomeData.biomes.Length; a++)
		{
			Transform Child = null;
			Child = BiomeIndicatorsTab.transform.Find(biomeData.biomes[a].Indicator.name + "(Clone)"); //need to add (Clone) because it is a prefab, if biomeindicator not there it will remaine 0

			if (Child == null)//if biome not present
			{
				//regenerate with other seed
				biomeSeed += 1;
				GenerateBiomes(noiseMap,mapChunkSize);
				return;
			}
		}		
		transform.BroadcastMessage("GenerateMap");//generate the world after biomes are generated because mesh data needs biomedata for textureUVmap
		objectGenerator.GenerateBiomeObjects();//generate all the objects for the biome indicators
	}

	public void ClearBiomeIndicators()
	{
		while (BiomeIndicatorsTab.transform.childCount != 0)
		{
			DestroyImmediate(BiomeIndicatorsTab.transform.GetChild(0).gameObject);
		}
	}

	public void GenerateExtractableOreIndicators(float[,] noiseMap, int totalWorldSize)
	{
		int currentCount = 0;

		while (currentCount < biomeData.ExtractableOreTilesCount)
		{
			int x = Random.Range(0, totalWorldSize);
			int y = Random.Range(0, totalWorldSize);

			if (noiseMap[x, y] < biomeData.BiomeFalloffHeightMax + 0.06f && noiseMap[x, y] > biomeData.BiomeFalloffHeightMax + 0.025)//on the edge where grass turn to mountains mines can be build
			{
				currentCount++;

				
				float topLeftX = (totalWorldSize -1) / -2f; // -1 so that the tiles are placed in the middle
				float topLeftZ = (totalWorldSize -1) / 2f;

				Vector3 currentPos = new Vector3((topLeftX + x) , 0.5f, (topLeftZ - y));
				Physics.Raycast(currentPos + new Vector3(0, 1, 0), Vector3.down, out RaycastHit hit, 20);


				if (hit.point.y != 0.5f) //if a biomeTile has not spawned on this position than spawn tile, tiles are placed on y = 0.65
				{
					GameObject obj = Instantiate(biomeData.ExtractableOreIndicator, currentPos, Quaternion.identity);
					obj.transform.parent = BiomeIndicatorsTab.transform;
				}
				else if (hit.point.y == 0.5f) //if a biomeTile is already on this position replace it
				{
					DestroyImmediate(hit.transform.gameObject);
					GameObject obj = Instantiate(biomeData.ExtractableOreIndicator, currentPos, Quaternion.identity);
					obj.transform.parent = BiomeIndicatorsTab.transform;
				}
			}
		}
	}

	public void GeneratebiomeIndicators(int i, float[,] biomeMap, float[,] noiseMap, int totalWorldSize)
	{
		float topLeftX = (totalWorldSize-1) / -2f;
		float topLeftZ = (totalWorldSize-1) / 2f;

		for (int y = 0; y < totalWorldSize; y++)
		{
			for (int x = 0; x < totalWorldSize; x++)
			{
				if (biomeData.biomes[i].name == "Clay")
				{
					biomeTextureMapClay[x, y] = biomeMap[x, y];
				}
				if (biomeData.biomes[i].name == "PineForest")
				{
					biomeTextureMapForest[x, y] = biomeMap[x, y];
				}
				if (biomeData.biomes[i].name == "OakForest")
				{
					biomeTextureMapOak[x, y] = biomeMap[x, y];
				}

				if (noiseMap[x, y] < biomeData.BiomeFalloffHeightMax) //if no mountain is generated on the position
				{
					if (noiseMap[x, y] > biomeData.BiomeFalloffHeightMin)//checks if spawns on island
					{
						if (biomeMap[x, y] > biomeData.biomes[i].minHeight && biomeMap[x, y] < biomeData.biomes[i].maxHeight) //rarity check
						{

							Vector3 currentPos = new Vector3((topLeftX + x), 0.5f, (topLeftZ - y));
							Physics.Raycast(currentPos + new Vector3(0, 1, 0), Vector3.down, out RaycastHit hit, 10);


							if (hit.transform.position.y == currentPos.y) //if a biomeTile is already on this position check which tile has priority //othereise it will just collide with the water
							{
								int objHitPriority = 0;

								for (int b = 0; b < biomeData.biomes.Length; b++)//get biome priority index of already placed biome indicator
								{
									if (hit.transform.gameObject.CompareTag(biomeData.biomes[b].name))
									{
										objHitPriority = biomeData.biomes[b].placementPriority;
									}
								}

								if (objHitPriority < biomeData.biomes[i].placementPriority) //replace biomeindicator
								{
									GameObject obj = Instantiate(biomeData.biomes[i].Indicator, currentPos, Quaternion.identity);
									obj.tag = biomeData.biomes[i].name;
									obj.transform.parent = BiomeIndicatorsTab.transform;
									DestroyImmediate(hit.transform.gameObject);
								}
							}
							else //if a biomeTile has not spawned on this position than spawn posible tile, tiles are placed on y=0.5
							{
									GameObject obj = Instantiate(biomeData.biomes[i].Indicator, currentPos, Quaternion.identity);
									obj.tag = biomeData.biomes[i].name;//give proper tag
									obj.transform.parent = BiomeIndicatorsTab.transform;
							}         
						}
					}
				}
			}
		}
	}

	public float[,] GetBiomeTextureMap(Noise.MapPosition mapPosition, int value)
    {
		float[,] biomeTextureMap = new float[mapChunkSize, mapChunkSize]; ;
		if(value == 0)
        {
			biomeTextureMap = biomeTextureMapClay;
        }else if (value == 1)
        {
			biomeTextureMap = biomeTextureMapForest;
        }
		else if (value == 2)
		{
			biomeTextureMap = biomeTextureMapOak;
		}

		if (biomeTextureMap == null)
        {
			if(generatebiomes == true) //only give error when biomes are supposed to generate, but when there is no biomemap
            {
				Debug.LogError("No BiomeTextureMap");
			}
			return null;
        }

		float[,] BiomeTextureMap = new float[mapChunkSize,mapChunkSize];

		if (mapPosition == Noise.MapPosition.LeftDown)
		{
			for (int y = 0; y < mapChunkSize; y++)
			{
				for (int x = 0; x < mapChunkSize; x++)
				{
					BiomeTextureMap[x, y] = biomeTextureMap[x, mapChunkSize + y - 1];
				}
			}
		}
		else if (mapPosition == Noise.MapPosition.RightDown)
		{
			for (int y = 0; y < mapChunkSize; y++)
			{
				for (int x = 0; x < mapChunkSize; x++)
				{
					BiomeTextureMap[x, y] = biomeTextureMap[mapChunkSize + x - 1, mapChunkSize + y - 1];
				}
			}
		}
		else if (mapPosition == Noise.MapPosition.LeftUp)
		{
			for (int y = 0; y < mapChunkSize; y++)
			{
				for (int x = 0; x < mapChunkSize; x++)
				{
					BiomeTextureMap[x, y] = biomeTextureMap[x, y];
				}
			}
		}
		else if (mapPosition == Noise.MapPosition.RightUp)
		{
			for (int y = 0; y < mapChunkSize; y++)
			{
				for (int x = 0; x < mapChunkSize; x++)
				{
					BiomeTextureMap[x, y] = biomeTextureMap[mapChunkSize + x - 1, y];
				}
			}
		}
		return BiomeTextureMap;
    }

    public void LoadData(GameData data)
    {
		biomeSeed = data.WorldSeed;
    }

    public void SaveData(GameData data)
    {
        //data.BiomeSeed = biomeSeed;
    }
}

