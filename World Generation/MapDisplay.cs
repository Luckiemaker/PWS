using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
	public MapType type;

    [Header("For TextureMap")]
    public Renderer textureRender;

	[Header("For MeshMap")]
	public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

	MapGeneratorController mapGeneratorController;

    private void Start()
    {
		//mapGeneratorController = gameObject.GetComponentInParent<MapGeneratorController>();
		
		if(type == MapType.MeshMap)//at the begin set mesh to zero to prevent confusion for biome generator which is going to generate based on existing objects
        {
			meshFilter.sharedMesh = null;
			meshCollider.sharedMesh = null;
		}	
    }

	public void EditorMeshNullState()//will only be called at the start when generating in editor. in runtime the start function will call this
    {
		if(type == MapType.MeshMap)//at the begin set mesh to zero to prevent confusion for biome generator which is going to generate based on existing objects
		{
			meshFilter.sharedMesh = null;
			meshCollider.sharedMesh = null;
		}
	}

    public void DrawTexture(Texture2D texture)
    {
		textureRender.sharedMaterial.mainTexture = texture;
    }

	public void DrawMeshTextures(MeshData meshData, Noise.MapPosition position)
	{
		mapGeneratorController = gameObject.GetComponentInParent<MapGeneratorController>();

		meshFilter.sharedMesh = meshData.CreateMesh();
		meshCollider.sharedMesh = meshData.CreateMesh();

		if (position == Noise.MapPosition.RightUp) //only called once // THE MAPPOSITIONS SHOULD COMPARED TO THE LAST CALLED ONE SO THAT THE ENTIRE MESH IS GENERATED BEFORE THE finazileworldgen method is called
        {
			if (mapGeneratorController != null)
			{
				mapGeneratorController.FinalizeWorldGen();
			}
			else
				Debug.LogError("mapGeneratorController is null");
		}
	}


	public enum MapType {TextureMap, MeshMap}



	public void DrawMeshTexturesFromMesh(Mesh mesh) //NOTING IMPORT EXPIRIMENTAL THINGY FOR OCEAAN GEN
	{
		meshFilter.sharedMesh = mesh;
		meshCollider.sharedMesh = mesh;
	}
}
