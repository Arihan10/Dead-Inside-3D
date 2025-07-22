using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject player; 

    PhotonView PV;

    private void Awake() {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start() {
        if (PV.IsMine) {
            CreateController();
        }
    }

    void CreateController() {
        player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerControllerXR1"), new Vector3(0f,1.5f,0f), Quaternion.identity, 0, new object[] { PV.ViewID });
    }
}
