using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
	public Animator animator;
	public Rigidbody[] allRigidBodies;

	public bool isRagdollActive = false;

	bool isNeedAutodisable = false;
	float autodisableTime = 5.0f;

	private void Awake() {
		foreach (Rigidbody rb in allRigidBodies) rb.isKinematic = true;
	}

	void Start() {
		
	}

	void Update() {
		/*if (Input.GetKeyDown(KeyCode.Space)) {
			MakePhysical();
		}*/
		if (isNeedAutodisable) {
			autodisableTime -= Time.deltaTime;
			if (autodisableTime < 0) {
				foreach (Rigidbody rb in allRigidBodies) {
					CapsuleCollider capCol = rb.gameObject.GetComponent<CapsuleCollider>();
					if (capCol != null) capCol.enabled = false;
					BoxCollider boxCol = rb.gameObject.GetComponent<BoxCollider>();
					if (boxCol != null) boxCol.enabled = false;
					SphereCollider spCol = rb.gameObject.GetComponent<SphereCollider>();
					if (spCol != null) spCol.enabled = false;

					rb.constraints = RigidbodyConstraints.FreezeAll;
				}
			}
		}
	}

	public void MakePhysical() {
		CapsuleCollider capCol = GetComponent<CapsuleCollider>();
		capCol.enabled = false;
		animator.enabled = false;
		foreach (Rigidbody rb in allRigidBodies) rb.isKinematic = false;
		isNeedAutodisable = true;
	}
}
