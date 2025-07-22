using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Signal : MonoBehaviour
{
    public Transform camera;
    public Transform a;
    public Transform b;
    public float speed;
    public float amp;
    public float damping;
    public int targetFramerate;
    public float noise;
    public float xSpeed;

    void Start()
    {
        b.position = a.position;
    }

    void Update()
    {
        Application.targetFrameRate = targetFramerate;
        
        a.position = new Vector3(Time.time * xSpeed, Mathf.Sin(Time.time * speed) * amp + Random.Range(-noise, noise), 0.0f);
        b.position = new Vector3(Time.time * xSpeed, Mathf.Lerp(b.position.y, a.position.y, damping), b.position.z);
        camera.position = new Vector3(Time.time * xSpeed, 0.0f, camera.position.z);
    }
}
