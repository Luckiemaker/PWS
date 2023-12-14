using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    private Grid grid;
    private PreviewSystem previewSystem;
    private ObjectsDataBase database;
    private GridData stackableBuildingData;
    private GridData solidBuildingData;
    private ObjectPlacer objectPlacer;
    private PlacementSystem placement;

    private int rotationState;

    public RemovingState(Grid grid,PreviewSystem previewSystem,ObjectsDataBase database,GridData stackableData, GridData solidData,ObjectPlacer objectPlacer, PlacementSystem placementSystem)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.stackableBuildingData = stackableData;
        this.solidBuildingData = solidData;
        this.objectPlacer = objectPlacer;
        this.placement = placementSystem;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)//on remove
    {
        GridData selectedData = GetHoveringBuildingData(gridPosition);//check which dataSet should be used

        if (selectedData == null)//if place is vacant
        {
            return;
        }

        int BuildingID = selectedData.GetID(gridPosition);

        if (placement.checkForBuildingAmountRestrictions(BuildingID))//when a building gets removed with a maximumBuidAMount then change list so a new building of this type can be build later
        {
            int index = placement.GetIndexFromIDBuildingRestrictions(BuildingID);//get index

            if (placement.BuildingAmountRestrictions[index].isInRemoveable)//check if a building should be removable
            {
                //maybe later shoe message with"this building can not be removed"
                return;
            }

            if (placement.BuildingAmountRestrictions[index].AllowedPlacedBuildings == placement.BuildingAmountRestrictions[index].CurrentlyPlacedBuildings)
            {
                placement.EnableBuildingAbilityOfBuilding(index);//when this building type was at its maximum, but now when removed enable all features to build a building with this ID again
            }

            placement.ModifyBuildingRestrictionList(index, -1, 0);
        }

        gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);//get unique index of object on this position
        selectedData.RemoveObjectAt(gridPosition);//remove occupied tile positions
        objectPlacer.RemoveObjectAt(gameObjectIndex);//remove object


        UpdateState(gridPosition);//update new position
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)//return true when a building is already placed in this position
    {
        if (solidBuildingData.CanRemoveObjectAt(gridPosition, Vector2Int.one, rotationState) || stackableBuildingData.CanRemoveObjectAt(gridPosition, Vector2Int.one, rotationState))
        {
            GridData selectedData = GetHoveringBuildingData(gridPosition);
            int ID = selectedData.GetID(gridPosition);
            
            if (placement.checkForBuildingAmountRestrictions(ID))//first check if buildingID is on the list before telling the game to find it on the list
            {
                int index = placement.GetIndexFromIDBuildingRestrictions(ID);//check if a placed building has the property of not being able to be removed

                if (placement.BuildingAmountRestrictions[index].isInRemoveable)
                {
                    return false;//is building has propertie of not being able to be removed set validity to false to get default remove preview
                }
            }

            return true;
        }
        else
            return false;
    }

    private GridData GetHoveringBuildingData(Vector3Int gridPosition)
    {
        GridData selectedData = null;

        if (solidBuildingData.CanRemoveObjectAt(gridPosition, Vector2Int.one, rotationState))
        {
            selectedData = solidBuildingData;//checks if in this layer the positions is taken set correspondig databuilding
        }
        else if (stackableBuildingData.CanRemoveObjectAt(gridPosition, Vector2Int.one, rotationState))
        {
            selectedData = stackableBuildingData;//checks if in this layer the positions is taken and set correspondig databuilding
        }

        return selectedData;
    }

    public void UpdateState(Vector3Int gridPosition)//gets checked every update
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);//returns true if a object is on position
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);//only update cellindicator remove possition when off the object
        previewSystem.StartShowingRemovePreview();//default removepreview

        if(validity)//when hovering over placed objects
        {
            GridData selectedData = GetHoveringBuildingData(gridPosition);

            if(selectedData == null)
            {
                previewSystem.StartShowingRemovePreview();
                Debug.LogError("No BuildingData Selected");
                return;
            }
            int Index = selectedData.GetID(gridPosition);//get OBJECT ID for this position //PROBLEM: nothing placed, but still occupied in data layers(BeachTIles???)
            Vector2Int size = database.objectsData[Index].Size;//get object size

            int rotationState = selectedData.GetRotationState(gridPosition);//get rotationstate
            Vector3Int pos;//get center tile from rotated object(left tile when no rotated) tile if multiple tiles are occupied by Object so that remove indicator will stay in one place when selecting that building
          
            if (size.x == size.y)//if hovering object is equally sided than get leftDown Corner as position (default corner) to apply indicator so that when hovering over other tile with same object the position is the same thus the position of the indicator is also fixed
            {
                pos = selectedData.GetCenterRotatedTileObject(gridPosition, 0);
                previewSystem.UpdateBuildingRemovePreview(size, grid.CellToWorld(pos), 0);
            }
            else
            {
                pos = selectedData.GetCenterRotatedTileObject(gridPosition, rotationState);
                previewSystem.UpdateBuildingRemovePreview(size, grid.CellToWorld(pos), rotationState);//update remove building preview indicator acoording to size and rotation/position
            }
        }
    }
}
