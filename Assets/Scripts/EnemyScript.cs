using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType{
	Static,
	Horizontal,
	Circle,
}

public struct EnemyConfig{
	public MovementType movementType;
	public ShootType shootType;
	public Vector3 position;
	public float fireDelayMult; // Make this shorter
	public float shipSpeedMult;
	public float bulletSpeedMult;
	public string heldValue;
	public EnemyConfig(MovementType movementType, ShootType shootType, Vector3 pos, float fdm, float ssm, float bsm, string hval){
		this.movementType = movementType;
		this.shootType = shootType;
		position = pos;
		fireDelayMult = fdm;
		shipSpeedMult = ssm;
		bulletSpeedMult = bsm;
		heldValue = hval;
	}
	public EnemyConfig(EnemyConfig ec, string hval){ // clumsy but fast fix for the lesson that structs are immutable...
		movementType = ec.movementType;
		shootType = ec.shootType;
		position = ec.position;
		fireDelayMult = ec.fireDelayMult;
		shipSpeedMult = ec.shipSpeedMult;
		bulletSpeedMult = ec.bulletSpeedMult;
		heldValue = hval;
	}
}

public class EnemyScript : ShipBase {
	public AudioClip deathSound;
	Rigidbody2D rb;
	SpriteRenderer sr;
	public MovementType movementType;
	//Circle around point
	Vector3 center;
	public float radius = 1.0f;
	public float rotateSpeed = 2.0f; // misnomer, should be base speed
	float speedMult;
	float _speed;
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

	TextMesh heldValMesh;
	string heldVal;

	//How to inform game mgmt of what's going on
	public delegate void EnemyKillCallback(GameObject go, string held);
	public EnemyKillCallback OnKillCB = null;

	//Implement ship base functions
	public override void OnDamage(){} // Do nothing
	public override void OnHeal(){} // Do nothing

	public override void OnKill(){
		if(OnKillCB != null){
			OnKillCB(gameObject, heldVal);
		}
		rb.simulated = false; // no more collisions for this one
		alive = false;
		Destroy(this.gameObject, 1.0f);
		GetComponent<AudioSource>().Play();
	}

	public void ApplyConfig(EnemyConfig ec){
		//Debug.LogFormat("ApplyingEnemy config: {0}", ec.heldValue);
		movementType = ec.movementType;
		sb.shootType = ec.shootType;
		transform.position = ec.position;
		
		sb.updateVelMult(ec.bulletSpeedMult);
		UpdateSpeedMult(ec.shipSpeedMult);
		SetHeldVal(ec.heldValue);
		// ec.fireDelayMult; //TODO implement
	}

	protected override void Awake(){ // If you want to use awake, add stuff after base call
		base.Awake();
		this.sb = GetComponent<ShootBullet> ();
		this.rb = GetComponent<Rigidbody2D> ();
		this.sr = GetComponent<SpriteRenderer>();
		this.center = this.transform.position; // Set center to where we currently are positioned
		this.heldValMesh = GetComponentInChildren<TextMesh>();
		HeldCharScript hc = GetComponentInChildren<HeldCharScript>();
		if(hc != null){ // Legacy we need to just set our heldVal to the initalized val
			heldVal = hc.heldValue;
		}
	}

	void SetHeldVal(string newVal){
		heldVal = newVal;
		heldValMesh.text = heldVal;
	}

	public void SetTargetGO(GameObject target){
		this.sb.targetGameObject = target;
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
		if(!alive){
			return;
		}
		switch(movementType){
		case MovementType.Circle:
			CircleUpdate();
			break;
		case MovementType.Horizontal:
			LineUpdate();
			break;
		case MovementType.Static:
			break; //EzPz
		default: // do nothing default
			break;
		}
	}

	public void UpdateSpeedMult(float mult){
		speedMult = mult;
		_speed = speedMult * rotateSpeed;
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
