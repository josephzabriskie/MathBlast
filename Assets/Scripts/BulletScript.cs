using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {
	float maxTime = 5;

	void Awake(){
		Destroy (this.gameObject, this.maxTime);
	}

	public void Hit(){
		Destroy (this.gameObject);
	}	
}
