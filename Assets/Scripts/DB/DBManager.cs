using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    public int a, b;

    private void Start()
    {
        StartCoroutine(Send());
    }

    private IEnumerator Send()
    {
        WWWForm form = new WWWForm();
        form.AddField("A", a);
        form.AddField("B", b);

        WWW www = new WWW("http://5.59.143.60:9089/login.php", form);

        yield return www;
        
        Debug.Log("Вывод: " + www.text);
    }
}
