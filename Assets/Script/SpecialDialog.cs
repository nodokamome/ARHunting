using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SpecialDialog : MonoBehaviour
{
    //通信部分
	private ServerHostSetting serverHostSetting = new ServerHostSetting();
    private float TimeOutSec;
    private string URL;
    //UserDataDBクラス
    private UserDataDB userDataDB = new UserDataDB();
    //Methodクラス（名前チェックのため）
    private Method method = new Method();
    private UserData userData = new UserData();
    //DialogControlloer
    private DialogController SpecialDialogController;
    
    void Start(){
        //SpecialDialog
        GameObject SpecialDialog = GameObject.Find ("SpecialDialog");                             
        SpecialDialogController = SpecialDialog.GetComponent<DialogController>();
        //URL, timeoutを取得
        TimeOutSec = serverHostSetting.TimeOutSec;
        URL = serverHostSetting.GetURL("SettingDialog");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
