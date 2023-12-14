using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorController : MonoBehaviour, IDataPersistance
{
    public bool autoUpdate;
    public bool generateBiomes;
    public bool generateRandomMap;
    public Noise.NormalizeMode normalizeMode;

    [SerializeField]private GenerationDataSelector.GenerationMode GenerationMode;
    private GenerationDataSelector.GenerationData generationData;

    public const int mapChunkSize = 100;

    private BiomeGenerator biomeGenerator;
    private BiomeObjectsHandler objectGenerator;
    private GenerationDataSelector generationDataSelector;

    public PlacementSystem placementSystem;
    public StructureGenerator structureGenerator;

    [SerializeField] private int worldSeed;

    private float[,] noiseMap;
    private float[,] fallOffMap;

    private void Awake()
    {
        transform.name = "WorldGenerator"; //Some code depends on this name for this scrip.
    }

    public void GenerateWorld()
    {
        if (!Application.isPlaying)
        {
            transform.BroadcastMessage("EditorMeshNullState");
        }

        //clear stored data beachtiles
        placementSystem.ClearGridData();

        biomeGenerator = GetComponent<BiomeGenerator>();
        objectGenerator = GetComponent<BiomeObjectsHandler>();
        generationDataSelector = GetComponent<GenerationDataSelector>();

        if(generationDataSelector == null)
        {
            Debug.LogError("can not retrieve data");
            return;
        }


        generationData = generationDataSelector.GetGenerationData(GenerationMode);

        if (generateRandomMap)
        {
            worldSeed = Random.Range(-1000000000, 1000000000);

           noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize,
           worldSeed, //get random seed everytime
           generationData.noiseData.noiseScale,
           generationData.noiseData.octaves,
           generationData.noiseData.persistance,
           generationData.noiseData.persistanceMultiplierMountains,
           generationData.noiseData.lacunarity,
           generationData.noiseData.offset,
           normalizeMode);
        }else
        {
           noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize,
           worldSeed,
           generationData.noiseData.noiseScale,
           generationData.noiseData.octaves,
           generationData.noiseData.persistance,
           generationData.noiseData.persistanceMultiplierMountains,
           generationData.noiseData.lacunarity,
           generationData.noiseData.offset,
           normalizeMode);
        }

        fallOffMap = FallOffGenerator.GenerateFalloffMap(mapChunkSize,
            generationData.terrainData.falloffStart,
            generationData.terrainData.falloffEnd,
            generationData.terrainData.useCircularFalloff);


        //generateBiomes
        if (generateBiomes)
        {
            biomeGenerator.GenerateBiomes(GetNoiseMap(), mapChunkSize);
        }
        else
        {
            biomeGenerator.ClearBiomeIndicators();
            objectGenerator.Clear(); //clear all previous spawned objects

            //Generate map throug smaller maps
            transform.BroadcastMessage("GenerateMap");//only execute in this script when no biomes are generated because if biomees are generated than that script will set this line of code in motion 
        }
        //set water
        //waterGenerator.GenerateWaters(noiseMap);
    }

    public int GetMapChunkSize()
    {
        return mapChunkSize;
    }

    public void FinalizeWorldGen()//called once after the world mesh is generated
    {
        if (placementSystem != null && Application.isPlaying)//When running in the editor CalculateGridBeachTiles causes error for some reason. might be unity bug
        {
            placementSystem.CalculateGridBeachTiles();
            structureGenerator.SpawnStructures();
        }
        else if (placementSystem == null && Application.isPlaying)
        {
            throw new System.Exception("placementSystem is null, can not calculate beachTiles");
        }
        else if(placementSystem == null)
        {
            Debug.LogError("PlacementSystem is null");
        }
    }

    public float[,] GetNoiseMap()
    {
        float[,] NoiseMap = new float[noiseMap.GetLength(0), noiseMap.GetLength(1)];//need to define new float every time this method is called otherwise the fallofmap will be substracted
                                                                                    //from the noisemap multiple time depending on the place where gameobject stands in hierarchy of unity
        if (generationData.terrainData.useFalloff)
        {
            int size = fallOffMap.GetLength(0);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    NoiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - fallOffMap[x, y]);
                }
            }
        }
        else
        {
            NoiseMap = noiseMap;//when no falloff applied return defaut noisemap
        }

        return NoiseMap;
    }

    public TerrainData GetTerrainData()
    {
        return generationData.terrainData;
    }

    public void LoadData(GameData data)
    {
        GenerationMode = data.WorldGenerationMode;
        worldSeed = data.WorldSeed;//safe seed seperately because seed will change every other game while generatation settings will be fixed for each profile
    }

    public void SaveData(GameData data)
    {
        data.WorldSeed = worldSeed;
        data.WorldGenerationMode = GenerationMode;
    }
}
