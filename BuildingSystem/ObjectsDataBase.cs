using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


[CreateAssetMenu]
public class ObjectsDataBase : ScriptableObject
{
    public List<ObjectData> objectsData;

}
[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public bool IsObjectSpecific { get; private set; }

    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;//default is one

    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}

