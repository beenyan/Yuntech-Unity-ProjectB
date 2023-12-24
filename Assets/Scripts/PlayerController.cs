using TMPro;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerController: MonoBehaviour {
    // Start is called before the first frame update
    private float Health = 100;
    private readonly float MaxHealth = 100;
    private float Defense = 100;
    private readonly float MaxDefense = 100;
    public TMP_Text FontText;
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        FontText.text = $"生命：{Health}\n防禦：{Defense}";
    }

    public void Attack(GemType gemType, int count) {

    }

    public void Def(int count) {
        Defense = Mathf.Min(MaxDefense, Defense + count);
    }

    public void Heal(int count) {
        Health = Mathf.Min(MaxHealth, Health + count);
    }
}
