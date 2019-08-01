using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {
	int PlayerLayer, PlayerBulletLayer, EnemyLayer, EnemyBulletLayer;

	public enum BulletOwner{
		player,
		enemy
	}
	public BulletOwner owner;
	[Range(0,10)]
	public int damage;
	public Vector2 spinMinMax; // What rotation do bullets spawn with? x = min y = max
	int targetLayer;
	float maxTime = 5;

	protected virtual void Awake(){ // Make
		//The name to layer isn't allowed to be called in constructor, I guess ...
		int PlayerLayer = LayerMask.NameToLayer("Player");
		int PlayerBulletLayer = LayerMask.NameToLayer("PlayerBullet");
		int EnemyLayer = LayerMask.NameToLayer("Enemy");
		int EnemyBulletLayer = LayerMask.NameToLayer("EnemyBullet");
		Destroy (this.gameObject, this.maxTime); // If we're still around after 5s, we've probably missed our mark

		GetComponent<Rigidbody2D>().AddTorque(Random.Range(spinMinMax.x, spinMinMax.y));
		if(owner == BulletOwner.player){
			gameObject.layer = PlayerBulletLayer;
			targetLayer = EnemyLayer;
		}
		else{
			gameObject.layer = EnemyBulletLayer;
			targetLayer = PlayerLayer;
		}

	}

	public virtual void OnHit(){ // the cleanup hit stuff. Spawn particles and destroy
		Destroy(this.gameObject);
	}

	//The bullet should call these!
	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.layer == targetLayer){
			ShipBase ship = other.gameObject.GetComponent<ShipBase>();
			if(ship == null){
				Debug.LogErrorFormat("We collided with something that wasn't a ship : {0}", other.gameObject.name);
			}
			else{ // real hit! do stuff!
				ship.DoDamage(damage);
				OnHit();
			}
		}
	}
}
