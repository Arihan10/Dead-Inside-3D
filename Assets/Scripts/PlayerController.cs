using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.XR.CoreUtils;

public class PlayerController : MonoBehaviour {

    public PhotonView PV;

    PlayerManager playerManager; 

    private void Awake() {
        PV = GetComponent<PhotonView>();
        if (PV) playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>(); 
    }

    private void Start() {
        if (PV && !PV.IsMine) {
            Destroy(GetComponent<Rigidbody>()); 
            Destroy(GetComponent<XROrigin>());
            Destroy(GetComponent<LocalPlayer>());
        }
    }

    private void Update()
    {
        if (!PV.IsMine) return; 

        if (Input.GetKeyDown(KeyCode.N))
        {
            GameManager.instance.StartNewWave(2, "anything"); 
        }
    }
}
