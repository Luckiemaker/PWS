using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public enum DrawMode { NoiseMap, ColourMap, MeshTextures, FalloffMap };
	public DrawMode drawMode;

	//public Noise.NormalizeMode normalizeMode;
	public Noise.MapPosition mapPosition;

	TerrainData terrainData;

	public bool autoUpdate;

    Color[] ColourMap;
	int mapChunkSize;
	BiomeGenerator biomeGenerator;
	MapGeneratorController mapGeneratorController;


    public void GenerateMap()//gets called trough broadcast
	{
		mapGeneratorController = GetComponentInParent<MapGeneratorController>();
		mapChunkSize = mapGeneratorController.GetMapChunkSize();
		biomeGenerator = GetComponentInParent<BiomeGenerator>();

		terrainData = mapGeneratorController.GetTerrainData();

		float[,] noiseMap = mapGeneratorController.GetNoiseMap();//large noisemap of whole map
		Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
		float[,] NoiseMap = new float[mapChunkSize, mapChunkSize];//splitted chunk map


		//Split noisemap into 4 smaller noismaps because of the limiting of vertices in a mesh.
		if (mapPosition == Noise.MapPosition.LeftDown)
		{
			for (int y = 0; y < mapChunkSize; y++)
			{
				for (int x = 0; x < mapChunkSize; x++)
				{
					NoiseMap[x, y] = noiseMap[x, mapChunkSize + y -1];
				}
			}
		}
		else if (mapPosition == Noise.MapPosition.RightDown)
		{
			for (int y = 0; y < mapChunkSize; y++)
			{
				for (int x = 0; x < mapChunkSize; x++)
				{
					NoiseMap[x, y] = noiseMap[mapChunkSize + x -1, mapChunkSize + y -1];
				}
			}
		}
		else if (mapPosition == Noise.MapPosition.LeftUp)
		{
			for (int y = 0; y < mapChunkSize; y++)
			{
				for (int x = 0; x < mapChunkSize; x++)
				{
					NoiseMap[x, y] = noiseMap[x, y];
				}
			}
		}
		else if (mapPosition == Noise.MapPosition.RightUp)//last called mesh generations --> importand for mapdisplay code!
		{
			for (int y = 0; y < mapChunkSize; y++)
			{
				for (int x = 0; x < mapChunkSize; x++)
				{
					NoiseMap[x, y] = noiseMap[mapChunkSize + x -1, y];
      			}
			}
		}

		DrawInEditor(NoiseMap, ColourMap, noiseMap);
	}



	void DrawInEditor(float[,] noiseMap, Color[] colourMap, float[,] fullNoiseMap)
    {
		MapDisplay display = transform.gameObject.GetComponent<MapDisplay>();

		if (drawMode == DrawMode.NoiseMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(fullNoiseMap));
		}
		else if (drawMode == DrawMode.ColourMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
		}
		else if (drawMode == DrawMode.MeshTextures)
		{
			display.DrawMeshTextures(MeshGenerator.GenerateTerrainMesh(noiseMap, 
				terrainData.meshHeightMultiplier, 
				terrainData.meshHeightCurve, 
				terrainData.useFlatShading, 
				biomeGenerator.GetBiomeTextureMap(mapPosition, 0), 
				biomeGenerator.GetBiomeTextureMap(mapPosition, 1), 
				biomeGenerator.GetBiomeTextureMap(mapPosition, 2)), 
				mapPosition);
		}
		else if (drawMode == DrawMode.FalloffMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallOffGenerator.GenerateFalloffMap(mapChunkSize, terrainData.falloffStart, terrainData.falloffEnd, terrainData.useCircularFalloff)));
		}
	}
}
