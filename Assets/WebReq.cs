using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class WebReq : MonoBehaviour
{
    public static WebReq inst { get; private set; }
    
    [field: SerializeField]
    public string server { get; private set; } = "http://localhost:8000";

    public void Awake()
    {
        if (inst)
        {
            Destroy(gameObject);
        }
        else
        {
            inst = this;
        }
    }

    public void Post(string endpoint, object data)
    {
        StartCoroutine(PostCoroutine(endpoint, data));
    }

    // data is something like ... var person = new { name = "Edem", age = 18 };
    private IEnumerator PostCoroutine(string endpoint, object data)
    {
        string json = JsonConvert.SerializeObject(data);
        Debug.Log(json);
        using (UnityWebRequest www = UnityWebRequest.Post($"{server}/{endpoint}",  json, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                //Debug.Log("Form upload complete!");
                Debug.Log("Form upload complete!");
                Debug.Log("Result: " + www.result);
                Debug.Log("Response body: " + www.downloadHandler.text);
            }
        }
    }
}
