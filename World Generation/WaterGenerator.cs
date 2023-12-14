using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGenerator : MonoBehaviour
{
    public Mesh waterMeshData;

    MapDisplay oceanMapDisplay;
    public Mesh GenerateOceanMesh()
    {
        return waterMeshData;
    }

    public void GenerateWaters(float[,] noiseMap)
    {
        oceanMapDisplay = GetComponent<MapDisplay>();

        //generate ocean
        Mesh mesh = GenerateOceanMesh();
        

        oceanMapDisplay.DrawMeshTexturesFromMesh(mesh);

        //generate lakes
    }
}
