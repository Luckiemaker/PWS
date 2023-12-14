using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectPlacer : MonoBehaviour, IDataPersistance
{
    [SerializeField]
    private SerializedDictionary<int, PlacedObjectsData> CurrentPlacedGameObjects = new();

    [SerializeField]
    private SerializedDictionary<int , PlacedObjectsData> placedGameObjectsNEW;

    public ObjectsDataBase dataBase;

    float XOffset;
    float ZOffset;

    [SerializeField]
    private Grid grid;
    private float gridCellSize;

    private void Start()
    {
        gridCellSize = grid.cellSize.x;//cell sizes are all equal thus does not matter if get x or z
    }
    public int PlaceObject(int index, Vector3 position, Vector2Int size, int rotationState)
    {
        GameObject obj = Instantiate(dataBase.objectsData[index].Prefab);

        if (size.x == size.y)//if object is equally sided on every side than rotate object without changing the position
        {
            if (rotationState == 0)
            {
                obj.transform.rotation = Quaternion.Euler(0, 0, 0);
                XOffset = 0;
                ZOffset = 0;

            }
            else if (rotationState == 1)
            {
                obj.transform.rotation = Quaternion.Euler(0, 90, 0);
                XOffset = 0;
                ZOffset = gridCellSize * size.y;
            }
            else if (rotationState == 2)
            {
                obj.transform.rotation = Quaternion.Euler(0, 180, 0);
                XOffset = gridCellSize * size.x;
                ZOffset = gridCellSize * size.y;
            }
            else if (rotationState == 3)
            {
                obj.transform.rotation = Quaternion.Euler(0, 270, 0);
                XOffset = gridCellSize * size.x;
                ZOffset = 0;
            }
        }
        else//rotate with small position change to let it look better
        {
            if (rotationState == 0)
            {
                obj.transform.rotation = Quaternion.Euler(0, 0, 0);
                XOffset = 0;
                ZOffset = 0;

            }
            else if (rotationState == 1)
            {
                obj.transform.rotation = Quaternion.Euler(0, 90, 0);
                XOffset = 0;
                ZOffset = gridCellSize;
            }
            else if (rotationState == 2)
            {
                obj.transform.rotation = Quaternion.Euler(0, 180, 0);
                XOffset = gridCellSize;
                ZOffset = gridCellSize;
            }
            else if (rotationState == 3)
            {
                obj.transform.rotation = Quaternion.Euler(0, 270, 0);
                XOffset = gridCellSize;
                ZOffset = 0;
            }
        }

        obj.transform.position = position + new Vector3(XOffset, 0, ZOffset);//set correct position
        CurrentPlacedGameObjects.Add(CurrentPlacedGameObjects.Count, new PlacedObjectsData(obj,index, position, size, rotationState));//add to list
        return CurrentPlacedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (CurrentPlacedGameObjects.Count <= gameObjectIndex || CurrentPlacedGameObjects[gameObjectIndex] == null) //if no object exists
            return;

        Destroy(CurrentPlacedGameObjects[gameObjectIndex].Obj);

        CurrentPlacedGameObjects[gameObjectIndex] = null;//remove data

        placedGameObjectsNEW.Remove(gameObjectIndex);
    }

    public void LoadPlacedObjects()
    {
        for (int i = 0; i < placedGameObjectsNEW.Count; i++)
        {
            PlaceObject(placedGameObjectsNEW[i].ObjIndex, placedGameObjectsNEW[i].Position, placedGameObjectsNEW[i].Size, placedGameObjectsNEW[i].Rotation);
        }
    }

    public void LoadData(GameData data)
    {
        placedGameObjectsNEW = data.PlacedObjects;
        LoadPlacedObjects();
    }

    public void SaveData(GameData data)
    {
        data.PlacedObjects = CurrentPlacedGameObjects;
    }

    [System.Serializable]
    public class PlacedObjectsData
    {
        public GameObject Obj;
        public int ObjIndex;
        public Vector3 Position;
        public Vector2Int Size;
        public int Rotation;

        public PlacedObjectsData(GameObject obj, int objIndex, Vector3 position, Vector2Int size, int rotation)
        {
            Obj = obj;
            ObjIndex = objIndex;
            Position = position;
            Size = size;
            Rotation = rotation;
        }
    }
}
