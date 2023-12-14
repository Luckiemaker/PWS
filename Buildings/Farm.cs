using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour
{
    ResourceManager resourceManager;
    void Start()
    {

        resourceManager = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();
        StartCoroutine(FoodCollecting());
    }


    IEnumerator FoodCollecting()
    {
        yield return new WaitForSeconds(30f);
        resourceManager.AddResource("food", Random.Range(1, 2));
        StartCoroutine(FoodCollecting());
    }
}
