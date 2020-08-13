using UnityEngine;

public class tric : MonoBehaviour
{
	public AudioClip Waterflush, Hit_jaw, Hit_head, Hit_tail, Medstep, Medsplash, Sniff2, Chew, Slip, Largestep, Largesplash, Idleherb, Tric1, Tric2, Tric3, Tric4;
	Transform Neck0, Neck1, Neck2, Neck3, Head, Tail0, Tail1, Tail2, Tail3, Tail4, Tail5, Tail6, Tail7, Tail8, 
	Left_Arm0, Right_Arm0, Left_Arm1, Right_Arm1, Left_Hand, Right_Hand, 
	Left_Hips, Right_Hips, Left_Leg, Right_Leg, Left_Foot, Right_Foot;
	float crouch, spineX, spineY, tailX; bool reset;
	const float MAXYAW=20, MAXPITCH=15, MAXCROUCH=4, MAXANG=2, ANGT=0.025f;

	Vector3 dir;
	shared shared;
	AudioSource[] source;
	Animator anm;
	Rigidbody body;

	//*************************************************************************************************************************************************
	//Get components
	void Start()
	{
		Left_Hips = transform.Find ("Tric/root/pelvis/left hips");
		Right_Hips = transform.Find ("Tric/root/pelvis/right hips");
		Left_Leg  = transform.Find ("Tric/root/pelvis/left hips/left leg");
		Right_Leg = transform.Find ("Tric/root/pelvis/right hips/right leg");
		
		Left_Foot = transform.Find ("Tric/root/pelvis/left hips/left leg/left foot");
		Right_Foot = transform.Find ("Tric/root/pelvis/right hips/right leg/right foot");
		
		Left_Arm0 = transform.Find ("Tric/root/spine0/spine1/spine2/spine3/spine4/left arm0");
		Right_Arm0 = transform.Find ("Tric/root/spine0/spine1/spine2/spine3/spine4/right arm0");
		Left_Arm1 = transform.Find ("Tric/root/spine0/spine1/spine2/spine3/spine4/left arm0/left arm1");
		Right_Arm1 = transform.Find ("Tric/root/spine0/spine1/spine2/spine3/spine4/right arm0/right arm1");
		
		Left_Hand = transform.Find ("Tric/root/spine0/spine1/spine2/spine3/spine4/left arm0/left arm1/left hand");
		Right_Hand = transform.Find ("Tric/root/spine0/spine1/spine2/spine3/spine4/right arm0/right arm1/right hand");

		Tail0 = transform.Find ("Tric/root/pelvis/tail0");
		Tail1 = transform.Find ("Tric/root/pelvis/tail0/tail1");
		Tail2 = transform.Find ("Tric/root/pelvis/tail0/tail1/tail2");
		Tail3 = transform.Find ("Tric/root/pelvis/tail0/tail1/tail2/tail3");
		Tail4 = transform.Find ("Tric/root/pelvis/tail0/tail1/tail2/tail3/tail4");
		Tail5 = transform.Find ("Tric/root/pelvis/tail0/tail1/tail2/tail3/tail4/tail5");
		Tail6 = transform.Find ("Tric/root/pelvis/tail0/tail1/tail2/tail3/tail4/tail5/tail6");
		Tail7 = transform.Find ("Tric/root/pelvis/tail0/tail1/tail2/tail3/tail4/tail5/tail6/tail7");
		Tail8 = transform.Find ("Tric/root/pelvis/tail0/tail1/tail2/tail3/tail4/tail5/tail6/tail7/tail8");

		Neck0 = transform.Find ("Tric/root/spine0/spine1/spine2/spine3/spine4/neck0");
		Neck1 = transform.Find ("Tric/root/spine0/spine1/spine2/spine3/spine4/neck0/neck1");
		Neck2 = transform.Find ("Tric/root/spine0/spine1/spine2/spine3/spine4/neck0/neck1/neck2");
		Neck3 = transform.Find ("Tric/root/spine0/spine1/spine2/spine3/spine4/neck0/neck1/neck2/neck3");
		Head = transform.Find ("Tric/root/spine0/spine1/spine2/spine3/spine4/neck0/neck1/neck2/neck3/head");

		source = GetComponents<AudioSource>();
		shared= GetComponent<shared>();
		body=GetComponent<Rigidbody>();
		anm=GetComponent<Animator>();
	}

