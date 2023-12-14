using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGrowth : MonoBehaviour, IDataPersistance
{
    public GameObject stump;

    private Vector3 defaultScale;
    private int growthStage;
    private GameObject newTree;

    private int index;

    private BiomeObjectsHandler handler;


    void Start()
    {
        handler = GameObject.Find("WorldGenerator").GetComponent<BiomeObjectsHandler>();
    }

    public void SetIndex(int i)
    {
        index = i;
    }

    public void SetTreeToSpawn(GameObject obj)
    {
        newTree = obj;
        this.name = obj.name;
    }

    public void StartGrowth(int stage)
    {       
        growthStage = stage;
        StartCoroutine(GrowthProccess());
    }

    IEnumerator GrowthProccess()
    {
        GameObject tree = Instantiate(newTree, this.transform.position, Quaternion.identity);//spawn small sappling
        tree.transform.SetParent(this.transform);
        tree.tag = "Untagged";//set untagged so that lumberjack wont cut it
        defaultScale = tree.transform.localScale;//set default scale of object

        if (growthStage == 0)//spawn stump
        {
            tree.transform.localScale = new Vector3(0,0,0);//set scale of sappling(this object) to zero

            GameObject obj = Instantiate(stump, this.transform.position, Quaternion.identity);
            obj.transform.SetParent(this.transform);

            yield return new WaitForSeconds(30 + Random.Range(0, 15));
            growthStage = 1;

            SapplingData data = new(this.name, transform.position, growthStage);
            handler.UpdateSapplingData(index, data);//update data which will later be saved -> growthstage
            Destroy(obj); //destroy stump
        }

        if (growthStage == 1)
        {
            tree.transform.localScale = defaultScale / 5;
            yield return new WaitForSeconds(30 + Random.Range(0, 15));
            tree.transform.localScale = defaultScale / 2.5f;

            growthStage = 2;
            SapplingData data = new(this.name, transform.position, growthStage);
            handler.UpdateSapplingData(index, data);
        }
        
        if(growthStage == 2)
        {
            tree.transform.localScale = defaultScale / 2.5f;
            yield return new WaitForSeconds(30 + Random.Range(0, 15));
            tree.transform.localScale = defaultScale / 1.333f;

            growthStage = 3;
            SapplingData data = new(this.name, transform.position, growthStage);
            handler.UpdateSapplingData(index, data);
        }
       
        if(growthStage == 3)
        {
            tree.transform.localScale = defaultScale / 1.333f;
            yield return new WaitForSeconds(30 + Random.Range(0, 15));
            tree.transform.localScale = defaultScale;
            tree.tag = "Tree";//now lumberjack will be able to chop the tree

            growthStage = 4;
            SapplingData data = new(this.name, transform.position, growthStage);
            handler.UpdateSapplingData(index, data);
        }
        
        if(growthStage == 4)
        {
            tree.transform.localScale = defaultScale;
            tree.tag = "Tree";//now lumberjack will be able to chop the tree
            handler.UpdateSapplingData(index, null);
            handler.RemoveObjectToDestroyedBiomesObjectsList(new BiomeObjectData(transform.position));
        }
    }

    public void LoadData(GameData data)
    {
        throw new System.NotImplementedException();
    }

    public void SaveData(GameData data)
    {
        throw new System.NotImplementedException();
    }
}
