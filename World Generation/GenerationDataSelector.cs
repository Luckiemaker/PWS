using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationDataSelector : MonoBehaviour
{
    public enum GenerationMode { Default, Islands }

    public GenerationData Default;
    public GenerationData Islands;

    public GenerationData GetGenerationData(GenerationMode generationMode)
    {
        if (generationMode == GenerationMode.Default)
        {
            return Default;
        }

        if (generationMode == GenerationMode.Islands)
        {
            return Islands;
        }
       
        Debug.LogError("No GenerationMode set -> selected Default mode");
        return Default;
    }

    [System.Serializable]
    public struct GenerationData
    {
        public NoiseData noiseData;
        public TerrainData terrainData;
    }
}
