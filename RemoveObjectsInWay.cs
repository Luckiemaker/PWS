using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveObjectsInWay : MonoBehaviour
{
    //simple script to removew all objects within a collider, give placed structures room
    int framCount;
    private void Start()
    {
        MeshCollider col = this.gameObject.AddComponent<MeshCollider>();
        col.convex = true;
        col.isTrigger = true;
    }

    private void Update()
    {
        framCount++;

        if(framCount == 60)//small timer for when script is removed based on framecounts to avoid it glitching while a player is lagging
        {
            RemoveScript();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("World"))
        {
            //do nothing
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("World"))
        {
            //do nothing
        }
        else
        {
            Destroy(other.gameObject);
        }
    }

    void RemoveScript()
    {
        Destroy(GetComponent<MeshCollider>());
        Destroy(GetComponent<RemoveObjectsInWay>());
    }
}
