using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScript : MonoBehaviour {
	public AnimationCurve ac;
	[Range(0.0f,20.0f)]
	public float hoverDist;
	[Range(0.01f, 20.0f)]
	public float hoverPeriod;
	Vector3 startPos;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos_new = new Vector3 (startPos.x, startPos.y, startPos.z);
		float acpos = Time.time % hoverPeriod / hoverPeriod;
		pos_new.y += ac.Evaluate(acpos) * hoverDist;
		transform.position = pos_new;
	}
}
