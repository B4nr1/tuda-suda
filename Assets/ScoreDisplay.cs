using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    private TMP_Text _text;
    void Start()
    {
        _text = GetComponent<TMP_Text>();  
    }

    public void UpdateScore(int score)
    {
        _text.text = score.ToString();
    }

  
}