	//*************************************************************************************************************************************************
	//Play sound
	void OnCollisionStay(Collision col)
	{
		int rndPainsnd=Random.Range(0, 4); AudioClip painSnd=null;
		switch (rndPainsnd) { case 0: painSnd=Tric1; break; case 1: painSnd=Tric2; break; case 2: painSnd=Tric3; break; case 3: painSnd=Tric4; break; }
		shared.ManageCollision(col, MAXPITCH, MAXCROUCH, source, painSnd, Hit_jaw, Hit_head, Hit_tail);
	}
	void PlaySound(string name, int time)
	{
		if(time==shared.currframe && shared.lastframe!=shared.currframe)
		{
			switch (name)
			{
			case "Step": source[1].pitch=Random.Range(0.75f, 1.25f); 
				if(shared.IsInWater) source[1].PlayOneShot(Waterflush, Random.Range(0.25f, 0.5f));
				else if(shared.IsOnWater) source[1].PlayOneShot(Medsplash, Random.Range(0.25f, 0.5f));
				else if(shared.IsOnGround) source[1].PlayOneShot(Medstep, Random.Range(0.25f, 0.5f));
				shared.lastframe=shared.currframe; break;
			case "Slip": source[1].pitch=Random.Range(1.0f, 1.25f); source[1].PlayOneShot(shared.IsOnWater|shared.IsInWater?Largesplash:Slip, 0.5f);
				shared.lastframe=shared.currframe; break;
			case "Die": source[1].pitch=Random.Range(1.5f, 1.75f); source[1].PlayOneShot(shared.IsOnWater|shared.IsInWater?Largesplash:Largestep, 1.0f);
				shared.lastframe=shared.currframe; shared.IsDead=true; break;
			case "Sniff": source[0].pitch=Random.Range(1.2F, 1.5f); source[0].PlayOneShot(Sniff2, 0.25f);
				shared.lastframe=shared.currframe; break;
			case "Chew": source[0].pitch=Random.Range(1.0f, 1.25f); source[0].PlayOneShot(Chew, 0.5f);
				shared.lastframe=shared.currframe; break;
			case "Sleep": source[0].pitch=Random.Range(1.5f, 1.75f); source[0].PlayOneShot(Idleherb,0.25f);
				shared.lastframe=shared.currframe; break;
			case "Growl": int rnd = Random.Range (0, 4); source[0].pitch=Random.Range(1.0f, 1.25f);
				if(rnd==0)source[0].PlayOneShot(Tric1, 1.0f);
				else if(rnd==1)source[0].PlayOneShot(Tric2, 1.0f);
				else if(rnd==2)source[0].PlayOneShot(Tric3, 1.0f);
				else source[0].PlayOneShot(Tric4, 1.0f);
				shared.lastframe=shared.currframe; break;
			}
		}
	}

