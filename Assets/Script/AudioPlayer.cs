using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioPlayer : MonoBehaviour {

	private AudioSource StartSound;
    private AudioSource ErrorSound;
    private AudioSource SelectSound;
    private AudioSource Bullet1Sound;
    private AudioSource Bullet2Sound;
    private AudioSource Bullet3Sound;
    private AudioSource BulletReloadSound;

    private int Sound;
    private UserDataDB userDataDB = new UserDataDB();

	// Use this for initialization
	void Start () {
		AudioSource[] audioSources = GetComponents<AudioSource>();
 	    StartSound = audioSources[0];
 	    ErrorSound = audioSources[1];
 	    SelectSound = audioSources[2];
		if(SceneManager.GetActiveScene().name != "Start"){
			Bullet1Sound = audioSources[3];
			Bullet2Sound = audioSources[4];
			Bullet3Sound = audioSources[5];
			BulletReloadSound = audioSources[6];
		}
	}

	public void onStartSound(){
		if(userDataDB.GetUserData("sound") == "1"){
			StartSound.PlayOneShot(StartSound.clip);
		}
	}

	public void onErrorSound(){
		if(userDataDB.GetUserData("sound") == "1"){
			ErrorSound.PlayOneShot(ErrorSound.clip);
		}	
	}

	public void onSelectSound(){
		if(userDataDB.GetUserData("sound") == "1"){
			SelectSound.PlayOneShot(SelectSound.clip);
		}	
	}

	public void onBullet1Sound(){
		if(userDataDB.GetUserData("sound") == "1"){
			Bullet1Sound.PlayOneShot(Bullet1Sound.clip);
		}
	}
	public void onBullet2Sound(){
		if(userDataDB.GetUserData("sound") == "1"){
			Bullet2Sound.PlayOneShot(Bullet2Sound.clip);
		}
	}
	public void onBullet3Sound(){
		if(userDataDB.GetUserData("sound") == "1"){
			Bullet3Sound.PlayOneShot(Bullet3Sound.clip);
		}
	}
	public void onBulletReloadSound(){
		if(userDataDB.GetUserData("sound") == "1"){
			BulletReloadSound.PlayOneShot(BulletReloadSound.clip);
		}
	}
}
