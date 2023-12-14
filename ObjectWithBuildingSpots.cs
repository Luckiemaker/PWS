using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWithBuildingSpots : MonoBehaviour, IDataPersistance
{
    [SerializeField]
    private int sizeX;
    [SerializeField]
    private int sizeY;
    [SerializeField]
    private int numberOfLocations;
    [SerializeField]
    Vector3 objectSizeTiles;
    [SerializeField]
    private GameObject locationVisualisationTile;

    private int seed;

    private Grid grid;
    private GameObject tileParent;
    private GridData gridData;

    public void SetData(GridData Data, PlacementSystem placementSystem)
    {
        grid = placementSystem.GetGrid();
        tileParent = placementSystem.GetStoneTileVis();
        tileParent.SetActive(false);
        gridData = Data;
    }

    //structures should be centered on a grid tile and is centered in structure generator
    public void CalculateOccupybleTiles()
    {
        Random.InitState(seed);

        Vector3 cellSize = grid.cellSize;

        bool oneIsCalled = false;
        bool twoIsCalled = false;
        bool threeIsCalled = false;
        bool fourIsCalled = false;

        Vector3 gapSize = objectSizeTiles;

        for (int b = 0; b < numberOfLocations; b++)
        {
            int x;
            int y;

            int randomLocations = Random.Range(1, 5);  //1:++, 2:+-, 3:-+, 4:--

            if (randomLocations == 1 && !oneIsCalled)
            {
                oneIsCalled = true;
                gapSize.x = objectSizeTiles.x;
                gapSize.y = 0;
            }
            else if (randomLocations == 2 && !twoIsCalled)
            {
                twoIsCalled = true;
                gapSize.x = 0;
                gapSize.y = -1 * objectSizeTiles.y;
            }
            else if (randomLocations == 3 && !threeIsCalled)
            {
                threeIsCalled = true;
                gapSize.x = 0;
                gapSize.y = objectSizeTiles.y;

            }
            else if (randomLocations == 4 && !fourIsCalled)
            {
                fourIsCalled = true;
                gapSize.x = objectSizeTiles.x * -1;
                gapSize.y = 0;
            }
            else
            {
                b--;
                continue;
            }

            for (int i = 0; i < sizeX; i++)
            {
                x = i;
                if (randomLocations == 3 || randomLocations == 4)
                {
                    x *= -1;
                }

                for (int j = 0; j < sizeY; j++)
                {
                    y = j;
                    if (randomLocations == 2 || randomLocations == 4)
                    {
                        y *= -1;
                    }
                    Vector3 pos = this.transform.position + new Vector3(
                        gapSize.x * cellSize.x + cellSize.x * x,
                        0,
                        gapSize.y * cellSize.y + cellSize.y * y); // translations

                    pos.y = 0.2931f; //HARDCODED
                    GameObject obj = Instantiate(locationVisualisationTile, pos, Quaternion.identity);
                    obj.transform.SetParent(tileParent.transform);
                    gridData.AddNewLocationSpecificTiles(grid.WorldToCell(pos));
                }
            }
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
