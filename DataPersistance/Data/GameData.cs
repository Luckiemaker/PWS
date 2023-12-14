using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class GameData
{
    public string WorldName;

    public GenerationDataSelector.GenerationMode WorldGenerationMode;
    public int WorldSeed;

    public Vector3 PlayerPosition;

    public int Wood;
    public int Stone;
    public int Food;

    public SerializedDictionary<int, ObjectPlacer.PlacedObjectsData> PlacedObjects;
    public SerializedDictionary<Vector3Int, PlacementData> PlacedObjectsData;

    public GridData DefaultGridData;
    public GridData StoneDepositData;

    public SerializedDictionary<int, SapplingData> SapplingsPlaced;

    public List<BiomeObjectData> DestroyedBiomeSpawnObjects;

    //values defined in this constructer will be default values
    public GameData(string worldName)
    {
        this.WorldName = worldName;

        this.WorldGenerationMode = GenerationDataSelector.GenerationMode.Default;
        this.WorldSeed = Random.Range(-2000000000, 2000000000);

        this.PlayerPosition = new Vector3(0, 1, 0);

        this.Wood = 50;
        this.Stone = 20;
        this.Food = 60;

        this.PlacedObjects = new SerializedDictionary<int, ObjectPlacer.PlacedObjectsData>();

        this.PlacedObjectsData = new();

        this.SapplingsPlaced = new SerializedDictionary<int, SapplingData>();

        this.DestroyedBiomeSpawnObjects = new List<BiomeObjectData>();//destroyed spawned objects at default
    }
}
