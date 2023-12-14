using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumberjack : MonoBehaviour
{
    private bool treeFound = false;
    SphereCollider sphereCollider;
    ResourceManager resourceManager;
    bool isPreview;
    bool chopTree;

    public float startRadius = 0.4f;
    float radius;
    public GameObject TreeSpawnPointObject;

    [SerializeField]
    int newTreePlantCouldown;
    [SerializeField]
    int chopTreeCouldown;

    [SerializeField]
    GameObject oakTree1;
    [SerializeField]
    GameObject oakTree2;

    [SerializeField]
    GameObject pineTree1;
    [SerializeField]
    GameObject pineTree2;

    BiomeObjectsHandler handler;
    public BiomeData biomeData;

    private int biomeIndex;
    private int objectIndex;

    private void Start()
    {
        sphereCollider = this.GetComponent<SphereCollider>();
        BuildingPreviewCheck buildingPreviewCheck = transform.GetComponentInParent<BuildingPreviewCheck>();
        resourceManager = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();
        handler = GameObject.Find("WorldGenerator").GetComponent<BiomeObjectsHandler>();

        isPreview = buildingPreviewCheck.GetPreviewState();
        sphereCollider.enabled = false;
        radius = startRadius;
        chopTree = false;

        if (!isPreview)
        {
            sphereCollider.enabled = true;
            sphereCollider.radius = startRadius;
        }

        StartCoroutine(ChopTreeCouldown());//at the beginning initiate first coldown to activate the loop which lasts troughout the whole game
    }

    private void Update()
    {
        if(chopTree)//when no tree is found keep searching
        {
            if (!isPreview)//When the building is no preview but is already placed and thus in action
            {
                FindTree();
            }
        }
    }

    public void FindTree()
    { 
        radius += 0.02f;
        sphereCollider.radius = radius;

        if (treeFound)//reset search radius after tree is found and start couldown
        {
            treeFound = false;
            chopTree = false;
            radius = startRadius;
            sphereCollider.radius = startRadius;
            StartCoroutine(ChopTreeCouldown());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Tree"))//only when collided with a tree execute
        {
            treeFound = true;

            GameObject obj = other.gameObject;
            ChopTree(obj);
        }
    }

    public void ChopTree(GameObject obj)
    {        
        Vector3 pos = obj.gameObject.transform.position;
        GameObject newTree = TreeSelector(obj);

        handler.AddObjectToDestroyedBiomesObjectsList(new BiomeObjectData(pos));//add to destroyed list for worldgeneration
        Destroy(obj);//destroy chopped tree

        int rotationDiff = Random.Range(0, 360);
        GameObject stumpobj = Instantiate(TreeSpawnPointObject, pos, Quaternion.Euler(0, rotationDiff, 0));//add object to grow new tree on chopped spot

        int index = handler.GetNewSapplingIndex();//get unique index for this sappling
        SapplingData data = new SapplingData(newTree.name, pos, 0);
        TreeGrowth script = stumpobj.GetComponent<TreeGrowth>();

        script.SetIndex(index);
        handler.AddObjectToSapplingsList(index, data);//add sapplingdata to list for generation purposes
        script.SetTreeToSpawn(newTree);
        script.StartGrowth(0);

        resourceManager.AddResource("wood", Random.Range(1, 3));//add resources
    }


    public GameObject TreeSelector(GameObject obj)
    {
        if (obj.name == "Oak_Tree_01(Clone)")
        {
            biomeIndex = 0;
            objectIndex = 0;
            return biomeData.biomes[biomeIndex].objects[objectIndex].Object;
        }
        else if (obj.name == "Oak_Tree_02(Clone)")
        {
            biomeIndex = 0;
            objectIndex = 1;
            return biomeData.biomes[biomeIndex].objects[objectIndex].Object;
        }
        else if (obj.name == "Pine_Tree_01(Clone)")
        {
            biomeIndex = 1;
            objectIndex = 0;
            return biomeData.biomes[biomeIndex].objects[objectIndex].Object;
        }
        else if (obj.name == "Pine_Tree_02(Clone)")
        {
            biomeIndex = 1;
            objectIndex = 1;
            return biomeData.biomes[biomeIndex].objects[objectIndex].Object;
        }
        else
            throw new System.Exception($"No tree of type {obj}");
    }

    IEnumerator ChopTreeCouldown()
    {
        yield return new WaitForSeconds(chopTreeCouldown);
        chopTree = true; //after couldown a new tree can be chopped thus reset bool to continue searching new tree
    }
}
