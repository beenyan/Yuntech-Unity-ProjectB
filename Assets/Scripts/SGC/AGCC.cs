using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;
//using UnityEditor.PackageManager;

public class AGCC: MonoBehaviour {
    public TextMeshProUGUI text;
    //遊戲的gguid
    private const string gguid = "fca651ff-cd80-45dc-b18d-f7b9e2c4c107";
    //遊戲的憑證
    byte[] certificate = { 0xB6, 0x29, 0x1D, 0x18, 0x8E, 0xDF, 0x43, 0x22, 0xAD, 0x1E, 0x54, 0xAB, 0xD9, 0x0F, 0xD8, 0x8E, 0x6B, 0x93, 0x9F, 0x91, 0xA4, 0xFE, 0x47, 0xB8, 0xA5, 0x2B, 0x1E, 0x78, 0x8A, 0x20, 0x0C, 0x8F, 0xD2, 0x0E, 0x26, 0x8D, 0x6B, 0xB7, 0x47, 0xEB, 0xA2, 0xF8, 0x25, 0x30, 0x3F, 0x64, 0xCC, 0x7C, 0xE1, 0xD3, 0x19, 0x46, 0xC0, 0x6C, 0x4D, 0x63, 0x90, 0xE7, 0xFF, 0x50, 0x45, 0xAF, 0x88, 0x92, 0x8C, 0xAA, 0xB5, 0xC6, 0x77, 0x76, 0x49, 0x6C, 0xBC, 0xC2, 0x6D, 0x4F, 0x2E, 0x5D, 0x98, 0xC8, 0x42, 0x8B, 0x39, 0xDA, 0xD2, 0x87, 0x46, 0x63, 0xA5, 0x22, 0x45, 0xE5, 0x82, 0x9E, 0x18, 0x93, 0x31, 0xFC, 0x3D, 0x1E, 0x8F, 0x71, 0x44, 0x5A, 0x9B, 0xE2, 0xDE, 0xE2, 0xCF, 0x2D, 0x19, 0x1F, 0x44, 0xCD, 0xDD, 0x15, 0x6C, 0x67, 0x4A, 0xE8, 0xA4, 0x66, 0x5D, 0x38, 0x06, 0x35, 0xCE, 0x40 };
    public CloudGame ag = null; //SGC主控物件
    public CloudScene chatSn;
    private string sguid = "025974a0-2642-4123-b4c8-5b2e7e5c281a";
    public static GameObject[,] PlayerMap = new GameObject[(int)GameController.MapSize.y, (int)GameController.MapSize.x];
    public static GameObject[,] EnemyMap = new GameObject[(int)GameController.MapSize.y, (int)GameController.MapSize.x];
    public static int PlayerRandomSeed = 0;
    public static int EnemyRandomSeed = 0;
    GameController EnemyGameController;
    public uint EnemyUID = 0;
    public Action loginCallBack;

    //登入時請呼叫此方法，分別帶入帳號與密碼參數
    private void Awake() {
        DontDestroyOnLoad(this);
        CloudSystem.UnityEnvironment(); // 啟動為Unity模式
        CloudSystem.ServerProvider("sgc-api-us.spkita.com"); // 設定伺服器
        CloudSystem.ApplyNewUser(gguid, certificate, CB_GetSysAccount, null);
    }

    void CB_GetSysAccount(int code, object data, object token) {
        if (code == 0) //Code為0表示取得帳號成功
        {
            Hashtable ht = data as Hashtable; //取得帳號成功時將返回一組Hashtable
            string acc = ht["userid"].ToString(); //抓出帳號
            string pw = ht["passwd"].ToString();  //抓出密碼
            CloudLaunch(acc, pw, OnLoginSuccess);
        }
        //Code非0表示取得帳號失敗
        else {
            print("取得失敗，請確認gguid與憑證的正確");
        }
    }

    public void OnLoginSuccess() {
        ag.SetPlayerNickname("name", CB_SetNickName, null);
    }

    void CB_SetNickName(int code, object token) {
        if (code == 0) {
            Debug.Log("暱稱設定成功");
            Utils.Scenes.Login.Load(); //開場直接跳到登入頁面
        } else {
            Debug.LogWarning("Regist Failed - Error:" + code);
        }
    }

