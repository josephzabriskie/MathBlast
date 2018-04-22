using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBullet : MonoBehaviour {
	//All shooting types have these
	public GameObject bullet;
	public Transform spawnPoint;
	public float velocity = 10.0f;
	public float startingDeg = 270.0f;
	float currentRads;
	AudioSource audioS;
	public AudioClip shootSound;

	public string shootType = "Straight"; //Straight, CircleWave, Targeted
	//CircleWave
	public int numBullets;

	//Target
	public GameObject targetGameObject;



	void Start(){
		this.currentRads = this.startingDeg * Mathf.Deg2Rad;
		this.audioS = this.GetComponent<AudioSource> ();
	}

	public void Shoot(){
		if (this.shootType == "Straight") {
			ShootStraight ();
		}
		else if (this.shootType == "CircleWave") {
			ShootCircleWave ();
		}
		else if (this.shootType == "Target") {
			ShootTarget ();
		}
	}

	void ShootStraight(){
		GameObject bullet = Instantiate (this.bullet, this.spawnPoint.position, this.spawnPoint.rotation);
		bullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (velocity * Mathf.Cos (currentRads), velocity * Mathf.Sin (currentRads));
		this.audioS.PlayOneShot (this.shootSound);
	}

	void ShootCircleWave(){
		float maxRads = 6.283f;
		float rads = this.currentRads;
		for(int i = 0; i < this.numBullets; i++){
			//Debug.Log (string.Format ("Add bullet xvel: {0}, yvel: {1}", velocity * Mathf.Cos (rads), velocity * Mathf.Sin (rads)));
			GameObject bullet = Instantiate(this.bullet, this.spawnPoint.position, this.spawnPoint.rotation);
			bullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (velocity * Mathf.Cos (rads), velocity * Mathf.Sin (rads));
			rads += maxRads / this.numBullets;
		}
		this.audioS.PlayOneShot (this.shootSound);
	}

	void ShootTarget(){
		Vector3 targetPosition = this.targetGameObject.GetComponent<Transform> ().position;
		float rads = Mathf.Atan2 ((targetPosition.y - this.transform.position.y), (targetPosition.x - this.transform.position.x));
		GameObject bullet = Instantiate(this.bullet, this.spawnPoint.position, this.spawnPoint.rotation);
		bullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (velocity * Mathf.Cos (rads), velocity * Mathf.Sin (rads));
		this.audioS.PlayOneShot (this.shootSound);
	}
		
}
