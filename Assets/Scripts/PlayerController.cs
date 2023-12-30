using System;
using TMPro;
using UnityEngine;

public class PlayerController: MonoBehaviour {
    private float Health = 100;
    private readonly float MaxHealth = 100;
    private float Defense = 100;
    private readonly float MaxDefense = 100;
    public TMP_Text FontText;

    // Update is called once per frame
    void Update() {
        FontText.text = $"生命：{Health}\n防禦：{Defense}";
    }

    public void Attack(GemType gemType, int count) {
        float DmgOverflow = Defense - count;
        Health -= DmgOverflow < 0 ? DmgOverflow : 0;
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
