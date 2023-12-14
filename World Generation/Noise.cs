using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = System.Random;


public static class Noise
{
	public enum NormalizeMode {Local,Global,GlobalPerformance}
	public enum MapPosition {LeftDown,RightDown,LeftUp,RightUp }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float persistanceMultiplierMountain, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
	{
		
		int TotalWorldWidth = mapWidth * 2;
		

		float[,] noiseMap = new float[TotalWorldWidth, TotalWorldWidth];

		Random prng = new Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];

		float maxPossibleHeight = 0;
		float amplitude = 1;

		for (int i = 0; i < octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= persistance;
		}

		if (scale <= 0)//Debug purpose
		{
			scale = 0.0001f;
		}

		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		float halfWidth = TotalWorldWidth / 2f;
		float halfHeight = TotalWorldWidth / 2f;



		for (int y = 0; y < TotalWorldWidth; y++)
		{
			for (int x = 0; x < TotalWorldWidth; x++)
			{
				amplitude = 1;
				float frequency = 1;

				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++)
				{
					//higher frequency makes points furhter apart
					float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
					float sampleY = (y - halfHeight - octaveOffsets[i].y) / scale * frequency;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

					noiseHeight += perlinValue * amplitude;

					//amplitude decreases over time with persistance


					amplitude *= persistance;
					frequency *= lacunarity;
				}


				if (noiseHeight > maxLocalNoiseHeight)
				{
					maxLocalNoiseHeight = noiseHeight;
				}
				else if (noiseHeight < minLocalNoiseHeight)
				{
					minLocalNoiseHeight = noiseHeight;
				}

				noiseMap[x, y] = noiseHeight;
			}
		}


		//Xtra mountain percistance
		for (int y = 0; y < TotalWorldWidth; y++)
		{
			for (int x = 0; x < TotalWorldWidth; x++)
			{
				if (noiseMap[x, y] > 0.3f)
				{
					amplitude = 1;
					float frequency = 1;

					float noiseHeight = 0;

					for (int i = 0; i < octaves; i++)
					{
						//higher frequency makes points furhter apart
						float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
						float sampleY = (y - halfHeight - octaveOffsets[i].y) / scale * frequency;

						float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
						noiseHeight += perlinValue * amplitude;

						//amplitude decreases over time with persistance
						amplitude *= persistance * persistanceMultiplierMountain;
						frequency *= lacunarity;
					}
					if (noiseHeight > maxLocalNoiseHeight)
					{
						maxLocalNoiseHeight = noiseHeight;
					}
					else if (noiseHeight < minLocalNoiseHeight)
					{
						minLocalNoiseHeight = noiseHeight;
					}

					noiseMap[x, y] = noiseHeight;
				}
			}
		}


		//nomralize values between 0 and 1
		if (normalizeMode == NormalizeMode.Local) //only When normalizemode is local because if global we use finalizeNoisemapMethod
		{
			for (int y = 0; y < TotalWorldWidth; y++)
			{
				for (int x = 0; x < TotalWorldWidth; x++)
				{
					noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
				}
			}
		}
		else if (normalizeMode == NormalizeMode.GlobalPerformance)
		{
			for (int y = 0; y < TotalWorldWidth; y++)
			{
				for (int x = 0; x < TotalWorldWidth; x++)
				{
					float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
					noiseMap[x, y] = normalizedHeight;
				}
			}
		}

	
	
		return noiseMap; 



		/*
		float[,] noiseMap = new float[mapWidth, mapHeight];

		Random prng = new Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];

		float maxPossibleHeight = 0;
		float amplitude = 1;

		for (int i = 0; i < octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= persistance;
		}

		if (scale <= 0)
		{
			scale = 0.0001f;
		}

		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;



        for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				 amplitude = 1;
				 float frequency = baseFrequency;
                
                float noiseHeight = 0;

				for (int i = 0; i < octaves; i++)
				{
					//higher frequency makes points furhter apart
					float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
					float sampleY = (y - halfHeight - octaveOffsets[i].y) / scale * frequency;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

					noiseHeight += perlinValue * amplitude;

					//amplitude decreases over time with persistance


					amplitude *= persistance;
					frequency *= lacunarity;
				}
                

				if (noiseHeight > maxLocalNoiseHeight)
				{
					maxLocalNoiseHeight = noiseHeight;
				}
				else if (noiseHeight < minLocalNoiseHeight)
				{
					minLocalNoiseHeight = noiseHeight;
				}

				noiseMap[x, y] = noiseHeight;
            }
		}


		//Xtra mountain percistance
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{				
				if (noiseMap[x, y] > 0.3f)
				{
					amplitude = 1;
					float frequency = baseFrequency;

					float noiseHeight = 0;

					for (int i = 0; i < octaves; i++)
					{
						//higher frequency makes points furhter apart
						float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
						float sampleY = (y - halfHeight - octaveOffsets[i].y) / scale * frequency;

						float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
						noiseHeight += perlinValue * amplitude;

						//amplitude decreases over time with persistance
						amplitude *= persistance * persistanceMultiplierMountain;
						frequency *= lacunarity;
					}
					noiseMap[x, y] = noiseHeight;
				}
			}
		}

		//nomralize values between 0 and 1
		if (normalizeMode == NormalizeMode.Local) //only When normalizemode is local because if global we use finalizeNoisemapMethod
		{
			for (int y = 0; y < mapHeight; y++)
			{
				for (int x = 0; x < mapWidth; x++)
				{
					noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
				}
			}
		} else if (normalizeMode == NormalizeMode.GlobalPerformance)
        {
			for (int y = 0; y < mapHeight; y++)
			{
				for (int x = 0; x < mapWidth; x++)
				{
					float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
					noiseMap[x, y] = normalizedHeight;
				}
			}
		}
		return noiseMap; */
	}


	public static float[,] FinalizeNoiseMap(float min, float max, float[,] noisemap, float mapSize)
    {
		for (int y = 0; y < mapSize; y++)
		{
			for (int x = 0; x < mapSize; x++)
			{
				noisemap[x, y] = Mathf.InverseLerp(min, max, noisemap[x, y]);
			}
		}
		return noisemap;
    }
}
