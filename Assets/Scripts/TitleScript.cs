using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScript : MonoBehaviour {
	RectTransform rt;
	float hoverAmount = 0.3f;
	float speed = 1.0f;

	// Use this for initialization
	void Start () {
		this.rt = GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos_new = new Vector3 (this.rt.position.x, this.rt.position.y, this.rt.position.z);
		pos_new.y += Mathf.Sin (Time.time * this.speed) * this.hoverAmount;
		this.rt.position = pos_new;
	}
}
