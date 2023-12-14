using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class StructureData : UpdatableData
{
    public structureType[] Structures;
    public float structureMinHeight;
    public float structureMaxHeight;
}

[System.Serializable]
public struct structureType{
    public string Name;
    public int SpawnTimes;
    public bool CentreToGridPos;
    public GameObject SpawnObject;

}
