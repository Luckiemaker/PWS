using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GridData
{
    private SerializedDictionary<Vector3Int, PlacementData> placedObjects = new();
    
    List<Vector3Int> beachTiles = new();
    List<Vector3Int> locationSpecificTiles = new();


    public void AddNewOccupiedTiles(Vector3Int gridPosition, Vector2Int objectSize, int ID, int PlacedObjectIndex, int rotationState)//add object to list of occupied tiles
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize, rotationState);//tiles which are now occupied because of the placed object
        PlacementData data = new PlacementData(positionsToOccupy, ID, PlacedObjectIndex, rotationState);//store data of positions occupied, object ID, and unique index
       
        foreach(var pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception($"Dictionary already contains this cell position {pos}");
            }
            placedObjects[pos] = data;//positions of occupied tile contains data of object which is placed on him
        }
    }

    public void AddNewBeachTile(Vector3Int pos)
    {
        beachTiles.Add(pos);
    }

    public void AddNewLocationSpecificTiles(Vector3Int pos)
    {
        locationSpecificTiles.Add(pos);
    }

    public void ClearLocationSpecificTiles()
    {
        locationSpecificTiles.Clear();
    }

    public void ClearBeachTiles()
    {
        beachTiles.Clear();
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize, int rotationState)
    {
        List<Vector3Int> positionsToOccupy = new();
        if(objectSize.x == objectSize.y || rotationState == 0)//when rotating has no effect on the occupied tiles
        {
            for (int x = 0; x < objectSize.x; x++)
            {
                for (int y = 0; y < objectSize.y; y++)
                {
                    positionsToOccupy.Add(gridPosition + new Vector3Int(x, y, 0)); //grid is set in editor to xzy so for the z positions we need the middle pos in a vector 3 reffering to cellpos
                }
            }
        }
        else//when rotating does have effect on the occupied tiles
        {
            if(rotationState == 1)//rotated 90
            {
                for (int x = 0; x < objectSize.x; x++)
                {
                    for (int y = 0; y < objectSize.y; y++)
                    {
                        positionsToOccupy.Add(gridPosition + new Vector3Int(y, x - (objectSize.x -1), 0)); //swap x and y size translate position
                    }
                }
            }else if (rotationState == 2)//rotated 180
            {
                for (int x = 0; x < objectSize.x; x++)
                {
                    for (int y = 0; y < objectSize.y; y++)
                    {
                        positionsToOccupy.Add(gridPosition + new Vector3Int(x - (objectSize.x - 1), y - (objectSize.y - 1), 0)); //translate position based on size
                    }
                }
            }else if (rotationState == 3)//rotated 270
            {
                for (int x = 0; x < objectSize.x; x++)
                {
                    for (int y = 0; y < objectSize.y; y++)
                    {
                        positionsToOccupy.Add(gridPosition + new Vector3Int(y - (objectSize.y - 1), x, 0)); //swap x and y size and translate position
                    }
                }
            }
        }
        return positionsToOccupy;
    }
    public bool CanPlaceAtSpecificPosition(Vector3Int gridPosition, Vector2Int objectSize, int rotationState)
    {
        //CODE
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize, rotationState);//checks the positions which needs to be occupied when placing this object
        
        foreach (var pos in positionToOccupy)
        {
            if (!locationSpecificTiles.Contains(pos)) //check if tile is occupyable
            {
                return false;
            }
        }
        return true;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int rotationState)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize, rotationState);//checks the positions which needs to be occupied when placing this object

        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos)) //check if tile is already occupied
            {
                return false;
            }else if (beachTiles.Contains(pos)) // check if tile is on land
            {
                return false;
            }
        }
        return true;
    }

    public bool CanRemoveObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int rotationState)//returns true when a building is placed on this position
    {

        if (placedObjects.ContainsKey(gridPosition))
        {
            return true;
        }
        else
            return false;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
        {
            return -1;//returns -1 if no object is at this position
        }
        else
            return placedObjects[gridPosition].PlacedObjectIndex; //returns the UNIQUE index of placed object on this pos
    }

    internal int GetID(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
        {
            Debug.LogError("No object in this positions");
            return -1;//returns -1 if no object is at this position
        }
        else
            return placedObjects[gridPosition].ID; //returns the OBJECT index of placed object on this pos
    }

    internal int GetRotationState(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
        {
            return -1;//returns -1 if no object is at this position
        }
        else
            return placedObjects[gridPosition].RotationState; //returns the rotationState
    }

    internal Vector3Int GetCenterRotatedTileObject(Vector3Int gridPosition, int rotationState)
    {
        int posX;
        int posY;

        List<Vector3Int> gridPositions = placedObjects[gridPosition].occupiedPositions;
        if (rotationState == 0)
        {
            posX = 10000000;//get min
            posY = 10000000;

            foreach (var pos in gridPositions)
            {
                if (pos.x < posX)
                {
                    posX = pos.x;
                }
                if (pos.y < posY)
                {
                    posY = pos.y;
                }
                gridPosition = new Vector3Int(posX, posY, pos.z);
            }
        }else if (rotationState == 1)
        {
            posX = 100000000;//get min
            posY = -100000000;//get max
            foreach (var pos in gridPositions)
            {

                if (pos.x < posX)
                {
                    posX = pos.x;
                }
                if (pos.y > posY)
                {
                    posY = pos.y;
                }
                gridPosition = new Vector3Int(posX, posY, pos.z);
            }
        } else if (rotationState == 2)
        {
            posX = -100000000;//get max
            posY = -100000000;//get max
            foreach (var pos in gridPositions)
            {

                if (pos.x > posX)
                {
                    posX = pos.x;
                }
                if (pos.y > posY)
                {
                    posY = pos.y;
                }
                gridPosition = new Vector3Int(posX, posY, pos.z);
            }
        }else if (rotationState == 3)
        {
            posX = -100000000;//get max
            posY = 100000000;//get min

            foreach (var pos in gridPositions)
            {

                if (pos.x > posX)
                {
                    posX = pos.x;
                }
                if (pos.y < posY)
                {
                    posY = pos.y;
                }
                gridPosition = new Vector3Int(posX, posY, pos.z);
            }
        }

        return gridPosition;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;//positions which are occupied by the object
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }
    public int RotationState { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex, int rotationState)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
        RotationState = rotationState;
    }
}