    public void CloudLaunch(string username, string password, Action loginCallBack) {
        this.loginCallBack = loginCallBack; //設定登入完成的CallBack
        ag = new CloudGame(username, password, gguid, certificate); //登入時所需的參數 分別為(帳號,密碼,gguid, certificate)
        ag.onCompletion += OnCompletion; //指定處理方法，啟用連線是否成功偵測
        ag.onStateChanged += CloudStateChanged; //連線進度追蹤
        ag.onPrivateMessageIn += OnPrivateMessageIn; //設定私訊接收方法
        ag.UnityLaunch(); //連線
    }

    //當登入完畢後會執行此方法
    void OnCompletion(int code, CloudGame game) {
        //登入成功時會執行此段
        if (code == 0) {
            if (loginCallBack != null) {
                loginCallBack();
            }
        } else {
            Debug.LogWarning("Error: " + code);
        }
    }

    void OnApplicationQuit() {
        //當程式關閉時使用者帳號並不會登出,我們可以此加入自動登出的機制
        if (ag != null)
            ag.Dispose();
    }

    //追蹤連線進度
    private void CloudStateChanged(int state, int code, CloudGame game) {
        if (state <= 600) {
            Debug.Log("連線進度:" + state);
        } else if (state >= 900) {
            Debug.Log("斷開連線:" + state);
        }
    }

    /// <summary>
    /// 取得gguid
    /// </summary>
    public string GetGGUID() {
        return gguid;
    }

    /// <summary>
    /// 取得憑證
    /// </summary>
    public byte[] GetCertificate() {
        return certificate;
    }

    public void EnterScene() {
        chatSn = new CloudScene(ag, sguid);
        chatSn.onCompletion += CB_EnterScene;
        chatSn.onMessageIn += OnSceneMessageIn;
        chatSn.Launch();
    }

    public void MatchScene(uint sid) {
        chatSn = new CloudScene(ag, sguid, sid);
        chatSn.onCompletion += CB_EnterScene;
        chatSn.onMessageIn += OnSceneMessageIn;
        chatSn.Launch();
    }

    public void JoinRandomScene() {
        chatSn = new CloudScene(ag, sguid);
        chatSn.onCompletion += CB_EnterScene;
        chatSn.onMessageIn += OnSceneMessageIn;
        chatSn.Match();
    }

    void CB_EnterScene(int code, CloudScene scene) {
        if (code == 0) {
            Debug.Log("Enter Scene Successed - Sid:" + chatSn.sid);
            text.text = chatSn.sid + "";
            Utils.Scenes.Play.Load();
        } else {
            Debug.LogError("Enter Scene Failed:" + code);
        }
    }

    void OnSceneMessageIn(string msg, int delay, CloudScene scene) {
        GameInitData data = JsonConvert.DeserializeObject<GameInitData>(msg);

        if (data.uuid == ag.poid) {
            return;
        }

        if (data.Request) {
            var sendData = new GameInitData(PlayerMap, PlayerRandomSeed, ag.poid, false);
            chatSn.Send(JsonConvert.SerializeObject(sendData));
        }

        Debug.Log(msg);

        EnemyUID = data.uuid;
        EnemyRandomSeed = data.RandomSeed;
        EnemyMap = new GameObject[data.Map.GetLength(0), data.Map.GetLength(1)];
        for (int y = 0; y < data.Map.GetLength(0); y++) {
            for (int x = 0; x < data.Map.GetLength(1); x++) {
                EnemyMap[y, x] = Instantiate(Resources.Load<GameObject>(Utils.Resources.Gem.ToString()));
                EnemyMap[y, x].GetComponent<Gem>().Init(y, x, Utils.FindByTag(Utils.Tags.EnemyPlace).transform.GetChild(0).gameObject, (GemType)data.Map[y, x]);
            }
        }

        EnemyGameController = Utils.FindByTag(Utils.Tags.EnemyPlace).transform.GetChild(0).GetComponent<GameController>();
        EnemyGameController.Map = EnemyMap;
        Utils.FindByTag(Utils.Tags.Waiting).SetActive(false);
    }

    void OnPrivateMessageIn(string msg, int delay, CloudGame game) {
        Vector2[] data = JsonConvert.DeserializeObject<Vector2[]>(msg, new Vector2Converter());
        Debug.Log($"Received: {data[0]}&{data[1]}");
        EnemyGameController.GemClick(new Vector2((int)data[0].x, (int)data[0].y));
        EnemyGameController.GemClick(new Vector2((int)data[1].x, (int)data[1].y));
    }
}