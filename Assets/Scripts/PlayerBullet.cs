using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : BulletScript {

	ParticleSystem hitParticle;
	SpriteRenderer sr;
	Rigidbody2D rb;

	protected override void Awake(){
		base.Awake();
		hitParticle = transform.Find("HitParticle").GetComponent<ParticleSystem>();
		sr = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
	}

	public override void OnHit(){ // the cleanup hit stuff. Spawn particles and destroy
		Debug.Log("player bullet OnHit");
		StartCoroutine(IEOnHit());
	}

	IEnumerator IEOnHit(){
		Debug.Log("IE player bullet OnHit");
		sr.enabled = false;
		rb.velocity = new Vector2(0,0); // Stop the shot in place
		hitParticle.Play(); // Fizzle
		yield return null; // Give one frame to spawn the shtuff
		while(hitParticle.particleCount > 0){
			yield return null; // Check every frame if there are still particles left
		}
		Destroy(this.gameObject);
	}
}
