using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks {
    [SerializeField] TMP_Text text;
    Player player;

    public void Setup(Player _player) {
        player = _player;
        text.text = _player.NickName;
        // Debug.Log(_player.NickName + " nick"); 
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        if (player == otherPlayer) {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom() {
        Destroy(gameObject);
    }
}
