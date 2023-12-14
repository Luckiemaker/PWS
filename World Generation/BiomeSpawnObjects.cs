using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeSpawnObjects : MonoBehaviour
{
    [SerializeField]public int index;

    public void SetIndex(int i)
    {
        this.index = i;
    }

    public int GetIndex()
    {
        return index;
    }

}
