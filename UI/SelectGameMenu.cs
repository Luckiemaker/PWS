using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class SelectGameMenu : MonoBehaviour
{
    public DataPersistanceManager manager;
    public TMP_InputField NewGameName;
    public GameObject CreateNewWorldPanel;
    public TMP_InputField LoadGameName;
    public TMP_InputField SeedText;

    private bool useRandomSeed;
    private GenerationDataSelector.GenerationMode mode;

    private void Start()
    {
        CreateNewWorldPanel.SetActive(false);
        this.gameObject.SetActive(true);

        SetRandomSeedBool(true);
    }

    public void SetRandomSeedBool(bool val)
    {
        useRandomSeed = val;
        SeedText.interactable = !val;
    }

    public void SelectGenerationType(int val)//selected via dropdown in UI
    {
        if (val == 0)
        {
            mode = GenerationDataSelector.GenerationMode.Default;//PLayer can select which data set is used for the world generation
        }
        if (val == 1)
        {
            mode = GenerationDataSelector.GenerationMode.Islands;
        }
    }

    public void OpenCreateWorldPanel()
    {
        CreateNewWorldPanel.SetActive(true);
    }
    public void CreateNewGame()
    {
        string name = NewGameName.text + ".game";
        GameData data = new(name);

        data.WorldGenerationMode = mode;//data for world generation is stored before data is loaded and game begins
        if (useRandomSeed != true)
        {
            int.TryParse(SeedText.text,out data.WorldSeed);
        }

        manager.NewGame(data);
        this.gameObject.SetActive(false);
    }

    public void LoadGame()
    {
        string name = LoadGameName.text + ".game";
        manager.LoadGame(name);//load data
        this.gameObject.SetActive(false);
    }
}
