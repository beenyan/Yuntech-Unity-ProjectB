using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class RemaningScript: MonoBehaviour {
    // Start is called before the first frame update
    private AGCC CloudController;
    private PlayerController PlayerController;
    private TextMeshProUGUI text;
    public static RemaningScript instance { get; private set; }
    private void Awake() {
        instance = this;
        CloudController = CloudController != null ? CloudController : FindObjectOfType<AGCC>();
        PlayerController = PlayerController != null ? PlayerController : FindObjectOfType<PlayerController>();
    }

    public void GiveUp() {
        CloudController.ag.PrivacySend("Leave", CloudController.EnemyUID);
        CloudController.text.text = "已放棄";
        Utils.Scenes.Login.Load();
    }

}
