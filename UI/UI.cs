using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    public TMP_Text woodCounter;
    public TMP_Text stoneCounter;
    public TMP_Text foodCounter;

    public void LoadScene(int scene)
    {
        SceneLoader.Load(scene);
    }

    public void UpdateResourceCounter(string type,int value)//update resource definde by a string
    {
        TMP_Text updatedText;

        if (type == "wood")
        {
            updatedText = woodCounter;
        }
        else if(type == "stone")
        {
            updatedText = stoneCounter;
        }else if(type == "food")
        {
            updatedText = foodCounter;
        }
        else
            throw new System.Exception($"No resource type of {type}");

        updatedText.text = value.ToString();
    }

    public void DisableButton(Button button)
    {    
         button.interactable = false;   
    }

    public void EnableButton(Button button)
    {
        button.interactable = true;    
    }
}
