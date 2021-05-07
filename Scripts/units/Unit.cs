using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
	public bool isPlayer = false;

	public float maxHealth = 3;
	public float health = 1;


	bool isDead = false;

	void Start() {
		
	}

	void Update() {
		
	}

	public bool dealDamage(float damageCount) {
		health -= damageCount;

		if (health <= 0) {
			isDead = true;
			return true;
		} else {
			return false;
		}
		
	}
}
