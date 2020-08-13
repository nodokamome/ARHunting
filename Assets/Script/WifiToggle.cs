using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WifiToggle : MonoBehaviour
{
    //Toggle01~04
    GameObject toggleObject01;
    GameObject toggleObject02;
    Toggle toggle01;
    Toggle toggle02;
    //settingNoの取得
    private ServerHostSetting serverHostSetting = new ServerHostSetting();
    private bool push = false;
    private float time = 0;
    private int second = 0;
    private float oldSeconds;
    private GameObject wifiDialog;
    private GameObject IPDialog;
    void Start()
    {
        wifiDialog = GameObject.Find("WifiDialog");
        IPDialog = GameObject.Find("IPDialog");

        toggleObject01 = GameObject.Find("Toggle01");
        toggle01 = toggleObject01.GetComponent<Toggle>();
        toggleObject02 = GameObject.Find("Toggle02");
        toggle02 = toggleObject02.GetComponent<Toggle>();

        if (serverHostSetting.GetSettingNo() == "1")
        {
            toggle01.isOn = true;
        }
        else if (serverHostSetting.GetSettingNo() == "2")
        {
            toggle02.isOn = true;
        }

    }

    public void PushDown()
    {
        push = true;
    }

    public void PushUp()
    {
        push = false;
    }

    void Update()
    {
        //Debug.Log(delay);
        if (push)
        {
            time += Time.deltaTime;
            second = (int)time % 60;
        }
        else
        {
            if (0 < time && time < 3)
            {
                DialogController dialogController = wifiDialog.GetComponent<DialogController>();
                dialogController.OpenDialog();
                OpenDialog();
            }
            else if (3 <= second)
            {
                DialogController dialogController = IPDialog.GetComponent<DialogController>();
                dialogController.OpenDialog();

                IPDialog ipDialog = IPDialog.GetComponent<IPDialog>();
                ipDialog.OpenDialog();
            }
            time = 0;
            second = 0;
        }
    }

    public void OpenDialog()
    {
        if (serverHostSetting.GetSettingNo() == "1")
        {
            toggle01.isOn = true;
        }
        else if (serverHostSetting.GetSettingNo() == "2")
        {
            toggle02.isOn = true;
        }
    }
}
