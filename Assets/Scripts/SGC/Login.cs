using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class Login: MonoBehaviour {
    private Button quickRegisterBtn;         //按鈕本體
    public TMP_InputField nickNameInp;     //輸入框的帳號
    private AGCC ag;                          //網路主控制
    public GameObject LoginP;
    public GameObject ServerP;
    // Start is called before the first frame update
    void Start() {
        quickRegisterBtn = GetComponent<Button>();          //自動抓取元件
        quickRegisterBtn.onClick.AddListener(QuickUser);        //註冊該按鈕按下的時候會呼叫Login()
    }

    public void QuickUser() {

        if (ag == null) {
            ag = FindObjectOfType<AGCC>();  //抓網路主控物件
        }
        //向SGC申請新的用戶
        CloudSystem.ApplyNewUser(ag.GetGGUID(), ag.GetCertificate(), CB_GetSysAccount, null);
    }
    void CB_GetSysAccount(int code, object data, object token) {
        if (code == 0) //Code為0表示取得帳號成功
        {
            Hashtable ht = data as Hashtable; //取得帳號成功時將返回一組Hashtable
            string acc = ht["userid"].ToString(); //抓出帳號
            string pw = ht["passwd"].ToString();  //抓出密碼
            print("帳號:" + acc + ",密碼:" + pw);
            if (nickNameInp.text != "") {
                ag.CloudLaunch(acc, pw, OnLoginSuccess);
            } else {
                Debug.Log("不可以是空白暱稱");
            }
        }
        //Code非0表示取得帳號失敗
        else {
            print("取得失敗，請確認gguid與憑證的正確");
        }
    }
    public void SetNickName() {
        if (ag == null) {
            ag = FindObjectOfType<AGCC>();  //抓網路主控物件
        }
        if (nickNameInp.text != "") {
            //向SGC申請新的用戶
            ag.ag.SetPlayerNickname(nickNameInp.text, CB_SetNickName, null);
        } else {
            Debug.Log("不可以是空白暱稱");
        }
    }
    void CB_SetNickName(int code, object token) {
        //Code為0表示註冊成功
        if (code == 0) {
            Debug.Log("暱稱設定成功");
            LoginP.SetActive(false);
            ServerP.SetActive(true);
        }
        //Code非0表示註冊失敗
        else {
            Debug.LogWarning("Regist Failed - Error:" + code);
        }
    }
    public void OnLoginSuccess() {
        SetNickName();
    }
}