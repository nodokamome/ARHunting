using UnityEngine;

public class rex : MonoBehaviour
{
	public AudioClip Waterflush, Hit_jaw, Hit_head, Hit_tail, Bigstep, Largesplash, Largestep, Idlecarn, Bite, Swallow, Sniff1, Rex1, Rex2, Rex3, Rex4, Rex5;
	Transform Spine0, Spine1, Spine2, Neck0, Neck1, Neck2, Head, Tail2, Tail3, Tail4, Tail5, Tail6, 
	Left_Hips, Right_Hips, Left_Leg, Right_Leg, Left_Foot0, Right_Foot0, Left_Foot1, Right_Foot1;
	float crouch, spineX, spineY, tailX; bool reset;
	const float MAXYAW=25, MAXPITCH=12, MAXCROUCH=7, MAXANG=2, ANGT=0.075f;

	Vector3 dir;
	shared shared;
	AudioSource[] source;
	Animator anm;
	Rigidbody body;

	//*************************************************************************************************************************************************
	//Get components
	void Start()
	{
		Right_Hips = transform.Find ("Rex/root/tail0/tail1/right hips");
		Right_Leg = transform.Find ("Rex/root/tail0/tail1/right hips/right leg");
		Right_Foot0 = transform.Find ("Rex/root/tail0/tail1/right hips/right leg/right foot0");
		Right_Foot1 = transform.Find ("Rex/root/tail0/tail1/right hips/right leg/right foot0/right foot1");
		Left_Hips = transform.Find ("Rex/root/tail0/tail1/left hips");
		Left_Leg = transform.Find ("Rex/root/tail0/tail1/left hips/left leg");
		Left_Foot0 = transform.Find ("Rex/root/tail0/tail1/left hips/left leg/left foot0");
		Left_Foot1 = transform.Find ("Rex/root/tail0/tail1/left hips/left leg/left foot0/left foot1");

		Tail2 = transform.Find ("Rex/root/tail0/tail1/tail2");
		Tail3 = transform.Find ("Rex/root/tail0/tail1/tail2/tail3");
		Tail4 = transform.Find ("Rex/root/tail0/tail1/tail2/tail3/tail4");
		Tail5 = transform.Find ("Rex/root/tail0/tail1/tail2/tail3/tail4/tail5");
		Tail6 = transform.Find ("Rex/root/tail0/tail1/tail2/tail3/tail4/tail5/tail6");
		Spine0 = transform.Find ("Rex/root/spine0");
		Spine1 = transform.Find ("Rex/root/spine0/spine1");
		Spine2 = transform.Find ("Rex/root/spine0/spine1/spine2");
		Neck0 = transform.Find ("Rex/root/spine0/spine1/spine2/spine3/neck0");
		Neck1 = transform.Find ("Rex/root/spine0/spine1/spine2/spine3/neck0/neck1");
		Neck2 = transform.Find ("Rex/root/spine0/spine1/spine2/spine3/neck0/neck1/neck2");
		Head = transform.Find ("Rex/root/spine0/spine1/spine2/spine3/neck0/neck1/neck2/head");

		source = GetComponents<AudioSource>();
		shared= GetComponent<shared>();
		body=GetComponent<Rigidbody>();
		anm=GetComponent<Animator>();
	}

