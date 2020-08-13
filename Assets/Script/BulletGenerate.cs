using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BulletGenerate : MonoBehaviour {
	//bulletPrefabと1~3のbullet
	private GameObject bulletPrefab;
	public GameObject bullet1;
	public GameObject bullet2;
	public GameObject bullet3;
	//ARカメラ
	public GameObject ARcamera;
	//wepon
	public static int weponPower;
	public static int plusPower;
	private int Bullet1CountMax;
	private int Bullet2CountMax;
	private int Bullet3CountMax;
	public static int plusBullet;
	private int BulletCount;
	private int Bullet1Count;
	private int Bullet2Count;
	private int Bullet3Count;
	private float time = 0;
	private int second = 0;
	public Text bulletCountText;
	public Text ReloadText;
	public Image ReloadImage;
	private bool isReloadSound1 = true;
	private bool isReloadSound2 = true;
	//ボタンpush
	private bool isPush = false;
	private int timeCount = 0;
	private float bulletSpeed;
    private int SHOT_INTERVAL;
	private int weponNo;
	private int Sound;
	//UserDataDB
    private UserDataDB userDataDB = new UserDataDB();

	//効果音
	private AudioSource[] audioSources;
 	private AudioSource BulletSound;
 	private AudioSource BulletReloadSound;
 	private AudioSource ExplosionSound;
	private AudioSource ItemHPSound;
	private AudioSource ItemAttackSound;
	private AudioSource ItemBulletSound;
	private AudioSource ItemShieldSound;
	private AudioSource ItemSkullSound;


    private Color BulletCountColor = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    private Color nonBulletCountColor = new Color(255f / 255f, 120f / 255f, 120f / 255f);

	void Start () {
		plusPower = 1;
		audioSources = GetComponents<AudioSource>();
		//効果音とWeponを初期化
		SettingChange();
		WeponChange();
 		BulletReloadSound = audioSources[3];
		ExplosionSound = audioSources[4];

		ItemHPSound = audioSources[5];
		ItemAttackSound = audioSources[6];
		ItemBulletSound = audioSources[7];
		ItemShieldSound = audioSources[8];
		ItemSkullSound = audioSources[9];

		timeCount = SHOT_INTERVAL;
		bulletCountText.color = BulletCountColor;

		Bullet1CountMax = 200;
		Bullet2CountMax = 60;
		Bullet3CountMax = 10;
		WeponInitialize();

	}


	void Update(){
    	timeCount++;
	    if (isPush) {
			switch(weponNo){
					case 1:
						BulletCount = Bullet1Count;
						break;
					case 2:
						BulletCount = Bullet2Count;
						break;
					case 3:
						BulletCount = Bullet3Count;
						break;	
			}
        	//カウントが発射間隔に達したら、弾を発射
        	if(timeCount > SHOT_INTERVAL && BulletCount > 0) {
				timeCount = 0;	//カウント初期化
				SoundBullet();
            	GameObject bullet = Instantiate(bulletPrefab);
		 		bullet.transform.position = ARcamera.transform.TransformPoint(0,3f,50f);
				bullet.GetComponent<Rigidbody>().AddForce(ARcamera.transform.TransformDirection(0,0,bulletSpeed), ForceMode.Impulse);
				Destroy(bullet, 5.0f);
				switch(weponNo){
					case 1:
						Bullet1Count--;
						BulletCount = Bullet1Count;
						bulletCountText.text = BulletCount+ "/"+ Bullet1CountMax;
						break;
					case 2:
						Bullet2Count--;
						BulletCount = Bullet2Count;
						bulletCountText.text = BulletCount+ "/"+ Bullet2CountMax;
						break;
					case 3:
						Bullet3Count--;
						BulletCount = Bullet3Count;
						bulletCountText.text = BulletCount+ "/"+ Bullet3CountMax;
						break;	
				}

        	}else if(BulletCount == 0){
				ReloadImage.enabled = true;
				ReloadText.enabled = true;
				ReloadText.text = "銃ボタンを3秒間\n\n長押ししてください";

				bulletCountText.color = nonBulletCountColor;

				time += Time.deltaTime;
				ReloadImage.fillAmount = time/3;
				second = (int)time%60;
				
				if(second == 1 && isReloadSound1){
					isReloadSound1 = false;
					BulletReloadSound.PlayOneShot (BulletReloadSound.clip);
				}
				if(second == 2 && isReloadSound2){
					isReloadSound2 = false;
					BulletReloadSound.PlayOneShot (BulletReloadSound.clip);
				}

				if(3 <= second){
					switch(weponNo){
					case 1:
						Bullet1Count = Bullet1CountMax;
						BulletCount = Bullet1CountMax;
						bulletCountText.text = BulletCount+ "/"+ Bullet1CountMax;

						break;
					case 2:
						Bullet2Count = Bullet2CountMax;
						BulletCount = Bullet2CountMax;
						bulletCountText.text = BulletCount+ "/"+ Bullet2CountMax;

						break;
					case 3:
						Bullet3Count = Bullet3CountMax;
						BulletCount = Bullet3CountMax;
						bulletCountText.text = BulletCount+ "/"+ Bullet3CountMax;
						break;	
					}
					ReloadImage.enabled = false;
					ReloadText.text = "";
					isReloadSound1 = true;
					isReloadSound2 = true;
					bulletCountText.color = BulletCountColor;

					time = 0;
					second = 0;
				}
			}
		}
		if(BulletCount == 0 && !isPush){
			time = 0;
			second = 0;
			ReloadImage.fillAmount = time/3;
			isReloadSound1 = true;
			isReloadSound2 = true;
		}
	}

	public void plusDoubleBullet(){
		Bullet1CountMax *= 2;
		Bullet2CountMax *= 2;
		Bullet3CountMax *= 2;
		if(Bullet1CountMax > 1000){
			Bullet1CountMax = 999;
		}if(Bullet2CountMax > 1000){
			Bullet2CountMax = 999;
		}if(Bullet3CountMax > 1000){
			Bullet3CountMax = 999;
		}
		weponNo = int.Parse(userDataDB.GetUserData("weponNo"));
		switch(weponNo){
			case 1:
				bulletCountText.text = Bullet1Count+ "/"+ Bullet1CountMax;
				break;
			case 2:
				bulletCountText.text = Bullet2Count+ "/"+ Bullet2CountMax;
				break;
			case 3:
				bulletCountText.text = Bullet3Count+ "/"+ Bullet3CountMax;
				break;	
		}
	}
	public void onClickDonw () {
		isPush = true;
	}

	public void onClickUP(){
		isPush = false;
	}

	public void SoundBullet(){
		if(Sound == 1){
			BulletSound.PlayOneShot (BulletSound.clip);
		}
	}

	public void SoundExplosion(){
		if(Sound == 1){
			ExplosionSound.PlayOneShot (ExplosionSound.clip);
		}
	}

	public void SoundItem(string itemName){
		if(Sound == 1){
			switch(itemName){
				case "HP":
					ItemHPSound.PlayOneShot (ItemHPSound.clip);
					break;
				case "attack":
					ItemAttackSound.PlayOneShot (ItemAttackSound.clip);
					break;
				case "bullet":
					ItemBulletSound.PlayOneShot (ItemBulletSound.clip);
					break;	
				case "shield":
					ItemShieldSound.PlayOneShot (ItemShieldSound.clip);
					break;	
				case "skull":
					ItemSkullSound.PlayOneShot (ItemSkullSound.clip);
					break;	
			}
		}
	}
	


	public void SettingChange(){
		
		Sound = int.Parse(userDataDB.GetUserData("sound"));
	}
	void WeponInitialize(){
		Bullet1Count = Bullet1CountMax;
		Bullet2Count = Bullet2CountMax;
		Bullet3Count = Bullet3CountMax;
		ReloadImage.enabled = false;
		ReloadText.enabled = false;
		switch(weponNo){
			case 1:
				Bullet1Count = Bullet1CountMax;
				BulletCount = Bullet1CountMax;
				bulletCountText.text = BulletCount+ "/"+ Bullet1CountMax;
				break;
			case 2:
				Bullet2Count = Bullet2CountMax;
				BulletCount = Bullet2CountMax;
				bulletCountText.text = BulletCount+ "/"+ Bullet2CountMax;
				break;
			case 3:
				Bullet3Count = Bullet3CountMax;
				BulletCount = Bullet3CountMax;
				bulletCountText.text = BulletCount+ "/"+ Bullet3CountMax;
				break;	
		}
	}

	public void WeponChange(){
		weponNo = int.Parse(userDataDB.GetUserData("weponNo"));
		switch(weponNo){
			case 1:
 				BulletSound = audioSources[0];
			    SHOT_INTERVAL = 7;
				bulletPrefab = bullet1;
				bulletSpeed = 500f;
				weponPower = 1;
				BulletCount = Bullet1Count;
				bulletCountText.text = Bullet1Count+ "/"+ Bullet1CountMax;
				break;
			case 2:
 				BulletSound = audioSources[1];
			    SHOT_INTERVAL = 20;
				bulletPrefab = bullet2;
				bulletSpeed = 400f;
				weponPower = 10;
				BulletCount = Bullet2Count;
				bulletCountText.text = Bullet2Count+ "/"+ Bullet2CountMax;
				break;
			case 3:
 				BulletSound = audioSources[2];
			    SHOT_INTERVAL = 50;
				bulletPrefab = bullet3;
				bulletSpeed = 200f;
				weponPower = 50;
				BulletCount = Bullet3Count;
				bulletCountText.text = Bullet3Count+ "/"+ Bullet3CountMax;
				break;	
		}
		if(BulletCount==0){
			ReloadImage.enabled = true;
			ReloadText.enabled = true;
			bulletCountText.color = nonBulletCountColor;

		}else if(BulletCount > 0){
			ReloadImage.enabled = false;
			ReloadText.enabled = false;
			bulletCountText.color = BulletCountColor;

		}
	}
}
