using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableData
{
	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;
	public bool useFlatShading;

	[Range(0, 1)]
	public float falloffStart;
	[Range(0, 2)]
	public float falloffEnd;
	public bool useFalloff;
	public bool useCircularFalloff;

	public float minHeight
	{
		get
		{
			return meshHeightMultiplier * meshHeightCurve.Evaluate(0);
		}
	}

	public float maxHeight
	{
		get
		{
			return meshHeightMultiplier * meshHeightCurve.Evaluate(1);
		}
	}
}

