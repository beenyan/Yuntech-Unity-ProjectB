using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController: MonoBehaviour {
    public float Health = 100;
    public float Defense = 100;
    private readonly float MaxHealth = 100;
    private readonly float MaxDefense = 100;
    public TMP_Text FontText;
    public TextMeshProUGUI text;
    private AGCC CloudController;
    private GameObject LowHealth;
    private Image LowHealthImage;
    private AudioSource HeartBeat;

    void Awake() {
        CloudController = CloudController != null ? CloudController : FindObjectOfType<AGCC>();
        LowHealth = GameObject.Find("Overlay/LowHealth");
        LowHealthImage = LowHealth.GetComponent<Image>();
        HeartBeat = LowHealth.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        FontText.text = $"生命：{Health}\n防禦：{Defense}";
        HeartBeat.volume = 1 - Health / MaxHealth;
        Utils.FindByTag(Utils.Tags.Background).GetComponent<AudioSource>().volume = 0.1f - (1 - Health / MaxHealth) / 10;
        LowHealthImage.ChangeAlpha(1 - Health / MaxHealth);
        if (Health <= 0) {
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
