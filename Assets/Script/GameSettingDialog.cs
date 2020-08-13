using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class GameSettingDialog : MonoBehaviour {
   
    private int Sound;
    private int newSound;

    private bool MonstarInfoCheck = true;
    private bool OthersCheck = true;
    
    public Canvas canvasMonstarInfo = null;
    public Canvas canvasOthers = null;

    private Toggle ToggleSound, ToggleMonsterInfo, ToggleOthers;
    private UserDataDB userDataDB = new UserDataDB();

    
    void Start(){
        GameObject toggleSound = GameObject.Find ("ToggleSound");                             
        ToggleSound = toggleSound.GetComponent<Toggle>();
        
        GameObject toggleMonsterInfo = GameObject.Find ("ToggleMonsterInfo");                             
        ToggleMonsterInfo = toggleMonsterInfo.GetComponent<Toggle>();
        
        GameObject toggleOthers = GameObject.Find ("ToggleOthers");                             
        ToggleOthers = toggleOthers.GetComponent<Toggle>();
    }


	public void StartDialog () {
        Sound = int.Parse(userDataDB.GetUserData("sound"));

		if(Sound == 1){
			ToggleSound.isOn = true;
		}else{
			ToggleSound.isOn = false;
        }
	}

	public void onClickSave()
    {
        if(ToggleSound.isOn){
            newSound = 1;
        }else{
            newSound = 0;
        }
        userDataDB.SetUserData("sound", newSound.ToString());

        MonstarInfoCheck = ToggleMonsterInfo.isOn;
        OthersCheck = ToggleOthers.isOn;
    }
    
    public void onClickCancel()
    {
        ToggleMonsterInfo.isOn = MonstarInfoCheck;
        ToggleOthers.isOn = OthersCheck;
    }

    public void onClickMonstarInfo(){
        if(canvasMonstarInfo.enabled == true){
        	canvasMonstarInfo.enabled = false;
		} else{
        	canvasMonstarInfo.enabled = true;
        }
    }

    public void onClickOthersInfo(){
        if(canvasOthers.enabled == true){
        	canvasOthers.enabled = false;
		} else{
        	canvasOthers.enabled = true;
        }
    }
}
