using UnityEngine;
using Photon.Pun;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this; 
    }

    public void StartNewWave(int zombieCount, string _theme)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var data = new { num_characters = zombieCount, theme = _theme }; 

        Debug.Log($"{zombieCount} zombies spawning");
        
        WebReq.instance.Post("chat/characters/generate", data, response =>
        {
            var result = JObject.Parse(response); 
            var charactersArray = result["characters"] as JArray; 
            
            Debug.Log(result);

            if (charactersArray != null)
            {
                foreach (JObject character in charactersArray) {
                    string characterJson = character.ToString();

                    // old
                    // var pos = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));

                    var pos = NavManager.inst.getNextSpawn();
                    GameObject zombieGO = InstantiateZombie(pos); 
                    NavManager.inst.AddAgent(zombieGO.GetComponent<NavMeshAgent>());

                    ZombieNPC zombie = zombieGO.GetComponent<ZombieNPC>(); 
                    zombie.Setup(characterJson); 
                }
            }
        });
    }

    public GameObject InstantiateZombie(Vector3 position)
    {
        if (!PhotonNetwork.IsMasterClient) return null;

        PlayerController[] players = GameObject.FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        GameObject zombie = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "zombie2"), position, Quaternion.identity, 0);

        return zombie; 
    }
}