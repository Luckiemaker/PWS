using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour,IDataPersistance
{
    [Header("Start Resources")]
    [SerializeField]
    int startWood;
    [SerializeField]
    int startStone;
    [SerializeField]
    int startFood;

    [SerializeField]
    private int wood;
    [SerializeField]
    private int stone;
    [SerializeField]
    private int food;

    public UI uiManager;

   // public enum ResourceType { wood,stone,food}

    private void Start()
    {
        uiManager.UpdateResourceCounter("wood", wood);
        uiManager.UpdateResourceCounter("stone", stone);
        uiManager.UpdateResourceCounter("food", food);

        this.name = "ResourceManager";//some code depends on the name
    }
    public void AddResource(string Type, int amount)
    {
        int newAmount;

        if (Type == "wood")
        {
            wood += amount;
            newAmount = wood;
        }
        else if (Type == "stone")
        {
            stone += amount;
            newAmount = stone;
        } else if (Type == "food")
        {
            food += amount;
            newAmount = food;
        } else
            throw new System.Exception("No resource with name " + Type.ToString());


        uiManager.UpdateResourceCounter(Type, newAmount);
    }

    public void RemoveResource(string Type, int amount)
    {
        int newAmount;

        if (Type == "wood")
        {
            wood -= amount;
            newAmount = wood;
        }
        else if (Type == "stone")
        {
            stone -= amount;
            newAmount = stone;
        }
        else if (Type == "food")
        {
            food -= amount;
            newAmount = food;
        }
        else
            throw new System.Exception("No resource with name " + Type.ToString());


        uiManager.UpdateResourceCounter(Type, newAmount);
    }

    public int GetResourceAmount(string Type)
    {
        if (Type == "wood")
        {
            return wood;
        }
        else if (Type == "stone")
        {
            return stone;
        }
        else if (Type == "food")
        {
            return food;
        }
        else
        {
            throw new System.Exception("No resource with name " + Type.ToString());
        }
    }

    public void LoadData(GameData data)
    {
        wood = data.Wood;
        stone = data.Stone;
        food = data.Food;

        uiManager.UpdateResourceCounter("wood", wood);
        uiManager.UpdateResourceCounter("stone", stone);
        uiManager.UpdateResourceCounter("food", food);
    }

    public void SaveData(GameData data)
    {
        data.Wood = wood;
        data.Stone = stone;
        data.Food = food;
    }
}
