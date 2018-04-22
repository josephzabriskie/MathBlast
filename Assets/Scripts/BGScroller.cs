using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroller : MonoBehaviour {

	public float speed = 0.5f;
	MeshRenderer mr;


	// Use this for initialization
	void Start () {
		this.mr = this.GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 offset = new Vector2(0, Time.time * speed);
		this.mr.material.mainTextureOffset = offset;
		
	}
}
