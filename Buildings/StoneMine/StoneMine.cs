using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneMine : MonoBehaviour
{
    ResourceManager resourceManager;

    // Start is called before the first frame update
    void Start()
    {
        resourceManager = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();
        StartCoroutine(StoneCollecting());
    }


    IEnumerator StoneCollecting()
    {
        yield return new WaitForSeconds(30f);
        resourceManager.AddResource("stone", Random.Range(1, 2));
        StartCoroutine(StoneCollecting());
    }
}
