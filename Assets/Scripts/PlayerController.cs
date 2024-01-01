using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController: MonoBehaviour {
    private float Health = 100;
    private readonly float MaxHealth = 100;
    private float Defense = 100;
    private readonly float MaxDefense = 100;
    public TMP_Text FontText;
    public TextMeshProUGUI text;
    private AGCC CloudController;

    void Awake() {
        CloudController = CloudController != null ? CloudController : FindObjectOfType<AGCC>();
    }

    // Update is called once per frame
    void Update() {
        FontText.text = $"生命：{Health}\n防禦：{Defense}";
        if (Health < 0) {
            CloudController.ag.PrivacySend("Leave", CloudController.EnemyUID);
            CloudController.text.text = "你輸了";
            Utils.Scenes.Login.Load();
        }
    }

    public void Attack(GemType gemType, int count) {
        float DmgOverflow = Defense - count;
        Health += DmgOverflow > 0 ? 0 : DmgOverflow;
        Def(-count);
    }

    public void Def(int count) {
        Defense = Mathf.Max(Mathf.Min(MaxDefense, Defense + count), 0);
    }

    public void Heal(int count) {
        Health = Mathf.Min(MaxHealth, Health + count);

    }

    internal static object GetMap() {
        throw new NotImplementedException();
    }
}
