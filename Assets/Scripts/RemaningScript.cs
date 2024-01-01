using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemaningScript: MonoBehaviour {
    // Start is called before the first frame update
    private AGCC CloudController;
    private TextMeshProUGUI text;
    void Awake() {
        CloudController = CloudController != null ? CloudController : FindObjectOfType<AGCC>();
    }

    public void GiveUp() {
        CloudController.ag.PrivacySend("Leave", CloudController.EnemyUID);
        CloudController.text.text = "已放棄";
        Utils.Scenes.Login.Load();
    }
}
