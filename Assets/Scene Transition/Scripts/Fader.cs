using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    private const string FADER_PATH = "Fader";

    [SerializeField] private Animator animator;

    public static Fader _instance;

    public static Fader instance
    {
        get
        {
            if (_instance == null)
            {
                var prefab = Resources.Load<Fader>(FADER_PATH);
                _instance = Instantiate(prefab);
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    public bool isFadding { get; private set; }

    private Action _fadedInCallback;
    private Action _fadedOutCallback;

    public void FadeIn(Action fadedInCallback)
    {
        if(isFadding)
            return;

        isFadding = true;
        _fadedInCallback = fadedInCallback;
        animator.SetBool("faded", true);
    } 
    public void FadeOut(Action fadedOutCallback)
    {
        if (isFadding)
            return;

        isFadding = true;
        _fadedOutCallback = fadedOutCallback;
        animator.SetBool("faded", false);
    }

    private void Handle_FadeInAnimationOver()
    {
        _fadedInCallback?.Invoke();
        _fadedInCallback = null;
        isFadding = false;
    }
    private void Handle_FadeOutAnimationOver()
    {
        _fadedOutCallback?.Invoke();
        _fadedOutCallback = null;
        isFadding = false;

    }

}