	//*************************************************************************************************************************************************
	// Add forces to the Rigidbody
	void FixedUpdate ()
	{
		if(!shared.IsActive) { body.Sleep(); return; }
		reset=false; shared.IsAttacking=false; shared.IsConstrained= false;
		AnimatorStateInfo CurrAnm=anm.GetCurrentAnimatorStateInfo(0);
		AnimatorStateInfo NextAnm=anm.GetNextAnimatorStateInfo(0);

		//Set mass
		if(shared.IsInWater) { body.mass=10; body.drag=1; body.angularDrag=1; }
		else { body.mass=1; body.drag=4; body.angularDrag=4; }
		//Set Y position
		if(shared.IsOnGround) //Ground
		{ dir=transform.forward; body.AddForce(Vector3.up*(shared.posY-transform.position.y)*64); }
		else if(shared.IsInWater | shared.IsOnWater) //Water
		{
			dir=transform.forward; body.AddForce(Vector3.up*(shared.posY-transform.position.y)*8);
			if(shared.Health>0) { anm.SetInteger ("Move", 2); shared.Health-=0.01f; }
		} else body.AddForce(-Vector3.up*Mathf.Lerp(dir.y, 128, 1.0f)); //Falling
		
		//Stopped
		if(NextAnm.IsName("Tric|Idle1A") | NextAnm.IsName("Tric|Idle2A") | CurrAnm.IsName("Tric|Idle1A") | CurrAnm.IsName("Tric|Idle2A") | 
			CurrAnm.IsName("Tric|Die1") | CurrAnm.IsName("Tric|Die2"))
		{
			if(CurrAnm.IsName("Tric|Die1")) { reset=true; shared.IsConstrained=true; if(!shared.IsDead) { PlaySound("Growl", 2); PlaySound("Die", 12); } }
			else if(CurrAnm.IsName("Tric|Die2")) { reset=true; shared.IsConstrained=true; if(!shared.IsDead) { PlaySound("Growl", 2); PlaySound("Die", 10); } }
		}
		
		//Forward
		else if(CurrAnm.IsName("Tric|Walk") | CurrAnm.IsName("Tric|WalkGrowl") | CurrAnm.IsName("Tric|Step1") | CurrAnm.IsName("Tric|Step2") |
		   CurrAnm.IsName("Tric|ToEatA") | CurrAnm.IsName("Tric|ToEatC") | CurrAnm.IsName("Tric|ToIdle2C"))
		{
			transform.rotation*= Quaternion.Euler(0, anm.GetFloat("Turn")*MAXANG, 0);
			body.AddForce(transform.forward*18*transform.localScale.x*anm.speed);
			if(CurrAnm.IsName("Tric|WalkGrowl")) { PlaySound("Growl", 1); PlaySound("Step", 6); PlaySound("Step", 13); }
			else if(CurrAnm.IsName("Tric|Walk")) { PlaySound("Step", 6); PlaySound("Step", 13); }
			else PlaySound("Step", 9);
		}

		//Running
		else if(NextAnm.IsName("Tric|Run") | CurrAnm.IsName("Tric|Run") | CurrAnm.IsName("Tric|RunGrowl") |
					CurrAnm.IsName("Tric|StepAtk1") | CurrAnm.IsName("Tric|StepAtk2") | CurrAnm.IsName("Tric|RunAtk1") | 
					(CurrAnm.IsName("Tric|RunAtk2") && CurrAnm.normalizedTime < 0.5))
		{
			transform.rotation*= Quaternion.Euler(0, anm.GetFloat("Turn")*MAXANG, 0);
			if((!CurrAnm.IsName("Tric|StepAtk1") && !CurrAnm.IsName("Tric|StepAtk2")) | ((CurrAnm.IsName("Tric|StepAtk1") | CurrAnm.IsName("Tric|StepAtk2"))
				&& CurrAnm.normalizedTime <0.3)) body.AddForce(transform.forward*100*transform.localScale.x*anm.speed);
			if(CurrAnm.IsName("Tric|Run") | NextAnm.IsName("Tric|Run")) { PlaySound("Step", 3); PlaySound("Step", 6); }
			else if(CurrAnm.IsName("Tric|RunGrowl")) { PlaySound("Growl", 2); PlaySound("Step", 3); PlaySound("Step", 6); }
			else if(CurrAnm.IsName("Tric|RunAtk2")) { shared.IsAttacking=true; PlaySound("Growl", 2); PlaySound("Slip", 6); }
			else { shared.IsAttacking=true; PlaySound("Growl", 2); PlaySound("Step", 3); PlaySound("Step", 6); }
		}
		
		//Backward
		else if(CurrAnm.IsName("Tric|Step1-") | CurrAnm.IsName("Tric|Step2-") | CurrAnm.IsName("Tric|ToSit1") |
		   	CurrAnm.IsName("Tric|ToIdle1C") | CurrAnm.IsName("Tric|ToIdle1D"))
		{
			transform.rotation*= Quaternion.Euler(0, anm.GetFloat("Turn")*MAXANG, 0);
			body.AddForce(transform.forward*-10*transform.localScale.x*anm.speed);
			 PlaySound("Step", 9);
		}

		//Strafe/Turn right
		else if(CurrAnm.IsName("Tric|Strafe1-") | CurrAnm.IsName("Tric|Strafe2+"))
		{
			transform.rotation*= Quaternion.Euler(0, anm.GetFloat("Turn")*MAXANG, 0);
			body.AddForce(transform.right*8*transform.localScale.x*anm.speed);
			PlaySound("Step", 5); PlaySound("Step", 12);
		}

		//Strafe/Turn left
		else if(CurrAnm.IsName("Tric|Strafe1+") | CurrAnm.IsName("Tric|Strafe2-"))
		{
			transform.rotation*= Quaternion.Euler(0, anm.GetFloat("Turn")*MAXANG, 0);
			body.AddForce(transform.right*-8*transform.localScale.x*anm.speed);
			PlaySound("Step", 5); PlaySound("Step", 12);
		}

		//Stop
		else if(CurrAnm.IsName("Tric|EatA")) PlaySound("Chew", 10);
		else if(CurrAnm.IsName("Tric|EatB")) { PlaySound("Chew", 1); PlaySound("Chew", 4); PlaySound("Chew", 8); PlaySound("Chew", 12); }
		else if(CurrAnm.IsName("Tric|EatC")) reset=true;
		else if(CurrAnm.IsName("Tric|ToSit")) shared.IsConstrained=true;
		else if(CurrAnm.IsName("Tric|SitIdle")) shared.IsConstrained=true;
		else if(CurrAnm.IsName("Tric|Sleep")) { reset=true; shared.IsConstrained=true; PlaySound("Sleep", 2); }
		else if(CurrAnm.IsName("Tric|SitGrowl")) { shared.IsConstrained=true; PlaySound("Growl", 2); }
		else if(CurrAnm.IsName("Tric|Idle1B")) { PlaySound("Growl", 2); PlaySound("Slip", 3); }
		else if(CurrAnm.IsName("Tric|Idle1C")) { PlaySound("Growl", 2); PlaySound("Step", 5); PlaySound("Step", 6); PlaySound("Sniff", 9); }
		else if(CurrAnm.IsName("Tric|Idle1D")) { PlaySound("Sniff", 1); PlaySound("Growl", 4); PlaySound("Step", 9); PlaySound("Step", 11); }
		else if(CurrAnm.IsName("Tric|Idle2B")) { PlaySound("Growl", 2); PlaySound("Slip", 3); }
		else if(CurrAnm.IsName("Tric|Idle2C")) { reset=true; PlaySound("Sniff", 1); }
		else if(CurrAnm.IsName("Tric|IdleAtk1") | CurrAnm.IsName("Tric|IdleAtk2")) { shared.IsAttacking=true; PlaySound("Growl", 2); PlaySound("Step", 5); PlaySound("Step", 6); }
		else if(CurrAnm.IsName("Tric|Die1-")) { PlaySound("Growl", 3);  shared.IsDead=false; }
		else if(CurrAnm.IsName("Tric|Die2-")) { PlaySound("Growl", 3);  shared.IsDead=false; }
	}

	
	void LateUpdate()
	{
		//*************************************************************************************************************************************************
		// Bone rotation
		if(!shared.IsActive) return;

		//Set const varialbes to shared script
		shared.crouch_max=MAXCROUCH;
		shared.ang_t=ANGT;
		shared.yaw_max=MAXYAW;
		shared.pitch_max=MAXPITCH;

		if(shared.lastHit!=0)	//Taking damage animation
		{
			crouch=Mathf.Lerp(crouch, (MAXCROUCH*transform.localScale.x)/2, 1.0f);
			shared.lastHit--; Head.GetChild(0).transform.rotation*= Quaternion.Euler(-shared.lastHit, 0, 0);
		}
		else if(reset) //Reset
		{
			anm.SetFloat("Turn", Mathf.Lerp(anm.GetFloat("Turn"), 0.0f, ANGT*3));
			spineX=Mathf.Lerp(spineX, 0.0f, ANGT);
			spineY=Mathf.Lerp(spineY, 0.0f, ANGT);
			crouch=Mathf.Lerp(crouch, 0, ANGT);
		}
		else
		{
			shared.TargetLooking(spineX, spineY,crouch);
			spineX=Mathf.Lerp(spineX, shared.spineX_T, ANGT);
			spineY=Mathf.Lerp(spineY, shared.spineY_T, ANGT);
			crouch=Mathf.Lerp(crouch, shared.crouch_T, ANGT);
		}
		
		//Save head position befor transformation
		shared.fixedHeadPos=Head.position;

		//Spine rotation
		float spineZ =spineY*spineX/MAXYAW;
		Neck0.transform.rotation*= Quaternion.Euler(spineY, -spineZ, -spineX);
		Neck1.transform.rotation*= Quaternion.Euler(spineY, -spineZ, -spineX);
		Neck2.transform.rotation*= Quaternion.Euler(spineY, -spineZ, -spineX);
		Neck3.transform.rotation*= Quaternion.Euler(spineY, -spineZ, -spineX);
		Head.transform.rotation*= Quaternion.Euler(spineY, -spineZ, -spineX);

		//Tail rotation
		tailX=Mathf.Lerp(tailX, anm.GetFloat("Turn")*MAXYAW, ANGT);
		Tail0.transform.rotation*= Quaternion.Euler(0, 0, tailX);
		Tail1.transform.rotation*= Quaternion.Euler(0, 0, tailX);
		Tail2.transform.rotation*= Quaternion.Euler(0, 0, tailX);
		Tail3.transform.rotation*= Quaternion.Euler(0, 0, tailX);
		Tail4.transform.rotation*= Quaternion.Euler(0, 0, tailX);
		Tail5.transform.rotation*= Quaternion.Euler(0, 0, tailX);
		Tail6.transform.rotation*= Quaternion.Euler(0, 0, tailX);
		Tail7.transform.rotation*= Quaternion.Euler(0, 0, tailX);
		Tail8.transform.rotation*= Quaternion.Euler(0, 0, tailX);

		//IK feet (require "JP script extension" asset)
		shared.QuadIK( Right_Arm0, Right_Arm1, Right_Hand, Left_Arm0, Left_Arm1, Left_Hand, Right_Hips, Right_Leg, Right_Foot, Left_Hips, Left_Leg, Left_Foot, -0.5f*transform.localScale.x);
		//Check for ground layer
		shared.GetGroundAlt(true, crouch);

		//*************************************************************************************************************************************************
		// CPU (require "JP script extension" asset)
		if(shared.AI && shared.Health!=0) { shared.AICore(1, 2, 3, 4, 5, 6, 7); }
		//*************************************************************************************************************************************************
		// Human
		else if(shared.Health!=0) { shared.GetUserInputs(1, 2, 3, 4, 5, 6, 7); }
		//*************************************************************************************************************************************************
		//Dead
		else { anm.SetBool("Attack", false); anm.SetInteger ("Move", 0); anm.SetInteger ("Idle", -1); }
	}
}
