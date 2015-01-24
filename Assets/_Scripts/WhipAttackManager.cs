﻿using UnityEngine;
using System.Collections;

public class WhipAttackManager : MonoBehaviour {

	public int whipLevel = 1;
	public LayerMask collideLayer;
	[HideInInspector] 
	public bool attacking;
	[HideInInspector] 
	public bool testDamage; // NOTICE: update in animation of all attacks 
	private Animator animator;
	private PlayerController playerControl;

	private string tag_attack = "Attack";
	private float attackWaitInterval = 0.33f;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		playerControl = GetComponent<PlayerController> ();
		attacking = false;
		testDamage = false;
	}
	

	public IEnumerator WhipAttack() {
		// stop if walking 
		if (playerControl.grounded && animator.GetInteger("Speed") != 0) {
			animator.SetInteger("Speed", 0);
		}
		animator.SetInteger (tag_attack, whipLevel);
		yield return new WaitForSeconds(attackWaitInterval);
		animator.SetInteger (tag_attack, 0);


		
	}
	void FixedUpdate() {
		// for freeze the movement of Simon when attacking
		attacking = (animator.GetInteger (tag_attack) > 0);	

		if (testDamage) {
			Debug.Log("testing damage");
			// collide with other 
			// generate collider or do over raycast
			// there is also a position y drift of squat
			Vector3 From = WhipStart();
			Vector3 To = WhipEnd(From);
			RaycastHit2D[] hits = Physics2D.RaycastAll(From, (To-From).normalized, (To-From).magnitude, collideLayer);
			Debug.Log("number of hits: " + hits.Length);
			// Boardcast to all objects that has a WhipEventhandler
			foreach (RaycastHit2D hit in hits) {
				GameObject gb = hit.transform.gameObject;
				OnWhipEvent CC = gb.GetComponent<OnWhipEvent>();	
				if (CC){
					CC.onWhipEnter();
				}
			}
			Debug.DrawLine(From, To, Color.blue, 1.0f);

		}
	}

	Vector3 WhipStart() {
		return new Vector3(
			transform.position.x + Globals.PivotToWhipStart * (playerControl.facingRight ? 1.0f : -1.0f), 
			transform.position.y + Globals.SquatOffset * (animator.GetBool("Squat") ? 1.0f : 0.0f), 0);
	}

	Vector3 WhipEnd(Vector3 From) {
		return new Vector3(
			From.x + Globals.WhipLengthShort * (playerControl.facingRight ? 1.0f : -1.0f), 
			From.y, 0);
	}

}