	//*************************************************************************************************************************************************
	//Play sound
	void OnCollisionStay(Collision col)
	{
		int rndPainsnd=Random.Range(0, 3); AudioClip painSnd=null;
		switch (rndPainsnd) { case 0: painSnd=Rex2; break; case 1: painSnd=Rex3; break; case 2: painSnd=Rex4; break; }
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
				else if(shared.IsOnWater) source[1].PlayOneShot(Largesplash, Random.Range(0.25f, 0.5f));
				else if(shared.IsOnGround) source[1].PlayOneShot(Bigstep, Random.Range(0.25f, 0.5f));
				shared.lastframe=shared.currframe; break;
			case "Bite": source[1].pitch=Random.Range(0.5f, 0.75f); source[1].PlayOneShot(Bite, 2.0f);
				shared.lastframe=shared.currframe; break;
			case "Die": source[1].pitch=Random.Range(1.0f, 1.25f); source[1].PlayOneShot(shared.IsOnWater|shared.IsInWater?Largesplash:Largestep, 1.0f);
				shared.lastframe=shared.currframe; shared.IsDead=true; break; 
			case "Food": source[0].pitch=Random.Range(1.0f, 1.25f); source[0].PlayOneShot(Swallow, 0.5f);
				shared.lastframe=shared.currframe; break;
			case "Sniff": source[0].pitch=Random.Range(1.0f, 1.25f); source[0].PlayOneShot(Sniff1, 0.5f);
				shared.lastframe=shared.currframe; break;
			case "Sleep": source[0].pitch=Random.Range(0.75f, 1.25f); source[0].PlayOneShot(Idlecarn, 0.25f);
				shared.lastframe=shared.currframe; break;
			case "Atk": int rnd1 = Random.Range (0, 2); source[0].pitch=Random.Range(0.75f, 1.75f);
				if(rnd1==0) source[0].PlayOneShot(Rex3, 0.5f);
				else source[0].PlayOneShot(Rex4,0.5f);
				shared.lastframe=shared.currframe; break;
			case "Growl": int rnd2 = Random.Range (0, 3); source[0].pitch=Random.Range(1.0f, 1.25f);
				if(rnd2==0)source[0].PlayOneShot(Rex1, 1.0f);
				else if(rnd2==1) source[0].PlayOneShot(Rex2, 1.0f);
				else source[0].PlayOneShot(Rex5, 1.0f);
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
		if(NextAnm.IsName("Rex|Idle1A") | NextAnm.IsName("Rex|Idle2A") | CurrAnm.IsName("Rex|Idle1A") | CurrAnm.IsName("Rex|Idle2A") |
			CurrAnm.IsName("Rex|Die1") | CurrAnm.IsName("Rex|Die2"))
		{
			if(CurrAnm.IsName("Rex|Die1")) { reset=true; shared.IsConstrained=true; if(!shared.IsDead) { PlaySound("Atk", 2); PlaySound("Die", 12); } }
			else if(CurrAnm.IsName("Rex|Die2")) { reset=true; shared.IsConstrained=true; if(!shared.IsDead) { PlaySound("Atk", 2); PlaySound("Die", 10); } }
		}

		//End Forward
		else if((CurrAnm.IsName("Rex|Step1+") && CurrAnm.normalizedTime > 0.5) |
		 (CurrAnm.IsName("Rex|Step2+") && CurrAnm.normalizedTime > 0.5) |
		 (CurrAnm.IsName("Rex|ToIdle1C") && CurrAnm.normalizedTime > 0.5) | 
		 (CurrAnm.IsName("Rex|ToIdle2B") && CurrAnm.normalizedTime > 0.5) |
		 (CurrAnm.IsName("Rex|ToIdle2D") && CurrAnm.normalizedTime > 0.5) |
		 (CurrAnm.IsName("Rex|ToEatA") && CurrAnm.normalizedTime > 0.5) |
		 (CurrAnm.IsName("Rex|ToEatC") && CurrAnm.normalizedTime > 0.5) |
		 (CurrAnm.IsName("Rex|StepAtk1") && CurrAnm.normalizedTime > 0.5) |
		 (CurrAnm.IsName("Rex|StepAtk2") && CurrAnm.normalizedTime > 0.5))
			PlaySound("Step", 9);

		//Forward
		else if(CurrAnm.IsName("Rex|Walk") | CurrAnm.IsName("Rex|WalkGrowl") |
		   (CurrAnm.IsName("Rex|Step1+") && CurrAnm.normalizedTime < 0.5) |
		   (CurrAnm.IsName("Rex|Step2+") && CurrAnm.normalizedTime < 0.5) |
		   (CurrAnm.IsName("Rex|ToIdle2B") && CurrAnm.normalizedTime < 0.5) |
		   (CurrAnm.IsName("Rex|ToIdle1C") && CurrAnm.normalizedTime < 0.5) | 
		   (CurrAnm.IsName("Rex|ToIdle2D") && CurrAnm.normalizedTime < 05) |
		   (CurrAnm.IsName("Rex|ToEatA") && CurrAnm.normalizedTime < 0.5) |
		   (CurrAnm.IsName("Rex|ToEatC") && CurrAnm.normalizedTime < 0.5))
		{
			transform.rotation*= Quaternion.Euler(0, anm.GetFloat("Turn")*MAXANG, 0);
			body.AddForce(transform.forward*48*transform.localScale.x*anm.speed);
			if(anm.GetCurrentAnimatorStateInfo(0).IsName("Rex|WalkGrowl")) { PlaySound("Growl", 1); PlaySound("Step", 6); PlaySound("Step", 13); }
			else if(anm.GetCurrentAnimatorStateInfo(0).IsName("Rex|Walk")) { PlaySound("Step", 6); PlaySound("Step", 13); }
			else { PlaySound("Step", 8); PlaySound("Step", 12); }
		}

		//Run
		else if(CurrAnm.IsName("Rex|Run") | CurrAnm.IsName("Rex|RunGrowl") |
		   CurrAnm.IsName("Rex|WalkAtk1") | CurrAnm.IsName("Rex|WalkAtk2") |
		   (CurrAnm.IsName("Rex|StepAtk1") && CurrAnm.normalizedTime < 0.6) |
		   (CurrAnm.IsName("Rex|StepAtk2") && CurrAnm.normalizedTime < 0.6))
		{
			transform.rotation*= Quaternion.Euler(0, anm.GetFloat("Turn")*MAXANG, 0);
			body.AddForce(transform.forward*128*transform.localScale.x*anm.speed);
			if(anm.GetCurrentAnimatorStateInfo(0).IsName("Rex|RunGrowl")) { PlaySound("Growl", 1); PlaySound("Step", 6); PlaySound("Step", 13); }
			else if(anm.GetCurrentAnimatorStateInfo(0).IsName("Rex|Run")) { PlaySound("Step", 6); PlaySound("Step", 13); }
			else if(CurrAnm.IsName("Rex|StepAtk1") | CurrAnm.IsName("Rex|StepAtk2")) { shared.IsAttacking=true; PlaySound("Atk", 2); PlaySound("Bite", 5); }
			else { shared.IsAttacking=true; PlaySound("Atk", 2); PlaySound("Step", 6); PlaySound("Bite", 9); PlaySound("Step", 13); }
		}

		//Backward
		else if((CurrAnm.IsName("Rex|Step1-") && CurrAnm.normalizedTime < 0.8) |
		   (CurrAnm.IsName("Rex|Step2-") && CurrAnm.normalizedTime < 0.8) |
		   (CurrAnm.IsName("Rex|ToSleep2")&& CurrAnm.normalizedTime < 0.8))
		{
			transform.rotation*= Quaternion.Euler(0, anm.GetFloat("Turn")*MAXANG, 0);
			body.AddForce(transform.forward*-48*transform.localScale.x*anm.speed);
			PlaySound("Step", 12);
		}

		//Strafe/Turn right
		else if(CurrAnm.IsName("Rex|Strafe1-") | CurrAnm.IsName("Rex|Strafe2+"))
		{
			transform.rotation*= Quaternion.Euler(0, anm.GetFloat("Turn")*MAXANG, 0);
			body.AddForce(transform.right*25*transform.localScale.x*anm.speed);
			PlaySound("Step", 6); PlaySound("Step", 13);
		}

		//Strafe/Turn left
		else if(CurrAnm.IsName("Rex|Strafe1+") | CurrAnm.IsName("Rex|Strafe2-"))
		{
			transform.rotation*= Quaternion.Euler(0, anm.GetFloat("Turn")*MAXANG, 0);
			body.AddForce(transform.right*-25*transform.localScale.x*anm.speed);
			PlaySound("Step", 6); PlaySound("Step", 13);
		}

		//Various
		else if(CurrAnm.IsName("Rex|EatA")) { reset=true; PlaySound("Food", 4); PlaySound("Bite", 5); }
		else if(CurrAnm.IsName("Rex|EatB") | CurrAnm.IsName("Rex|EatC")) reset=true;
		else if(CurrAnm.IsName("Rex|Sleep")) { reset=true; shared.IsConstrained=true; PlaySound("Sleep", 2); }
		else if(CurrAnm.IsName("Rex|ToSleep1") | CurrAnm.IsName("Rex|ToSleep2")) { reset=true; shared.IsConstrained=true; }
		else if(CurrAnm.IsName("Rex|ToIdle2A")) PlaySound("Sniff", 1);
		else if(CurrAnm.IsName("Rex|Idle1B")) PlaySound("Growl", 2);
		else if(CurrAnm.IsName("Rex|Idle1C")) { PlaySound("Sniff", 4); PlaySound("Sniff", 7); PlaySound("Sniff", 10);}
		else if(CurrAnm.IsName("Rex|Idle2B")) { reset=true; PlaySound("Bite", 4); PlaySound("Bite", 6); PlaySound("Bite", 8);}
		else if(CurrAnm.IsName("Rex|Idle2C")) PlaySound("Growl", 2);
		else if(CurrAnm.IsName("Rex|Idle2D")) { reset=true; PlaySound("Atk", 2); }
		else if(CurrAnm.IsName("Rex|IdleAtk1") | CurrAnm.IsName("Rex|IdleAtk2"))
		{ shared.IsAttacking=true; transform.rotation*= Quaternion.Euler(0, anm.GetFloat("Turn")*MAXANG, 0); PlaySound("Atk", 1); PlaySound("Step", 3); PlaySound("Bite", 6); } 
		else if(CurrAnm.IsName("Rex|Die1-")) { shared.IsConstrained=true; PlaySound("Growl", 3);  shared.IsDead=false; }
		else if(CurrAnm.IsName("Rex|Die2-")) { shared.IsConstrained=true; PlaySound("Growl", 3);  shared.IsDead=false; }
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
			shared.lastHit--; Head.GetChild(0).transform.rotation*= Quaternion.Euler(shared.lastHit, 0, 0);
			crouch=Mathf.Lerp(crouch, (MAXCROUCH*transform.localScale.x)/2, 1.0f);
		}
		else if(reset) //Reset
		{
			anm.SetFloat("Turn", Mathf.Lerp(anm.GetFloat("Turn"), 0.0f, ANGT/3));
			spineX=Mathf.Lerp(spineX, 0.0f, ANGT/3);
			spineY=Mathf.Lerp(spineY, 0.0f, ANGT/3);
			crouch=Mathf.Lerp(crouch, 0, ANGT/3);
		}
		else
		{
			shared.TargetLooking(spineX, spineY,crouch);
			spineX=Mathf.Lerp(spineX, shared.spineX_T, ANGT/3);
			spineY=Mathf.Lerp(spineY, shared.spineY_T, ANGT/3);
			crouch=Mathf.Lerp(crouch, shared.crouch_T, ANGT);
		}
		
