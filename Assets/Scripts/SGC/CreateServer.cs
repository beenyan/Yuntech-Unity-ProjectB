using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreateServer: MonoBehaviour {
    private Button CreateServerBT;
    private AGCC ag;                          //網路主控制
    void Start() {
        if (FindObjectOfType<AGCC>() == null) {
            Utils.Scenes.AGCC.Load();
        }

        CreateServerBT = GetComponent<Button>();          //自動抓取元件
        CreateServerBT.onClick.AddListener(Create);        //註冊該按鈕按下的時候會呼叫Login()
    }

    void Create() {
        if (ag == null) {
            ag = FindObjectOfType<AGCC>();  //抓網路主控物件
        }

        ag.EnterScene();
    }
}
