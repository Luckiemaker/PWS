using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystemOld : MonoBehaviour
{
    public static BuildingSystemOld current;


    public GridLayout gridLayout;
    private Grid grid;
    [SerializeField] private Tilemap MainTilemap;
    [SerializeField] private TileBase whiteTile;

    public GameObject path;
    public GameObject smallHouse;
    public GameObject house;
    public GameObject largeHouse;

    private PlaceableObject objectToPlace;

    private bool buildingSelected = false;


    #region Unity methods

    private void Awake()
    {
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    private void Update()
    {
        BuildingSelections();

        if (!objectToPlace)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            objectToPlace.Rotate();
        }

        if (Input.GetKeyDown(KeyCode.Space) && buildingSelected)
        {
            if (CanBePlaced(objectToPlace))
            {
                buildingSelected = false;
                Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
                TakeArea(start, objectToPlace.Size);
                Debug.Log(start);
                Debug.Log(objectToPlace.Size);
                objectToPlace.Place();

                //pay
            }
            else
            {
                buildingSelected = false;
                Destroy(objectToPlace.gameObject);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Backspace) && buildingSelected) // only when building is selected thus not yet placed and key is pressed can building be deleted.
        {
            buildingSelected = false;
            Destroy(objectToPlace.gameObject);
        }
    }

    #endregion

    #region utils

    private void BuildingSelections()                //Checks if a certain building is selected to build
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && buildingSelected == false)
        {
            InitializeWithObject(path);
            buildingSelected = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && buildingSelected == false)
        {
            InitializeWithObject(smallHouse);
            buildingSelected = true;
        }

        else if (Input.GetKeyDown(KeyCode.Alpha3) && buildingSelected == false)
        {
            InitializeWithObject(largeHouse);
            buildingSelected = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && buildingSelected == false)
        {
            InitializeWithObject(house);
            buildingSelected = true;
        }
    }



    public static Vector3 GetMouseWorldPosition()                     //determine position of the mouse
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 50,3))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellpos = gridLayout.WorldToCell(position);   //get the cell in which the position lays
        position = grid.GetCellCenterWorld(cellpos);

        return position;
    }

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }
        return array;
    }

    #endregion

    #region Building Placement

    public void InitializeWithObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, SnapCoordinateToGrid(GetMouseWorldPosition()), Quaternion.identity);

        objectToPlace = obj.GetComponent<PlaceableObject>();
        obj.AddComponent<ObjectDrag>();
    }

    private bool CanBePlaced(PlaceableObject placeableObject)
    {
        BoundsInt area = new BoundsInt();
        area.position = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        area.size = new Vector3Int(objectToPlace.Size.x +1, objectToPlace.Size.y +1, area.size.z + 1);
        
        TileBase[] baseArray = GetTilesBlock(area, MainTilemap);

        foreach (var b in baseArray)
        {
            if(b == whiteTile)
            {
                return false;
            }
        };
        return true;
    }

    public void TakeArea(Vector3Int start, Vector3Int size)
    {
  
        MainTilemap.BoxFill(start, whiteTile, start.x, start.y, 
                            start.x + size.x, start.y + size.y); 
       /*
        Vector3Int[] cords;
        cords = new Vector3Int[101];

        int trueSizeX = size.x ;
        int trueSizeY = size.y ;


        for (int i = 0; i <= trueSizeX * trueSizeY; i++)
        {
            if(i <= trueSizeY)
            {
                cords[i] = new Vector3Int(start.x, start.y + (trueSizeY - i), 0);
                MainTilemap.SetTile(cords[i], whiteTile);
            }
            if( i <= trueSizeY * 2 && i > trueSizeY)
            {
                cords[i] = new Vector3Int(start.x + 1, start.y + trueSizeY - i + trueSizeY, 0);
                MainTilemap.SetTile(cords[i], whiteTile);
            }
            if (i <= trueSizeY * 3 && i > trueSizeY * 2)
            {
              cords[i] = new Vector3Int(start.x + 2, start.y + trueSizeY - i + trueSizeY * 2, 0);
              MainTilemap.SetTile(cords[i], whiteTile);
            }
            if (i <= trueSizeY * 4 && i > trueSizeY * 3)
            {
                cords[i] = new Vector3Int(start.x + 3, start.y + trueSizeY - i + trueSizeY * 3, 0);
                MainTilemap.SetTile(cords[i], whiteTile);
            }
            if (i <= trueSizeY * 5 && i > trueSizeY * 4)
            {
                cords[i] = new Vector3Int(start.x + 4, start.y + trueSizeY - i + trueSizeY * 4, 0);
                MainTilemap.SetTile(cords[i], whiteTile);
            }
            if (i <= trueSizeY * 6 && i > trueSizeY * 5)
            {
                cords[i] = new Vector3Int(start.x + 5, start.y + trueSizeY - i + trueSizeY * 5, 0);
                MainTilemap.SetTile(cords[i], whiteTile);
            }
            if (i <= trueSizeY * 7 && i > trueSizeY * 6)
            {
                cords[i] = new Vector3Int(start.x + 6, start.y + trueSizeY - i + trueSizeY * 6, 0);
                MainTilemap.SetTile(cords[i], whiteTile);
            }
            if (i <= trueSizeY * 8 && i > trueSizeY * 7)
            {
                cords[i] = new Vector3Int(start.x + 7, start.y + trueSizeY - i + trueSizeY * 7, 0);
                MainTilemap.SetTile(cords[i], whiteTile);
            }
            if (i <= trueSizeY * 9 && i > trueSizeY * 8)
            {
                cords[i] = new Vector3Int(start.x + 8, start.y + trueSizeY - i + trueSizeY * 8, 0);
                MainTilemap.SetTile(cords[i], whiteTile);
            }
            if (i <= trueSizeY * 10 && i > trueSizeY * 9)
            {
                cords[i] = new Vector3Int(start.x + 9, start.y + trueSizeY - i + trueSizeY * 9, 0);
                MainTilemap.SetTile(cords[i], whiteTile);
            }
            if (i <= trueSizeY * 11 && i > trueSizeY * 10)
            {
                cords[i] = new Vector3Int(start.x + 10, start.y + trueSizeY - i + trueSizeY * 10, 0);
                MainTilemap.SetTile(cords[i], whiteTile);
            }
        }*/
    }
    #endregion
}