		//Save head position befor transformation
		shared.fixedHeadPos=Head.position;

		//Spine rotation
		float spineZ =spineY*spineX/MAXYAW;
		Spine0.transform.rotation*= Quaternion.Euler(-spineY, spineZ, -spineX);
		Spine1.transform.rotation*= Quaternion.Euler(-spineY, spineZ, -spineX);
		Spine2.transform.rotation*= Quaternion.Euler(-spineY, spineZ, -spineX);
		Neck0.transform.rotation*= Quaternion.Euler(-spineY, spineZ, -spineX);
		Neck1.transform.rotation*= Quaternion.Euler(-spineY, spineZ, -spineX);
		Neck2.transform.rotation*= Quaternion.Euler(-spineY, spineZ, -spineX);
		Head.transform.rotation*= Quaternion.Euler(-spineY, spineZ, -spineX);
		//Tail rotation
		tailX=Mathf.Lerp(tailX, anm.GetFloat("Turn")*MAXYAW, ANGT/3);
		Tail2.transform.rotation*= Quaternion.Euler(0, 0, tailX);
		Tail3.transform.rotation*= Quaternion.Euler(0, 0, tailX);
		Tail4.transform.rotation*= Quaternion.Euler(0, 0, tailX);
		Tail5.transform.rotation*= Quaternion.Euler(0, 0, tailX);
		Tail6.transform.rotation*= Quaternion.Euler(0, 0, tailX);

	

		//IK feet (require "JP script extension" asset)
		shared.LargeBipedIK(Right_Hips, Right_Leg, Right_Foot0, Right_Foot1, Left_Hips, Left_Leg, Left_Foot0, Left_Foot1);
		//Check for ground layer
		shared.GetGroundAlt(false, crouch);

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

