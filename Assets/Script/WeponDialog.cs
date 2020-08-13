using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WeponDialog : MonoBehaviour {
    //UI
	public GameObject select1;
	public GameObject select2;
	public GameObject select3;
    public Image ButtonBallet;
    public Sprite wepon1;
    public Sprite wepon2;
    public Sprite wepon3;
    //WeponNo
	private int weponNo;
    private int newWeponNo;
    //UserDataDBクラス
    private UserDataDB userDataDB = new UserDataDB();
    //Color
    private Color Red = new Color(255.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f, 255.0f / 255.0f);
    private Color White = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);

    void Start(){
        if(SceneManager.GetActiveScene().name == "Game"){
            weponNo = int.Parse(userDataDB.GetUserData("weponNo"));
            switch(weponNo){
                case 1:
                ButtonBallet.GetComponent<Image> ().sprite = wepon1;
                    break;
                case 2:
                    ButtonBallet.GetComponent<Image> ().sprite = wepon2;
                    break;
                case 3:
                    ButtonBallet.GetComponent<Image> ().sprite = wepon3;
                    break;
                default:
                    ButtonBallet.GetComponent<Image> ().sprite = wepon1;
                    break;
            }
        }
    }
    
	public void StartDialog()
    {
        weponNo = int.Parse(userDataDB.GetUserData("weponNo"));
        if(weponNo == 1){
            Wepon1();
        }
        else if(weponNo == 2){
            Wepon2();
        }
        else if(weponNo == 3){
            Wepon3();
        }
    }

	public void Wepon1()
    {
        select1.GetComponent<Image>().color = Red;
        select2.GetComponent<Image>().color = White;
        select3.GetComponent<Image>().color = White;
        newWeponNo = 1;
    }

	public void Wepon2()
    {
		select1.GetComponent<Image>().color = White;
        select2.GetComponent<Image>().color = Red;
        select3.GetComponent<Image>().color = White;
        newWeponNo = 2;
    }

	public void Wepon3()
    {
		select1.GetComponent<Image>().color = White;
        select2.GetComponent<Image>().color = White;
        select3.GetComponent<Image>().color = Red;
        newWeponNo = 3;
    }

    public void onClickSaveHome()
    {
        userDataDB.SetUserData("weponNo", newWeponNo.ToString());
    }
    //保存する
    public void onClickSaveGame()
    {
        userDataDB.SetUserData("weponNo", newWeponNo.ToString());
        switch(newWeponNo){
        case 1:
            ButtonBallet.GetComponent<Image> ().sprite = wepon1;
            break;
        case 2:
            ButtonBallet.GetComponent<Image> ().sprite = wepon2;
            break;
        case 3:
            ButtonBallet.GetComponent<Image> ().sprite = wepon3;
            break;
        default:
            ButtonBallet.GetComponent<Image> ().sprite = wepon1;
            break;
        }
    }

    
}
