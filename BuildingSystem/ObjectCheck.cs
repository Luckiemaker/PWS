using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCheck : MonoBehaviour//is placed on the building (preview) prefab
{
    bool buildingValidity = true;//checks if the building wil not be placed inside a mountain or tree or other obstacles
    bool infrastructureValidity = false;//checks if building will be placed within the infrastructure boundaries
    public bool isTownhall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("World"))
        {
            buildingValidity = false;
        }else if(other.gameObject.CompareTag("Tree"))
        {
            buildingValidity = false;
        }

        if (other.gameObject.CompareTag("Infrastructure"))
        {
            if (other.transform.parent.gameObject != this.transform.parent.gameObject)//when placing infrastucture is should only be placeable if within range of OTHER infrastucture boundaries, ohterwise you could place infrastucture freely all around the word
            {
                infrastructureValidity = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("World"))
        {
            buildingValidity = false;
        }
        else if (other.gameObject.CompareTag("Tree"))
        {
            buildingValidity = false;
        }

        if (other.gameObject.CompareTag("Infrastructure"))
        {
            if (other.transform.parent.gameObject != this.transform.parent.gameObject)//when placing infrastucture is should only be placeable if within range of OTHER infrastucture boundaries, ohterwise you could place infrastucture freely all around the word
            {
                infrastructureValidity = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("World"))
        {
            buildingValidity = true;
        }else if(other.gameObject.CompareTag("Tree"))
        {
            buildingValidity = true;
        }

        if (other.gameObject.CompareTag("Infrastructure"))
        {
            if (other.transform.parent.gameObject != this.transform.parent.gameObject)//when placing infrastucture is should only be placeable if within range of OTHER infrastucture boundaries, ohterwise you could place infrastucture freely all around the word
            {
                infrastructureValidity = false;
            }
        }
    }

    public bool GetMountainValidity()
    {
        return buildingValidity;
    }

    public bool GetInfrastructureValidity()
    {
        if (isTownhall)//when the player wants to build a townhall no infrastructere field weill be present beacause the townhall will start this game mechanic, thus must it be ignored when placing your first townhall
        {
            return true;
        }
        return infrastructureValidity;
    }
}
