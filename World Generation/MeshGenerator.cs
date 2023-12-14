using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshGenerator : MonoBehaviour
{


	public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, bool useFlatShading, float[,] biomeTextureMap, float[,] biomeTextureMapForest, float[,] biomeTextureMapOak)
	{
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);
		float topLeftX = (width-1)  / -2f; //translate map coordinates to centered coordinates unity
		float topLeftZ = (height-1) / 2f;

		float textureUVs;
		float textureUVsForest;
		float textureUVsOak;

		//int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

		MeshData meshData = new MeshData(width, useFlatShading);
		int vertexIndex = 0;


		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				if (biomeTextureMap == null || biomeTextureMapForest == null || biomeTextureMapOak == null)
				{
					textureUVs = 0;
					textureUVsForest = 0;
					textureUVsOak = 0;
				}
				else
				{
					textureUVs = biomeTextureMap[x, y];
					textureUVsForest = biomeTextureMapForest[x, y];
					textureUVsOak = biomeTextureMapOak[x, y];
				}

				meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);//Add vertice
				meshData.uvs[vertexIndex] = new Vector2(0, heightMap[x, y]);//height UVs
				meshData.uvsTexture[vertexIndex] = new Vector2(textureUVsForest, textureUVs);//biomes UVs
				meshData.uvsTexture2[vertexIndex] = new Vector2(textureUVsOak, 0);//biome UVs



				if (x < width - 1 && y < height - 1)//check if edge coordinate
				{
					meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);//add triangles
					meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
				}
			    vertexIndex++;
			}
		}

		meshData.ProcessMesh();//apply flatshading
		return meshData;
	}
}



public class MeshData
{
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;
	public Vector2[] uvsTexture;
	public Vector2[] uvsTexture2;


	int triangleIndex;
	bool useFlatShading;

	public MeshData(int verticesPerLine, bool useFlatShading)
	{

		this.useFlatShading = useFlatShading;
		vertices = new Vector3[verticesPerLine * verticesPerLine];
		uvs = new Vector2[verticesPerLine * verticesPerLine];
		uvsTexture = new Vector2[verticesPerLine * verticesPerLine];
		uvsTexture2 = new Vector2[verticesPerLine * verticesPerLine];
		triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];
	}

	public void AddTriangle(int a, int b, int c)
	{
		triangles[triangleIndex] = a;
		triangles[triangleIndex + 1] = b;
		triangles[triangleIndex + 2] = c;
		triangleIndex += 3;
	}

	public void ProcessMesh()
    {
        if (useFlatShading)
        {
			FlatShading();
		}
	}


	void FlatShading()
	{

		Vector3[] flatShadedVertices = new Vector3[triangles.Length];
		Vector2[] flatShadedUvs = new Vector2[triangles.Length];
		Vector2[] flatShadedUvsTexture = new Vector2[triangles.Length];
		Vector2[] flatShadedUvsTexture2 = new Vector2[triangles.Length];


		for (int i = 0; i < triangles.Length; i++)
		{
			flatShadedVertices[i] = vertices[triangles[i]];
			flatShadedUvs[i] = uvs[triangles[i]];
			flatShadedUvsTexture[i] = uvsTexture[triangles[i]];
			flatShadedUvsTexture2[i] = uvsTexture2[triangles[i]];

			triangles[i] = i;
		}

		vertices = flatShadedVertices;
		uvs = flatShadedUvs;
		uvsTexture = flatShadedUvsTexture;
		uvsTexture2 = flatShadedUvsTexture2;
	}

	public Mesh CreateMesh()
	{
		Mesh mesh = new();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.uv2 = uvsTexture;
		mesh.uv3 = uvsTexture2;
		mesh.RecalculateNormals();
		return mesh;
	}

}
