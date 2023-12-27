using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AGCC: MonoBehaviour {
    //�C����gguid
    public TextMeshProUGUI text;
    private const string gguid = "d45aa87b-08a0-497b-b48b-4330b059db13";
    //�C��������
    byte[] certificate = { 0x13, 0x19, 0x4A, 0xC0, 0xE7, 0x58, 0x4C, 0x91, 0x9E, 0x11, 0x92, 0x74, 0x6E, 0x52, 0x2B, 0x64, 0x75, 0x21, 0x9C, 0x36, 0x3C, 0x14, 0x4B, 0x21, 0x89, 0x6D, 0xBB, 0x4C, 0xF0, 0xFE, 0x53, 0x97, 0x56, 0xF9, 0xE3, 0x25, 0xAC, 0x5D, 0x48, 0x62, 0x84, 0xDA, 0x8E, 0xCC, 0xFB, 0xA3, 0x81, 0xE4, 0x58, 0xB8, 0xB5, 0x23, 0x87, 0xC7, 0x48, 0x7D, 0xB6, 0x9E, 0x7A, 0xA9, 0x1C, 0xA1, 0x29, 0x49, 0xDA, 0xC3, 0x42, 0x8B, 0xC2, 0x31, 0x47, 0x19, 0xB9, 0x14, 0xCB, 0x8A, 0x36, 0xD4, 0x05, 0x54, 0x51, 0x1E, 0x3F, 0xBF, 0x9E, 0x8B, 0x42, 0x59, 0x90, 0x4D, 0x80, 0x7D, 0x2B, 0xB1, 0x16, 0xC1, 0x8A, 0x10, 0x8E, 0x6C, 0xB5, 0x9D, 0x4B, 0x06, 0xB5, 0x9E, 0xBB, 0x4B, 0x9E, 0xBE, 0xC7, 0x60, 0x0D, 0xF2, 0x76, 0x65, 0xEC, 0xFB, 0x4A, 0x00, 0x8E, 0x74, 0xCE, 0x57, 0xAE, 0x65, 0xAE, 0xF1 };
    public CloudGame ag = null;  //SGC�D������
    public CloudScene chatSn;    //��Ѻ������� (��ѥD�j�U)
    private string sguid = "e92557c3-dfdb-49d6-b921-64e63a466016";        //�ڪ���������(��ѫ�)
    private void Awake() {
        DontDestroyOnLoad(this);
    }
    public void Start() {
        CloudSystem.UnityEnvironment();                      // �Ұʬ�Unity�Ҧ�
        CloudSystem.ServerProvider("sgc-api-us.spkita.com"); // �]�w���A��
        SceneManager.LoadScene(1);                           //�}����������n�J����
    }
    public Action loginCallBack;
    //�n�J�ɽЩI�s����k�A���O�a�J�b���P�K�X�Ѽ�
    public void CloudLaunch(string username, string password, Action loginCallBack) {
        this.loginCallBack = loginCallBack;                         //�]�w�n�J������CallBack
        ag = new CloudGame(username, password, gguid, certificate); //�n�J�ɩһݪ��Ѽ� ���O��(�b��,�K�X,gguid, certificate)
        ag.onCompletion += OnCompletion;                            //���w�B�z��k�A�ҥγs�u�O�_���\����
        ag.onStateChanged += CloudStateChanged;                     //�s�u�i�װl��
        ag.UnityLaunch();                                           //�s�u
    }
    //���n�J������|���榹��k
    void OnCompletion(int code, CloudGame game) {
        if (code == 0) {
            //�n�J���\�ɷ|���榹�q
            // Debug.Log("�n�J���\");
            if (loginCallBack != null)//���a�JcallBack����
            {
                loginCallBack();    //�I�s
            }
        } else {
            //�n�J���Ѯɰ��檺�Ϭq
            // Debug.LogWarning("�n�J���ѡA���~�N�X�G" + code);
        }
    }
    //�l�ܳs�u�i��
    private void CloudStateChanged(int state, int code, CloudGame game) {
        if (state <= 600) {
            Debug.Log("�s�u�i��:" + state);
        } else if (state >= 900) {
            Debug.Log("�_�}�s�u:" + state);
        }
    }
    /// <summary>
    /// ���ogguid
    /// </summary>
    public string GetGGUID() {
        return gguid;
    }
    /// <summary>
    /// ���o����
    /// </summary>
    public byte[] GetCertificate() {
        return certificate;
    }

    public void EnterScene() {
        chatSn = new CloudScene(ag, sguid);
        chatSn.onCompletion += CB_EnterScene;   //���i��������\�Ncallback���ڪ��D�@�U
        chatSn.onMessageIn += OnSceneMessageIn; //���������T��
        chatSn.Launch();
    }
    public void MatchScene(uint sid) {
        chatSn = new CloudScene(ag, sguid, sid);
        chatSn.onCompletion += CB_EnterScene;   //���i��������\�Ncallback���ڪ��D�@�U
        chatSn.onMessageIn += OnSceneMessageIn; //���������T��
        chatSn.Launch();
    }
    //���i�J����������|���榹��k
    void CB_EnterScene(int code, CloudScene scene) {
        if (code == 0) {
            //�i�J���\�ɷ|���榹�q�A���K�������Sid�L�X��
            Debug.Log("Enter Scene Successed - Sid:" + chatSn.sid);
            text.text = chatSn.sid + "";
            SceneManager.LoadScene(2);
        } else {
            //�i�J���Ѯɰ��檺�Ϭq
            Debug.LogError("Enter Scene Failed:" + code);
        }
    }
    //��ѫǦ���T��
    void OnSceneMessageIn(string msg, int delay, CloudScene scene) {
        //�B�z�����T��
        Debug.Log("��������T��:" + msg);
    }
}