using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSound : MonoBehaviour
{
    private static readonly string MusicPref = "MusicPref";

    public float musicFloat;
    public AudioSource musicAudio;

    private void Awake()
    {
        SceneSoundSwttings();
    }

    private void SceneSoundSwttings()
    {
        musicFloat = PlayerPrefs.GetFloat(MusicPref);
        
        musicAudio.volume = musicFloat;
    }
}
