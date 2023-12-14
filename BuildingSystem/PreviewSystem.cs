using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    private float previewXOffset;
    private float previewZOffset;

    private float indicatorXOffset;
    private float indicatorZOffset;

    private float gridCellSize;
    ObjectCheck objectCheck;
    BuildingPreviewCheck previewCheck;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    [SerializeField]
    private Grid grid;

    private Renderer cellIndicatorRenderer;


    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        cellIndicator.SetActive(false);
        gridCellSize = grid.cellSize.x; //cell sizes are all the same thus does not matter if get x or z
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size, int rotationState)
    {
        cellIndicator.SetActive(true);
        previewObject = Instantiate(prefab);
        objectCheck = previewObject.GetComponentInChildren<ObjectCheck>();
        previewCheck = previewObject.GetComponent<BuildingPreviewCheck>();
        previewCheck.isPreview = true;
        PreparePreview(previewObject, rotationState, size);
        PrepareCursor(size, rotationState);
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one, 0);
    }

    private void PrepareCursor(Vector2Int size, int rotationState)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);//increase scale cell indicator to scale object
            //cellIndicatorRenderer.material.mainTextureScale = size; //increase tiling of texture
        }

        if (size.x == size.y)//if object is equally sided on every side than dont do anything bacause object rotates but occupied tiles thus cellindicator stays fixed
        {
            cellIndicator.transform.rotation = Quaternion.Euler(0,0,0);
            indicatorXOffset = 0;
            indicatorZOffset = 0;
        }
        else if (size.x != size.y)//rotate with small position change to let it look better
        {
            if (rotationState == 0)
            {
                cellIndicator.transform.rotation = Quaternion.Euler(0, 0, 0);
                indicatorXOffset = 0;
                indicatorZOffset = 0;

            }
            else if (rotationState == 1)
            {
                cellIndicator.transform.rotation = Quaternion.Euler(0, 90, 0);
                indicatorXOffset = 0;
                indicatorZOffset = gridCellSize;
            }
            else if (rotationState == 2)
            {
                cellIndicator.transform.rotation = Quaternion.Euler(0, 180, 0);
                indicatorXOffset = gridCellSize;
                indicatorZOffset = gridCellSize;
            }
            else if (rotationState == 3)
            {
                cellIndicator.transform.rotation = Quaternion.Euler(0, 270, 0);
                indicatorXOffset = gridCellSize;
                indicatorZOffset = 0;
            }
            else
                throw new Exception("Out of bounds RotationState");
        }
    }

    private void PreparePreview(GameObject previewObject, int rotationState ,Vector2Int size)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();//get al renderers
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;//get all materials
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;//swap all materials to preview(transparant) materials
            }
            renderer.materials = materials;
        }
        if(size.x == size.y)//if object is equally sided on every side than rotate object without changing the position
        {
            if (rotationState == 0)
            {
                previewObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                previewXOffset = 0;
                previewZOffset = 0;

            }
            else if (rotationState == 1)
            {
                previewObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                previewXOffset = 0;
                previewZOffset = gridCellSize * size.y;
            }
            else if (rotationState == 2)
            {
                previewObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                previewXOffset = gridCellSize * size.x;
                previewZOffset = gridCellSize * size.y;
            }
            else if (rotationState == 3)
            {
                previewObject.transform.rotation = Quaternion.Euler(0, 270, 0);
                previewXOffset = gridCellSize * size.x;
                previewZOffset = 0;
            }
            else
                throw new Exception("Out of bounds RotationState");
        }
        else//rotate with small position change to let it look better
        {
            if (rotationState == 0)
            {
                previewObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                previewXOffset = 0;
                previewZOffset = 0;

            }
            else if (rotationState == 1)
            {
                previewObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                previewXOffset = 0;
                previewZOffset = gridCellSize;
            }
            else if (rotationState == 2)
            {
                previewObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                previewXOffset = gridCellSize;
                previewZOffset = gridCellSize;
            }
            else if (rotationState == 3)
            {
                previewObject.transform.rotation = Quaternion.Euler(0, 270, 0);
                previewXOffset = gridCellSize;
                previewZOffset = 0;
            }
            else
                throw new Exception("Out of bounds RotationState");
        }
    }
   
    public bool checkMountainCollision()
    {
        return objectCheck.GetMountainValidity();
    }

    public bool checkInfrastructureValidity()
    {
        return objectCheck.GetInfrastructureValidity();
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
    }
    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (previewObject != null)//only run if previewobject is spawned
        {
            MovePreview(position);//if there is a preview object thus object is not yet placed, then MovePreview and add preview material
            ApplyFeedbackToPreview(validity);
        }
        ApplyFeedbackToCursor(validity);//always update cursor
        MoveCursor(position);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;//if valid then white preview color
        
        c.a = 0.5f;//transparancy
        previewMaterialInstance.color = c;
    }
    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;//if valid than white cursor color

        c.a = 0.5f;//transparancy
        cellIndicatorRenderer.material.color = c;
    }


    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = new Vector3(
         position.x + indicatorXOffset,
         position.y,
         position.z + indicatorZOffset); ;
    }

    private void MovePreview(Vector3 position)
    {
         previewObject.transform.position = new Vector3(
         position.x + previewXOffset,
         position.y, 
         position.z + previewZOffset);     
    }

    internal void UpdateBuildingRemovePreview(Vector2Int size, Vector3 position, int rotationState)
    {
        cellIndicator.SetActive(true);
        PrepareCursor(size, rotationState);
        MoveCursor(position);
    }
}
