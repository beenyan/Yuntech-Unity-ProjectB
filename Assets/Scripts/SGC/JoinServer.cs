using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class JoinServer: MonoBehaviour {
    private Button JoinServerBT;
    public TMP_InputField InputSID;
    private AGCC ag;                          //網路主控制
    // Start is called before the first frame update
    void Start() {
        JoinServerBT = GetComponent<Button>();          //自動抓取元件
        JoinServerBT.onClick.AddListener(Create);        //註冊該按鈕按下的時候會呼叫Login()
    }

    void Create() {
        if (ag == null)
            ag = FindObjectOfType<AGCC>();  //抓網路主控物件
        if (InputSID.text != "") {
            ag.MatchScene(Convert.ToUInt32(InputSID.text, 10));
        } else {
            ag.JoinRandomScene();
        }
    }
}