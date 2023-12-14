using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneDeposit : ObjectWithBuildingSpots
{
    private PlacementSystem _placementSystem;
    // Start is called before the first frame update
    void Start()
    {
        _placementSystem = GameObject.Find("BuildingSystem").GetComponentInChildren<PlacementSystem>();

        SetData(_placementSystem.GetStoneData(), _placementSystem);
        CalculateOccupybleTiles();
    }
}
