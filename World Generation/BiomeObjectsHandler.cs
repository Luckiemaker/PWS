using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BiomeObjectsHandler : MonoBehaviour, IDataPersistance
{
    [SerializeField]
    int Mapwidth;

    [SerializeField]
    private int seed;
    [SerializeField]
    private LayerMask biomeTilesLayer;

    public GameObject ObjectsHolder;
    public GameObject TreeSpawnPointObject;

    public bool generateObjects;
    public bool destroyBiomeIndicatorsAfterSpawn;

    public BiomeData BiomeData;
    private BiomeGenerator biomeGenerator;

    private List<BiomeObjectData> destroyedBiomeSpawnObjects;
    private SerializedDictionary<int, SapplingData> placedSapplings;


    private void Start()
    {
        Clear();
    }

    public void GenerateBiomeObjects()
    {
        biomeGenerator = GetComponent<BiomeGenerator>();
        GenerateSeedObjects();
    }

    public void GenerateSeedObjects()
    {
        Clear();//clear current objects

        Random.InitState(seed);

        if (generateObjects)
        {

            float LowestCoordinate = -Mapwidth / 2;
            float HighestCoordinate = Mapwidth / 2;

          
            for (int b = 0; b < BiomeData.biomes.Length; b++) //irritate trough all biomes
            {
                for (int o = 0; o < BiomeData.biomes[b].objects.Length; o++) //irritate trough all objects which this biomes needs to spawn
                {
                    for (int i = 0; i < BiomeData.biomes[b].objects[o].TotalSpawnAttempts; i++)//how many times this specific object should trough to spawn
                    {
                        float sampleX = Random.Range(LowestCoordinate, HighestCoordinate);
                        float sampleZ = Random.Range(LowestCoordinate, HighestCoordinate);

                        Vector3 rayStart = new Vector3(sampleX, 2, sampleZ);

                        if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 5, biomeTilesLayer))
                        {
                            continue;//no hit? than to next itteration
                        }

                        if (hit.transform.CompareTag(BiomeData.biomes[b].name))//if ray hits corresponding biometile than spawn
                        {
                            Vector3 SpawnCoordinates = hit.point - new Vector3(0, 0.225f, 0); //-0.225 for y coordinates because that is the distance the biome indicators spawn above the ground 
                           

                            float rotationDiff = Random.Range(0, 360);
                            float scaleDiff = Random.Range(-0.03f, 0.025f);

                            GameObject instantiatedPrefab = Instantiate(BiomeData.biomes[b].objects[o].Object.gameObject, SpawnCoordinates, Quaternion.Euler(0, rotationDiff, 0));
                            instantiatedPrefab.transform.parent = ObjectsHolder.transform;
                            instantiatedPrefab.transform.localScale = instantiatedPrefab.transform.localScale + new Vector3(0, scaleDiff, 0);
                        }
                    }
                }
            }
            FinalizeObjects();
        }

        if (destroyBiomeIndicatorsAfterSpawn == true)
        {
            biomeGenerator.ClearBiomeIndicators();
        }
    } 

    public void Clear()
    {
        while (ObjectsHolder.transform.childCount != 0)
        {
            DestroyImmediate(ObjectsHolder.transform.GetChild(0).gameObject);
        }
    }

    public void AddObjectToDestroyedBiomesObjectsList(BiomeObjectData data)
    {
        destroyedBiomeSpawnObjects.Add(data);
    }

    public void RemoveObjectToDestroyedBiomesObjectsList(BiomeObjectData data)
    {
        destroyedBiomeSpawnObjects.Remove(data);//does not work
        Debug.Log("removed");
    }

    public void AddObjectToSapplingsList(int index, SapplingData data)
    {
        placedSapplings.Add(index, data);
    }

    public void RemoveObjectToSapplingsList(int index)
    {
        placedSapplings.Remove(index);
    }

    public void FinalizeObjects()
    {
        if(destroyedBiomeSpawnObjects != null)
        {
            for (int i = 0; i < destroyedBiomeSpawnObjects.Count; i++)
            {
                if (destroyedBiomeSpawnObjects[i] != null)
                {
                    Vector3 pos = destroyedBiomeSpawnObjects[i].Position;

                    if (Physics.Raycast(pos + new Vector3(0, 1, 0), Vector3.down, out RaycastHit hit, 5))
                    {
                        Destroy(hit.transform.gameObject);
                    }
                    else
                        Debug.Log("Index: " + i + "not destroyed");
                }
            }
        }
        
        if(placedSapplings != null)
        {
            for (int i = 0; i < placedSapplings.Count; i++)
            {
                if (placedSapplings[i] != null)
                {
                    GameObject obj = TreeSelector(placedSapplings[i].Name);
                    GameObject prefab = Instantiate(TreeSpawnPointObject, placedSapplings[i].Position, Quaternion.identity);

                    TreeGrowth script = prefab.GetComponent<TreeGrowth>();
                    script.SetTreeToSpawn(obj);
                    script.StartGrowth(placedSapplings[i].GrowthStage);
                }
            }
        }
    }

    public GameObject TreeSelector(string name)
    {
        if (name == "Oak_Tree_01")
        {
            return BiomeData.biomes[0].objects[0].Object;
        }
        else if (name == "Oak_Tree_02")
        {
           
            return BiomeData.biomes[0].objects[1].Object;
        }
        else if (name == "Pine_Tree_01")
        {
           
            return BiomeData.biomes[1].objects[0].Object;
        }
        else if (name == "Pine_Tree_02")
        {
           
            return BiomeData.biomes[1].objects[0].Object;
        }
        else
            throw new System.Exception($"No tree of type {name}");
    }

    public int GetNewSapplingIndex()
    {
        return placedSapplings.Count;
    }

    public void UpdateSapplingData(int index, SapplingData data)
    {
        if (placedSapplings.ContainsKey(index))
        {
            placedSapplings.Remove(index);
            placedSapplings.Add(index, data);
            Debug.Log("update");
        }
    }

    public void LoadData(GameData data)
    {
        destroyedBiomeSpawnObjects = data.DestroyedBiomeSpawnObjects;
        placedSapplings = data.SapplingsPlaced;
        seed = data.WorldSeed;
    }

    public void SaveData(GameData data)
    {
        data.SapplingsPlaced = placedSapplings;
        data.DestroyedBiomeSpawnObjects = destroyedBiomeSpawnObjects;     
    }
}

[System.Serializable]
public class BiomeObjectData
{
    public string Tag;
    public Vector3 Position;


    public BiomeObjectData(Vector3 position)
    {
        Position = position;
    }
}

[System.Serializable]
public class SapplingData
{
    public string Name;
    public Vector3 Position;
    public int GrowthStage;

    public SapplingData(string name, Vector3 pos, int growthStage)
    {
        Name = name;
        Position = pos;
        GrowthStage = growthStage;
    }
}
