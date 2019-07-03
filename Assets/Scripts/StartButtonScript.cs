using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonScript : MonoBehaviour {

	public bool retro;

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.layer == 10) {//player shot
			GameController.instance.DelayedStart(retro);
		}
		else{
			//Start endless game
		}
		other.gameObject.GetComponent<BulletScript>().OnHit();
	}
}
