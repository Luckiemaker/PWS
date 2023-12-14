using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGenerator : MonoBehaviour, IDataPersistance
{
    public StructureData structureData;
    int WorldSize = MapGeneratorController.mapChunkSize * 2 -1;
    MapGeneratorController mapGeneratorController;
    public GameObject structureParent;
    public LayerMask GroundLayerMask;
    public Grid buildingGrid;
    float[,] noiseMap;

    [SerializeField]
    private int seed;


    private void Start()
    {
        mapGeneratorController = GetComponentInParent<MapGeneratorController>();
    }
    public void SpawnStructures()
    {
        Random.InitState(seed);

        mapGeneratorController = GetComponentInParent<MapGeneratorController>();

        ClearStructures();
        noiseMap = mapGeneratorController.GetNoiseMap();

        for (int i = 0; i < structureData.Structures.Length; i++)
        {
            for(int j = 0; j < structureData.Structures[i].SpawnTimes; j++)
            {
                int x = Random.Range(0, WorldSize);
                int y = Random.Range(0, WorldSize);


                float topLeftX = (WorldSize - 1) / -2f; // -1 so that the tiles are placed in the middle
                float topLeftZ = (WorldSize - 1) / 2f;

                Vector3 currentPos = new Vector3((topLeftX + x), 0, (topLeftZ - y));

                if (noiseMap[x, y] < structureData.structureMaxHeight) //if no mountain is generated on the position //based on noiseheight
                {
                    if (noiseMap[x, y] > structureData.structureMinHeight)//checks if spawns on island
                    {
                        Physics.Raycast(currentPos + new Vector3(0, 2, 0), Vector3.down, out RaycastHit hit, 5, GroundLayerMask);

                        int rotation = Random.Range(0, 360);//small rotation diff
                        GameObject obj;
                        if (structureData.Structures[i].CentreToGridPos)
                        {
                            obj = Instantiate(structureData.Structures[i].SpawnObject, buildingGrid.GetCellCenterWorld(buildingGrid.WorldToCell(currentPos)), Quaternion.Euler(0, rotation, 0));
                        }
                        else
                        {
                            obj = Instantiate(structureData.Structures[i].SpawnObject, currentPos + new Vector3(0, hit.point.y, 0), Quaternion.Euler(0, rotation, 0));

                        }
                        obj.transform.parent = structureParent.transform;//set parent
                        obj.gameObject.AddComponent<RemoveObjectsInWay>();
                    }
                    else
                    {
                        j -= 1;
                        continue;//when not spawned on the island return and do this iretation AGAIN not NEXT
                    }
                }else
                {
                    j -= 1;
                    continue; //when not spawned on the island return and do this iretation again
                }
            }
        }
    }

    public void ClearStructures()
    {
        while(structureParent.transform.childCount != 0)
        {
            DestroyImmediate(structureParent.transform.GetChild(0).gameObject);
        }
    }

    public void LoadData(GameData data)
    {
        seed = data.WorldSeed;
    }

    public void SaveData(GameData data)
    {
        //nothing yet
    }
}
