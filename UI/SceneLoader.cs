using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class SceneLoader
{
    private class LoadingMonoBehaviour : MonoBehaviour { }
    private static bool finishedLoading = false;

    private static Action onLoaderCallBack;
    private static AsyncOperation loadingAsyncOperation;

    public enum LoadingScene
    {
        LoadingScene,
    }


    public static void Load(int scene)
   {
        //set loader callback action to the targetscene
        onLoaderCallBack = () =>
        {
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));          
        };

        //load scene
        SceneManager.LoadScene(LoadingScene.LoadingScene.ToString());
   }

    public static void LoaderCallBack()
    {
        //triggered after first update so the screen can refresh
        //Load the targetscene
        if(onLoaderCallBack != null)
        {
            onLoaderCallBack();
            onLoaderCallBack = null;
        }
    }

    public static float GetLoadingProgress()
    {
        if(loadingAsyncOperation != null)
        {
            return loadingAsyncOperation.progress;
        }
        else if(!finishedLoading)
        {
            return 0f;
        }else
            return 1f;
    }

    private static IEnumerator LoadSceneAsync(int scene)
    {
        yield return null;
        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene);

        while (!loadingAsyncOperation.isDone)
        {
            //skip frames until loading is done than exit
            yield return null;
        }
        finishedLoading = true;
    }
}
