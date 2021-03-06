﻿using UnityEngine;
using System.Collections;

public class Warrior : Moving {

	//public Transform real;
	public Transform dummy;

	SpriteRenderer sr;
	public Sprite[] sp = new Sprite[3];
	public Sprite[] hitsp = new Sprite[7];
	public GameObject hitArea;
	//public GameObject walkArea;
	//public GameObject backHitArea;
	public GameObject tmpObj;

	public AudioClip hitSE;

	bool isHit = false;
	void hitProcess(float tpspd,Collider2D coll){
		if(coll.gameObject){
			coll.GetComponent<Rigidbody2D> ().AddForce (Vector2.right * 100 * tpspd, ForceMode2D.Impulse);
			coll.GetComponent<Slime> ().StartCoroutine ("dieAction");
			//base.speed = 0;
		}

	}
	void OnTriggerEnter2D(Collider2D coll){
		if (coll.tag == "Slime" && tmpObj != coll.gameObject && !isHit) {
			isHit = true;
			hitArea.GetComponent<CircleCollider2D> ().enabled = false;
			/*if (coll.transform.position.x < transform.position.x)
				base.moveBack ();*/
			StartCoroutine (hitAction(base.speed, coll));
		} else if(coll.tag == "Article" || coll.tag =="warriorblock"){
			base.moveBack ();
		}
	}
	void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.tag == "Slime" && tmpObj != coll.gameObject && !isHit) {
			isHit = true;
			//base.moveBack ();
			StartCoroutine (hitAction(base.speed, coll.collider));
		}
	}
	// Use this for initialization
	void Start () {
		sr = transform.GetComponent<SpriteRenderer> ();
		StartCoroutine ("moveAction");
		//real = transform.FindChild ("worrior");
		//dummy = transform.FindChild ("DummyWarrior");
	}
	IEnumerator hitAction(float tpspd,Collider2D coll){
		coll.GetComponent<Slime> ().isAlive = false;
		AudioSource.PlayClipAtPoint(hitSE,transform.position,2.0f);
		//Physics2D.IgnoreLayerCollision (8, 10);
		Transform tptr = transform;
		GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezePositionY;
		GetComponent<Rigidbody2D> ().freezeRotation = true;
		GetComponent<CircleCollider2D>().enabled = false;
		StopCoroutine ("moveAction");
		base.speed = 0;
		for (int i = 0; i < 7; i++) {
			sr.sprite = hitsp [i];
			if (i == 4 && coll.gameObject) {
				hitProcess (tpspd,coll);
			}
			yield return new WaitForSeconds (0.05f);

		}
		yield return new WaitForSeconds (0.6f);
		//Debug.Log (tpspd);
		base.speed = tpspd;
		StartCoroutine ("moveAction");
		tmpObj = coll.gameObject;
		isHit = false;
		GetComponent<CircleCollider2D>().enabled = true;
		hitArea.GetComponent<CircleCollider2D> ().enabled = true;
		//GetComponent<Rigidbody2D> ().gravityScale = 4;
		transform.position = tptr.position;
	}
	IEnumerator moveAction(){
		while (true) {
			for (int i = 0; i < 8; i++) {
				sr.sprite = sp [i];
				yield return new WaitForSeconds (0.1f);
			}
			//sr.sprite = sp [1];
			//yield return new WaitForSeconds (0.1f);
		}
	}
	// Update is called once per frame
	void Update () {
		MovingFunc ();
		if (GetComponent<Moving>().speed > 0) {
			hitArea.GetComponent<CircleCollider2D> ().offset = new Vector2 (0.39f, -0.45f);
			//walkArea.GetComponent<BoxCollider2D> ().offset = new Vector2 (0,0);
		} else {
			hitArea.GetComponent<CircleCollider2D> ().offset = new Vector2 (-0.38f, -0.45f);
			//walkArea.GetComponent<BoxCollider2D> ().offset = new Vector2 (-2,0);
		}
	}

}
