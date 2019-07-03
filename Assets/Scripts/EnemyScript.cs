using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : ShipBase {
	public AudioClip deathSound;
	Rigidbody2D rb;
	SpriteRenderer sr;
	public string movementType;
	//Circle around point
	Vector3 center;
	public float radius = 1.0f;
	public float rotateSpeed = 2.0f;
	private float angle = 0.0f;
	public bool cwPath = true;
	bool alive = true;
	//Line bool
	public bool linearReverse = false;
	public bool horizPath = true;

	//Shot vars
	ShootBullet sb;
	public bool allowShoot = true;
	public float shotDelay = 1.0f;
	float lastShotTime = 0.0f;

	//CharHeld is child
	HeldCharScript hcs;
	//We should have a parent waveScript
	WaveScript wc;


	//Implement ship base functions
	public override void OnDamage(){} // Do nothing
	public override void OnHeal(){} // Do nothing

	public override void OnKill(){
			Destroy(this.gameObject, 1.0f);
			GetComponent<AudioSource>().Play ();
			if(wc){
				wc.addChar (this.hcs.heldValue);
			}
			alive = false;
	}

	void Start(){
		this.sb = GetComponent<ShootBullet> ();
		this.rb = GetComponent<Rigidbody2D> ();
		this.sr = GetComponent<SpriteRenderer> ();
		this.center = this.transform.position; // Set center to where we currently are positioned
		this.hcs = GetComponentInChildren<HeldCharScript> ();
		this.wc = GetComponentInParent<WaveScript> ();
	}

	void Update(){
		if (Time.time - this.lastShotTime > this.shotDelay && this.alive && this.allowShoot){
			sb.Shoot ();
			this.lastShotTime = Time.time;
		}
		if (!alive){
			this.transform.Rotate(new Vector3(0,0,2));
			//don't need right now, looks alright
			this.transform.localScale = new Vector3(this.transform.localScale.x * 0.99f, this.transform.localScale.y * 0.99f, 1.0f);
		}
	}

	void FixedUpdate(){
		//Add these lines if you want to make the baddie fly away randomly, polish
		if(!alive){
			return;
		}
		if (this.movementType == "Circle")
			CircleUpdate();
		if (this.movementType == "Line")
			LineUpdate();
		//Else do nothing, just sits there
	}

	void CircleUpdate(){
		this.angle += this.rotateSpeed * Time.deltaTime;
		if (this.angle > 6.28319f)
			this.angle -= 6.28319f;
		Vector3 offset;
		if (this.cwPath){
			offset = new Vector3(Mathf.Sin (this.angle), Mathf.Cos (this.angle), 0) * this.radius;
		} else{
			offset = new Vector3(Mathf.Cos (this.angle), Mathf.Sin (this.angle), 0) * this.radius;
		}
		this.transform.position = this.center + offset;
	}

	void LineUpdate(){ //Still uses same vars as Circle update, radius, rotatespeed just add new bool for horiz/vert
		this.angle += this.rotateSpeed * Time.deltaTime;
		if (this.angle > 6.28319f)
			this.angle -= 6.28319f;
		Vector3 offset;
		if (this.horizPath){
			if (!this.linearReverse){
				offset = new Vector3(Mathf.Sin (this.angle), 0, 0) * this.radius;
			} else{
				offset = new Vector3(Mathf.Cos (this.angle), 0, 0) * this.radius;
			}
		} else{
			if (!this.linearReverse){
				offset = new Vector3(0, Mathf.Cos (this.angle), 0) * this.radius;
			}
			else{
				offset = new Vector3(0, Mathf.Sin (this.angle), 0) * this.radius;
			}
		}
		this.transform.position = this.center + offset;
	}
}
