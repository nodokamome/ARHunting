using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HomeDirector : MonoBehaviour {
    //UI
    public Text NameText;
    public Text HPText;
    public Text HPRecoveryText;
    public Slider SliderYourHP;
    //回復時間
    private float timeleft;
    private int recoveryTime;
    //体力
    private int currentHP;    
    private int initialHP;    
    //UserDataDB
    private UserDataDB userDataDB = new UserDataDB();

	// 初期化
    void Start() {
        //自分の名前と体力を取得
        string name = userDataDB.GetUserData("name");
        currentHP = int.Parse(userDataDB.GetUserData("currentHP"));
        initialHP = int.Parse(userDataDB.GetUserData("initialHP"));
        //取得した情報を表示
        NameText.text = name;
        HPText.text = currentHP+"/"+initialHP;
        SliderYourHP.maxValue = initialHP;
        SliderYourHP.value = currentHP;
    }
	
	void Update () {
        //体力回復
        if(currentHP < initialHP){  
            recoveryTime = initialHP - currentHP;
            //残り時間
            HPRecoveryText.text = "完全回復まで " + recoveryTime + " 秒";
            HPText.text = currentHP+"/"+initialHP;
            timeleft -= Time.deltaTime;
            if (timeleft <= 0.0) {
                timeleft = 1.0f;
                //回復
                currentHP = currentHP + 1;
                SliderYourHP.value = currentHP;
                
                userDataDB.SetUserData("currentHP", currentHP.ToString());
            }

        }
        if(currentHP == initialHP){
            HPRecoveryText.text = "";
            HPText.text = currentHP+"/"+initialHP;
        }
    }
}