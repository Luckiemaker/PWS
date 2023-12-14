using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    private int ID;
    private Grid grid;
    private PreviewSystem previewSystem;
    private ObjectsDataBase database;
    private GridData stoneDepositData;
    private GridData DefaultData;
    private ObjectPlacer objectPlacer;
    private int rotationState;
    private PlacementSystem placement;

    public PlacementState(int iD, Grid grid, PreviewSystem previewSystem, ObjectsDataBase database, GridData stoneDepositData, GridData defaultData, ObjectPlacer objectPlacer, int rotateState, PlacementSystem placement)
    {
        ID = iD;//set all values of object for script
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.stoneDepositData = stoneDepositData;
        this.DefaultData = defaultData;
        this.objectPlacer = objectPlacer;
        this.rotationState = rotateState;
        this.placement = placement;

        selectedObjectIndex = ID;  //get object with given ID so we can acces all its components       //database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)//if a object is selected
        {
            previewSystem.StartShowingPlacementPreview(
            database.objectsData[selectedObjectIndex].Prefab,
            database.objectsData[selectedObjectIndex].Size,
            rotateState);//start to show the preview of object
        }
        else
            throw new System.Exception($"No object with ID {iD}");
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);//checks if tiles are not occupied
        if (placementValidity == false)
        {
            return;
        }

        if (placement.checkForBuildingAmountRestrictions(ID))//if this ID correspond with a buildingtype with building amount restrictions, update data
        {
            int BuildListIndex = placement.GetIndexFromIDBuildingRestrictions(ID);

            placement.ModifyBuildingRestrictionList(BuildListIndex, 1, 0);//add new placed building to the list

            if (placement.BuildingAmountRestrictions[BuildListIndex].AllowedPlacedBuildings == placement.BuildingAmountRestrictions[BuildListIndex].CurrentlyPlacedBuildings)
            {
                //after this building is build and has reached it limits, disable all means to build this building again
               placement.DisableBuildingAbilityOfBuilding(BuildListIndex);
            }
        }

        int index = objectPlacer.PlaceObject(
            selectedObjectIndex,
            grid.CellToWorld(gridPosition),
            database.objectsData[selectedObjectIndex].Size,
            rotationState);//place object and get index of how many times a object has been placed(objects unique number)

        DefaultData.AddNewOccupiedTiles(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index,
            rotationState);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false); //preview system needs to show that a object is placed here, set validity to false
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        if (database.objectsData[selectedObjectIndex].IsObjectSpecific)
        {
            GridData specificLocationData;

            if (database.objectsData[selectedObjectIndex].ID == 2)//INDEX FOR STONEDEPOSIT IS HARDCODED!!!
            {
                specificLocationData = stoneDepositData;
            }
            else
                throw new System.Exception($"GameObject with ID {selectedObjectIndex} has no object specific DataBase");

            bool locationValidity = specificLocationData.CanPlaceAtSpecificPosition(gridPosition, database.objectsData[selectedObjectIndex].Size, rotationState);
            if (locationValidity == false) //if not on specific location validity is false and the building can not be placed
            {
                return false;
            }            
        }

        bool gridValidity = DefaultData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, rotationState);//checks if object can be placed on this positions for this data layer
        bool objectValdity = previewSystem.checkMountainCollision();//chekcs if building will collide with mountain or spawned objects
        bool checkInfrastructureReach = previewSystem.checkInfrastructureValidity();

        if (objectValdity && gridValidity && checkInfrastructureReach)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);//update placement validity and cursor position/color
    }
    
}
