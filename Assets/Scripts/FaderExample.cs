using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;



    public  class FaderExample : MonoBehaviour 
    {
        private const string SCENE_1 = "GameScene";

        private const string SCENE_0 = "MainScene";


        private bool _isLoading;


        private static FaderExample _instance;

        private void Awake() 
        { 
            if (_instance != null) 
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void Update()


        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
                LoadScene(SCENE_0);
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
                LoadScene(SCENE_1);

    }

        private void LoadScene(string sceneName)
    {
        if(_isLoading)
            return;

        var currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == sceneName)
            throw new Exception("You are trying to load already loaded scene. ");

        StartCoroutine(LoadSceneRoutine(sceneName));

    }
        private IEnumerator LoadSceneRoutine(string sceneName)
    {
        _isLoading= true;

        var waitFading = true;

        Fader.instance.FadeIn(() => waitFading = false);
        
        while (waitFading)
            yield return null;

        var async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
            yield return null;

        async.allowSceneActivation = true;

        waitFading= true;
        Fader.instance.FadeOut(() => waitFading = false);
        
        while (waitFading)
            yield return null;

        _isLoading= false;


    }
}
        
