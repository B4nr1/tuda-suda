using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FaderExample : MonoBehaviour
{

    private const string SCENE_0 = "MenuScene";
    private const string SCENE_1 = "GameScene";

    private bool _isloading;

    private static FaderExample _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        //DontDestroyOnLoad(gameObject);
    }
   
    public void Play()
    {
        LoadScene(SCENE_1);
    }
    public void Menu()
    {
        LoadScene(SCENE_0);
    }

    private void LoadScene(string sceneName) 
    {
        if(_isloading)
            return;

        var currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == sceneName)
            throw new Exception("You are trying to load alredy loaded scene.");

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        _isloading = true;

        var waitFading = true;
        Fader.instance.FadeIn(() => waitFading = false);

        while (waitFading)
            yield return null;

        var async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
            yield return null;

        async.allowSceneActivation = true;

        waitFading = true;
        Fader.instance.FadeOut(() => waitFading = false);

        while (waitFading)
            yield return null;

        _isloading = false;
    }
}
