using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using System.Collections.Generic;
using System.IO;
using UnityEngine.AI;
using UnityEngine.Networking;

public class ZombieNPC : MonoBehaviour
{
    [SerializeField]
    private AudioSource audio;
    
    public string uid;
    public string zombieName;
    public string shirt;
    public string pants;
    public string bodyType;
    public string gender;
    public string face;
    public string shoes; 

    public bool selected = false;

    [SerializeField]
    private GameObject deathPrefab;
    
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>(); 
        if (!PhotonNetwork.IsMasterClient) Destroy(GetComponent<NavMeshAgent>());
        audio = gameObject.AddComponent<AudioSource>();
    }

    public void Setup(string json)
    {
        var data = JObject.Parse(json); 

        uid = data["id"].ToString(); 
        zombieName = data["name"].ToString(); 
        shirt = data["body_material"].ToString();
        pants = data["leg_material"].ToString();
        bodyType = data["body_type"].ToString();
        gender = data["gender"].ToString(); 
        face = data["head_material"].ToString(); 
        shoes = data["feet_material"].ToString(); 
    }

    public void Select()
    {
        AudioRecorder.instance.StartRecording(); 

        PV.RPC("RPC_Select", RpcTarget.All); 
    }

    [PunRPC]
    void RPC_Select()
    {
        selected = true; 
    }

    public void Deselect()
    {
        AudioRecorder.instance.SaveRecording("save", (string outputPath) =>
        {
            if (outputPath != null)
            {
                var data = new { character_id = uid, audio_file_path = outputPath }; 
                WebReq.instance.Post("zombie", data, response =>
                {
                    var result = JObject.Parse(response); 

                    string character_response = result["character_response"].ToString(); 
                    int emotional_change = result["emotional_change"].ToObject<int>(); 
                    int emotional_state = result["emotional_state"].ToObject<int>(); 

                    HandleCharacterResponse(character_response, emotional_change, emotional_state); 
                }); 
            }
        }); 
    }

    public void HandleCharacterResponse(string response, int emotionalChange, int emotionalState)
    {
        if (emotionalState <= 0)
        {
            // zombie dead
            PhotonNetwork.Destroy(gameObject);
            Instantiate(deathPrefab, transform.position, Quaternion.identity);
        } else {
            PV.RPC("RPC_HandleCharacterResponse", RpcTarget.All, response, emotionalChange, emotionalState); 
        }
    }

    [PunRPC]
    void RPC_HandleCharacterResponse(string response, int emotionalChange, int emotionalState)
    {
        selected = false;

        string path = Path.Combine(Application.persistentDataPath, "tts_output.wav");
        
        var body =
        new {
            text = response,
            character_id = uid,
            stored_file_path = path
        };
        
        WebReq.instance.Post("tts", body, (string response) =>
        {
            StartCoroutine(LoadAndPlayWav(path));
        });
    }
    
    
    IEnumerator LoadAndPlayWav(string url_voice)
    {
        Debug.Log(url_voice);
        yield return new WaitForSeconds(0.2f);
        
        WWW w = new WWW(url_voice);
        yield return w;

        var ac = w.GetAudioClip();
        //var tempClip = w.audioClip;
        audio.clip = ac;
        audio.Play();
    }
}