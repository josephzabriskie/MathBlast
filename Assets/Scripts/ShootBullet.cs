using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShootType{
	None,
	Straight,
	Targeted,
	CircleBurst
}

public class ShootBullet : MonoBehaviour {
	//All shooting types have these
	public GameObject bullet;
	public Transform spawnPoint;
	public float velocity = 10.0f;
	float velocityMult = 1.0f;
	float _velocity;
	public float startingDeg = 270.0f;
	float currentRads;
	AudioSource audioS;
	public AudioClip shootSound;

	public ShootType shootType;
	//CircleWave
	public int numBullets;

	//Target
	public GameObject targetGameObject;

	//Wavescript: Make sure that we spawn our bullet int the correct wave so it get's destoryed on level change
	Transform spawnParent;

	void Awake(){
		this.currentRads = this.startingDeg * Mathf.Deg2Rad;
		this.audioS = this.GetComponent<AudioSource> ();
	}

	void Start(){
		WaveScript ws = this.GetComponentInParent<WaveScript>();
		SessionManager sm = GetComponentInParent<SessionManager>();
		if (ws != null)
			this.spawnParent = ws.transform;	
		else if(sm != null)
			this.spawnParent = sm.transform;
		else
			this.spawnParent = gameObject.transform;
		updateVelMult(velocityMult);
	}

	public void updateVelMult(float mult){
		velocityMult = mult;
		_velocity = velocity * velocityMult;
	}

	public void Shoot(){
		switch(shootType){
		case ShootType.None:
			break;
		case ShootType.Straight:
			ShootStraight ();
			break;
		case ShootType.CircleBurst:
			ShootCircleWave ();
			break;
		case ShootType.Targeted:
			ShootTarget ();
			break;
		default:
			break;
		}
	}

	void ShootStraight(){
		GameObject bullet = Instantiate (this.bullet, this.spawnPoint.position, Quaternion.identity, this.spawnParent);
		bullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (_velocity * Mathf.Cos (currentRads), _velocity * Mathf.Sin (currentRads));
		this.audioS.PlayOneShot (this.shootSound);
	}

	void ShootCircleWave(){
		float maxRads = 6.283f;
		float rads = this.currentRads;
		for(int i = 0; i < this.numBullets; i++){
			//Debug.Log (string.Format ("Add bullet xvel: {0}, yvel: {1}", velocity * Mathf.Cos (rads), velocity * Mathf.Sin (rads)));
			GameObject bullet = Instantiate(this.bullet, this.spawnPoint.position, this.spawnPoint.rotation, this.spawnParent);
			bullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (_velocity * Mathf.Cos (rads), _velocity * Mathf.Sin (rads));
			rads += maxRads / this.numBullets;
		}
		this.audioS.PlayOneShot (this.shootSound);
	}

	void ShootTarget(){
		if(this.targetGameObject == null){
			Debug.LogWarning("Hey you didn't set a target to shoot! Skipping");
			return;
		}
		Vector3 targetPosition = this.targetGameObject.GetComponent<Transform> ().position;
		float rads = Mathf.Atan2 ((targetPosition.y - this.transform.position.y), (targetPosition.x - this.transform.position.x));
		GameObject bullet = Instantiate(this.bullet, this.spawnPoint.position, this.spawnPoint.rotation, this.spawnParent);
		bullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (_velocity * Mathf.Cos (rads), _velocity * Mathf.Sin (rads));
		this.audioS.PlayOneShot (this.shootSound);
	}
}
