using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistanceManager : MonoBehaviour
{
    [SerializeField] private bool useEncryption;
    [SerializeField] private MapGeneratorController mapGeneratorController;

    private GameData gameData;
    private List<IDataPersistance> dataPersistancesObjects;
    private FileDataHandler dataHandler;

    public static DataPersistanceManager Instance { get; private set; }//means we can get it publicly but only modify in this class

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Found more than one Data Persistance Manager in the scene.");
        }

        Instance = this; 
    }

    //when starting loading the gamedata
    private void Start()
    {
        this.dataPersistancesObjects = FindAllDataPersistanceObjects();
    }

    public void NewGame(GameData data)
    {
        this.gameData = data;

        //push all the data to other scrips that need it
        foreach (IDataPersistance dataPersistanceObj in dataPersistancesObjects)
        {
            dataPersistanceObj.LoadData(gameData);
        }

        //after all the data is set, finalize game with this data
        mapGeneratorController.GenerateWorld();
    }
    
    public void LoadGame(string name)
    {
        //get right data path for json file
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, name, useEncryption);
        
        //deserialize selected data and load it
        this.gameData = dataHandler.Load();

        //if no data can be loaded error
        if(this.gameData == null)
        {
            Debug.LogError("No Data was found for this name: " + name);       
        }

        //push all the data to other scrips that need it
        foreach(IDataPersistance dataPersistanceObj in dataPersistancesObjects)
        {
            dataPersistanceObj.LoadData(gameData);
        }

        //after all the data is set, finalize game with this data
        mapGeneratorController.GenerateWorld();//generate world
    }

    public void SaveGame()
    {
        //get all current data
        foreach (IDataPersistance dataPersistanceObj in dataPersistancesObjects)
        {
            dataPersistanceObj.SaveData(gameData);
        }

        // save that data to a file using the data handler
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, gameData.WorldName, useEncryption);
        dataHandler.Save(gameData);
    }
    
    //when closing save the game
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects()//find all objects which have variables that store data/need saved data
    {
        IEnumerable<IDataPersistance> dataPersistancesObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();
        return new List<IDataPersistance>(dataPersistancesObjects);
    }
}
