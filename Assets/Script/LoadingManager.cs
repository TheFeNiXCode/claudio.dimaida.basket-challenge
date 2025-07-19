using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Image progressBar;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        gameLevel.allowSceneActivation = false;

        while (gameLevel.progress < 1)
        {
            progressBar.fillAmount = gameLevel.progress;
            if (gameLevel.progress >= 0.9f) gameLevel.allowSceneActivation = true;
            yield return new WaitForEndOfFrame();
        }
        SceneManager.UnloadSceneAsync(currentScene);
    }
}
