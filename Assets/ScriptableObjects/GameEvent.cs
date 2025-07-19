using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    public const float delayForDestroy = 0.04f;

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
    }
    public void LoadSceneAdditive(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
    }

    public void LoadAsyncScene(int sceneIndex)
    {
        CoroutineHandler.Instance.RunCoroutine(LoadYourAsyncScene(sceneIndex));
    }

    public void PauseScene(bool isPaused)
    {
        CoroutineHandler.Instance.RunCoroutine(PauseGame(isPaused));
    }

    private IEnumerator LoadYourAsyncScene(int sceneIndex)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad_0 = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad_0.isDone)
        {
            yield return null;
        }
        //Debug.Log(currentScene.name);
        //SceneManager.UnloadSceneAsync(currentScene);
    }

    private IEnumerator PauseGame(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0f;
            //AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1f;
            //AudioListener.pause = false;
        }
        yield return null;
    }

    public void DestroyGameObject(GameObject objToDestroy)
    {
        if (objToDestroy)
        {
            GameObject.Destroy(objToDestroy, delayForDestroy);
        }
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
