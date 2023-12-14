using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private ObjectsDataBase database;
    [SerializeField]
    private GameObject gridVisualisation;
    [SerializeField]
    private PreviewSystem previewSystem;
    [SerializeField]
    private ObjectPlacer objectPlacer;
    [SerializeField]
    private UI uiManager;

    [SerializeField]
    private LayerMask worldLayerMask;
    [SerializeField]
    private GameObject stoneTilesVisualisation;

    private GridData DefaultGridData;//initialized during loading
    private GridData StoneDepositData;//only contains specific locations for the building to be build

    private Vector3 lastDetectedPosition = Vector3Int.zero;
    private int rotationCount;
    private int selectedBuildingIndex;
    int totalWorldSize = MapGeneratorController.mapChunkSize * 2 - 1; //-1 because inner edges of worldmaps are shared
    float gridCellSize;

    [SerializeField]
    public List<BuildingLimitData> BuildingAmountRestrictions;

    IBuildingState buildingState;

    private void Start()
    {
        gridCellSize = grid.cellSize.x; //x and z should be the same
        StoneDepositData = new();
        DefaultGridData = new();
        StopPlacement(); //from default be out of building mode
        ClearGridData();
        CalculateGridBeachTiles();

        inputManager.OnR += RotateStructure;//rotate building when R pressed
    }

    public void StartPlacement(int ID)//called with buttons for seperate item
    {
        if (checkForBuildingAmountRestrictions(ID))//if given id has a building restriction amount then return true
        {
            int index = GetIndexFromIDBuildingRestrictions(ID);

            if (BuildingAmountRestrictions[index].AllowedPlacedBuildings == BuildingAmountRestrictions[index].CurrentlyPlacedBuildings)//check if this building when placed would not exceed the maximum allowed buildings of this type
            {
                StopPlacement();
                return;//when a buidingtype has reached its maximum amount of placed buildings, return en do not allow to build en extra
            }
        }

        StopPlacement();
        rotationCount = 0;
        selectedBuildingIndex = ID;
        if(selectedBuildingIndex == 2)//ID of stoneMines
        {
            stoneTilesVisualisation.SetActive(true);
        }
        gridVisualisation.SetActive(true);
        buildingState = new PlacementState(ID, grid, previewSystem, database, StoneDepositData, DefaultGridData, objectPlacer, rotationCount, this);

        inputManager.OnLeftClick += PlaceStructure;//when left clicked place
        inputManager.OnESC += StopPlacement;//when ESC key pressed exit
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualisation.SetActive(true);
        buildingState = new RemovingState(grid, previewSystem, database, StoneDepositData, DefaultGridData, objectPlacer, this);
        inputManager.OnLeftClick += PlaceStructure;
        inputManager.OnESC += StopPlacement;
    }

    private void RotateStructure()
    {
        if(buildingState == null)
        {
            return;//only rotate when in building mode otherwise return
        }
        buildingState.EndState();
        //rotatations are defined by a int, 0 for default, 1 for 90 turn, 2 for 180 and 3 for 270
        rotationCount ++;

        if (rotationCount == 4)
        {
            rotationCount = 0;
        }
        buildingState = new PlacementState(selectedBuildingIndex, grid, previewSystem, database, StoneDepositData, DefaultGridData, objectPlacer, rotationCount, this);//change buildingstate with right rotation index
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointedOverUI())//if clicked on UI in while in placementmode do nothing
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetMousePosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);//convert to grid position
       
        buildingState.OnAction(gridPosition);//remove or place depending on buildingstate
    }

    public void StopPlacement()
    {
        gridVisualisation.SetActive(false);
        stoneTilesVisualisation.SetActive(false);

        if (buildingState == null)//if already out of buildingmode
            return;
        buildingState.EndState();
        inputManager.OnLeftClick -= PlaceStructure;//remove events so that it only works when entered buildingmode
        inputManager.OnESC -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;
        buildingState = null; //player out of building menu so no buildingstate
    }

    public void CalculateGridBeachTiles()//calculate based on the height difference what is mainland, all other tiles (water/beach) will be sorted in a list
    {
        float topLeftX = (totalWorldSize - gridCellSize) / -2f; //-gridcellsize now the position is in the middle of all the cells
        float topLeftZ = (totalWorldSize - gridCellSize) / 2f;

        for (float y = 0; y < totalWorldSize; y += gridCellSize)
        {
            for (float x = 0; x < totalWorldSize; x += gridCellSize)
            {
                Vector3 currentPos = new Vector3(topLeftX + x, 1, topLeftZ - y);
                Physics.Raycast(currentPos, Vector3.down, out RaycastHit hit, 3, worldLayerMask);

                if (hit.distance > (1 - 0.283f))//MANUAL VALUES HARDCODED //check based on the distance if tile is off the island or not
                {
                    Vector3Int gridPosition = grid.WorldToCell(currentPos);//get currentcellposition
                    DefaultGridData.AddNewBeachTile(gridPosition); //add the tiles to a list
                    //stackableBuildingData.AddNewBeachTile(gridPosition);
                }
            }
        }
    }

    public void ClearGridData()//clear TilesList for when a new world is generated
    {
        if (DefaultGridData != null)//null check only when in play mode these data are made but in edit mode not now i still can generate maps in edit mode
        {
            DefaultGridData.ClearBeachTiles();
            StoneDepositData.ClearLocationSpecificTiles();

            while (stoneTilesVisualisation.transform.childCount != 0)
            {
                DestroyImmediate(stoneTilesVisualisation.transform.GetChild(0).gameObject);
            }
        }
    }

    private void Update()
    {
        if(buildingState == null)
        {
            return;//if the player is not in building menu
        }
        Vector3 mousePosition = inputManager.GetMousePosition();//get mouseposition
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);//get currentcellposition

        buildingState.UpdateState(gridPosition);
        //lastDetectedPosition = gridPosition;    used for loop to check if player changed gridposition, when changing grid only than update, not possible enymore thus deleted
    }
    public Grid GetGrid()
    {
        return grid;
    }

    public GameObject GetStoneTileVis()
    {
        return stoneTilesVisualisation;
    }
    public GridData GetStoneData()
    {
        return StoneDepositData;
    }







    public bool checkForBuildingAmountRestrictions(int BuildingID)
    {
        for(int i = 0; i < BuildingAmountRestrictions.Count; i++)
        {
            if (BuildingAmountRestrictions[i].ID == BuildingID)//if given id is same as an id saved in restrictedbuilding list then return true
            {
                return true;
            }
        }
        return false;
    }

    public void ModifyBuildingRestrictionList(int index, int currentPlacedBuildingsChangeAmount, int maxPlacedBuildingsChangeAmount)
    {
        BuildingAmountRestrictions[index].CurrentlyPlacedBuildings += currentPlacedBuildingsChangeAmount;
        BuildingAmountRestrictions[index].AllowedPlacedBuildings += maxPlacedBuildingsChangeAmount;
    }

    public void SetMaxBuildingRestrictions(int index, int maxPlacedBuildingsChangeAmount)
    {
        BuildingAmountRestrictions[index].AllowedPlacedBuildings = maxPlacedBuildingsChangeAmount;      
    }

    public void DisableBuildingAbilityOfBuilding(int index)
    {
        StopPlacement();
        uiManager.DisableButton(BuildingAmountRestrictions[index].BuildButton);
    }

    public void EnableBuildingAbilityOfBuilding(int index)
    {
        uiManager.EnableButton(BuildingAmountRestrictions[index].BuildButton);
    }

    public int GetIndexFromIDBuildingRestrictions(int ID)
    {
        for (int i = 0; i < BuildingAmountRestrictions.Count; i++)
        {
            if (BuildingAmountRestrictions[i].ID == ID)//if given id is same as id saved in restrictedbuilding list then return true
            {
                return i;
            }
        }
        Debug.LogError("No index for this Building ID");
        return -1;
    }
}

[Serializable]
public class BuildingLimitData
{
    public int ID;
    public int CurrentlyPlacedBuildings;
    public int AllowedPlacedBuildings;
    public Button BuildButton;
    public bool isInRemoveable;

    public BuildingLimitData(int iD, int MaxPlacedBuildings, int currentPlacedBuildings, Button Button, bool isNotRemoveable)
    {
        ID = iD;
        CurrentlyPlacedBuildings = currentPlacedBuildings;
        AllowedPlacedBuildings = MaxPlacedBuildings;
        BuildButton = Button;
        isInRemoveable = isNotRemoveable;
    }
}
