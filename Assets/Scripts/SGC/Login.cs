using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class Login: MonoBehaviour {
    private Button quickRegisterBtn;         //���s����
    public TMP_InputField nickNameInp;     //��J�ت��b��
    private AGCC ag;                          //�����D����
    public GameObject LoginP;
    public GameObject ServerP;
    // Start is called before the first frame update
    void Start() {
        quickRegisterBtn = GetComponent<Button>();          //�۰ʧ������
        quickRegisterBtn.onClick.AddListener(QuickUser);        //���U�ӫ��s���U���ɭԷ|�I�sLogin()
    }

    public void QuickUser() {

        if (ag == null)
            ag = FindObjectOfType<AGCC>();  //������D������
        //�VSGC�ӽзs���Τ�
        CloudSystem.ApplyNewUser(ag.GetGGUID(), ag.GetCertificate(), CB_GetSysAccount, null);
    }
    void CB_GetSysAccount(int code, object data, object token) {
        if (code == 0) //Code��0���ܨ��o�b�����\
        {
            Hashtable ht = data as Hashtable; //���o�b�����\�ɱN��^�@��Hashtable
            string acc = ht["userid"].ToString(); //��X�b��
            string pw = ht["passwd"].ToString();  //��X�K�X
            print("�b��:" + acc + ",�K�X:" + pw);
            ag.CloudLaunch(acc, pw, OnLoginSuccess);
        }
        //Code�D0���ܨ��o�b������
        else {
            print("���o���ѡA�нT�{gguid�P���Ҫ����T");
        }
    }
    public void SetNickName() {
        if (ag == null)
            ag = FindObjectOfType<AGCC>();  //������D������
        if (nickNameInp.text != "") {
            //�VSGC�ӽзs���Τ�
            ag.ag.SetPlayerNickname(nickNameInp.text, CB_SetNickName, null);
        } else {
            Debug.Log("���i�H�O�ťռʺ�");
        }
    }
    void CB_SetNickName(int code, object token) {
        //Code��0���ܵ��U���\
        if (code == 0) {
            Debug.Log("Get NickName Success");
            LoginP.SetActive(false);
            ServerP.SetActive(true);
            //ag.EnterScene();             //�}�l�i���������
        }
        //Code�D0���ܵ��U����
        else {
            Debug.LogWarning("Regist Failed - Error:" + code);
        }
    }
    public void OnLoginSuccess() {
        SetNickName();
    }
}