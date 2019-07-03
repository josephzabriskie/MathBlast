using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ShipBase {

	//Movement vars
	private Rigidbody2D rb;
	float accel = 10f;
	float decel = 1.0f;
	float maxSpeed = 7.0f;
	public bool allowMove = true;

	//Shoot vars
	ShootBullet sb;
	public bool allowShoot = true;
	float lastShotTime = 0.0f;
	float shotDelay = 0.5f;

	public struct PlayerInput{
		public bool moving; // Is there either horizontal or vertical axis input
		public float angle; //Angle in radians of input (WASD or Joy)
		public bool fire1;
		public PlayerInput(bool m, float ia, bool f1){
			this.moving = m;
			this.angle = ia;
			this.fire1 = f1;
		}
	}

	PlayerInput pi;
	Animator anim;
	public AudioClip hitSound;
	AudioSource auds;

	PlayerController(){
		healthMax = 3;
	}

	// Use this for initialization
	void Start () {
		this.rb = this.GetComponent<Rigidbody2D> ();
		this.sb = GetComponent<ShootBullet> ();
		this.lastShotTime = Time.time; // Doing this so that when a wave loads the player doesn't double shoot
		this.anim = GetComponent<Animator>();
		this.auds = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update () {
		this.pi = getPlayerInput ();
		//Update animation controller
		anim.SetFloat("HorizVelBlend", rb.velocity.x);
	}

	//----------Health stuff
	public override void OnDamage(){
		auds.PlayOneShot(this.hitSound);
		UICharSystem.instance.updateHealthBar (health);
	}

	public override void OnKill(){
		EventController.instance.StopEventController(false);
		
	}

	public override void OnHeal(){
		UICharSystem.instance.updateHealthBar (health);
	}

	//-----------Movement stuff
	void FixedUpdate(){
		if (this.pi.moving && this.allowMove) {
			//Debug.LogFormat("Fixed update move angle:{0}, moving: {1}, fire: {2}", pi.angle, pi.moving, pi.fire1);
			float x_mult = Mathf.Cos (pi.angle);
			x_mult = (Mathf.Abs (x_mult) > 0.001f) ? x_mult : 0;
			float y_mult = Mathf.Sin (pi.angle);
			y_mult = (Mathf.Abs (y_mult) > 0.001f) ? y_mult : 0;
			this.rb.velocity = new Vector2 (this.maxSpeed * x_mult, this.maxSpeed * y_mult);

			//Oh my god fix this later, driving me insane, keep it simple, stupid
			/*
			Vector2 target_v = new Vector2(x_mult * this.maxSpeed, y_mult * this.maxSpeed);
			Vector2 new_v = rb.velocity;
			int x_dir = ((target_v.x - this.rb.velocity.x) > 1) ? 1 : -1;
			float new_v_x = new_v.x + x_dir * this.accel;
			new_v.x = (new_v_x < target_v.x) ? new_v_x : target_v.x;
			int y_dir = ((target_v.y - this.rb.velocity.y) > 1) ? 1 : -1;
			float new_v_y = new_v.y + y_dir * this.accel;
			new_v.y = (new_v_y < target_v.y) ? new_v_y : target_v.y;
			this.rb.velocity = new_v;
			*/
		} else { // SLOW DOWN
			this.rb.velocity = new Vector2(0,0);
		}

		if (this.pi.fire1 && Time.time - this.lastShotTime > this.shotDelay && this.allowShoot){
			sb.Shoot ();
			this.lastShotTime = Time.time;
		}
			
	}

	float getVectorSpeed (Vector2 xyvel){
		return Mathf.Sqrt (Mathf.Pow (xyvel.x, 2) + Mathf.Pow (xyvel.y, 2));
	}
	
	bool valOppZero(float val1, float val2){
		return ((val1 > 0 && val2 < 0) || (val1 < 0 && val2 > 0)) ? true : false;
	}

	PlayerInput getPlayerInput(){
		PlayerInput ret_pi = new PlayerInput (false, 0, false);
		float x = Input.GetAxis ("Horizontal");
		float y = Input.GetAxis ("Vertical");
		ret_pi.angle = Mathf.Atan2 (y, x);
		ret_pi.moving = (!(x == 0 && y == 0)) ? true : false;
		ret_pi.fire1 = (Input.GetAxis ("Fire1") != 0) ? true : false;
		return ret_pi;
	}

	void OnCollisionEnter2D (Collision2D coll){
		if (coll.collider.gameObject.layer == 11){ //current layer for enemy
			base.DoDamage(healthMax); // insta kill
		}
	}
}
