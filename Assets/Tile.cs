using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{   
    private int _value = 2;

    [SerializeField]private TMP_Text text;

    public void SetValue(int value)
    {
        _value = value;
        text.text = value.ToString();
    }    
}
