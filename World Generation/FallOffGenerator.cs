using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FallOffGenerator
{
    
	public static float[,] GenerateFalloffMap(int size, float falloffStart, float falloffEnd, bool circularFalloff)//, Noise.MapPosition mapPositions)
	{
		int totalWorldSize = size * 2;

		float[,] heightMap = new float[totalWorldSize , totalWorldSize]; //*2 because of the map is made up of 4 tiles so falloof map must cover all these tiles
		
		for (int i = 0; i < totalWorldSize; i++)
		{
			for (int j = 0; j < totalWorldSize; j++)
			{

				if (circularFalloff)
				{
					float rInner = falloffStart * 108;
					float rOuter = falloffEnd * 108;

					//debug purpose
					if (rInner == rOuter)
					{
						rInner = rInner + 1;
					}

					float cX = totalWorldSize / 2;//rop right coordinates
					float cY = totalWorldSize / 2;

					float dx = cX - i;
					float dy = cY - j;

					float d;

					d = Mathf.Sqrt(dx * dx + dy * dy);

					heightMap[i, j] = Mathf.InverseLerp(rInner, rOuter, d);
				}
				else if (circularFalloff == false)
				{
					float x = i / (float)totalWorldSize *2 - 1; //gives values between -1 and 1 for coordinates
					float y = j / (float)totalWorldSize *2  - 1;



					//find which value is closer to the edge//
					float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

					heightMap[i, j] = Mathf.SmoothStep(0,1, Mathf.InverseLerp(falloffStart, falloffEnd, value));

				}			
			}
		}




		/*
		//now devide falloofheightmap in number of tiles
		float[,] fallOffMap = new float[sizePerMap, sizePerMap];
		if (mapPositions == Noise.MapPosition.LeftUp)
		{
			for (int y = 0; y < sizePerMap; y++)
			{
				for (int x = 0; x < sizePerMap; x++)
				{
					fallOffMap[x, y] = heightMap[x, y];
				}
			}
		}else if (mapPositions == Noise.MapPosition.RightUp)
		{
			for (int y = 0; y < sizePerMap; y++)
			{
				for (int x = 0; x < sizePerMap; x++)
				{
					fallOffMap[x, y] = heightMap[sizePerMap + x, y];
				}
			}
		}else if (mapPositions == Noise.MapPosition.LeftDown)
		{
			for (int y = 0; y < sizePerMap; y++)
			{
				for (int x = 0; x < sizePerMap; x++)
				{
					fallOffMap[x, y] = heightMap[x,sizePerMap + y];
				}
			}
		} else if (mapPositions == Noise.MapPosition.RightDown)
		{
			for (int y = 0; y < sizePerMap; y++)
			{
				for (int x = 0; x < sizePerMap; x++)
				{
					fallOffMap[x, y] = heightMap[sizePerMap + x, sizePerMap + y];
				}
			}
		}*/
		return heightMap;
	}
}
