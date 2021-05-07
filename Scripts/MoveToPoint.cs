using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPoint : MonoBehaviour
{
	public Vector3 initPosition;
	public Vector3 movePoint;

	public float speed = 5f;

	public bool isFinished = true;
	public bool isNeedUseLocalCoords = true;

	void Start() {
		initPosition = transform.localPosition;

		if (movePoint != new Vector3(0,0,0)) {
			moveToPoint(movePoint, speed, isNeedUseLocalCoords);
		}
	}

	void Update() {
		if (!isFinished) {
			float step = speed * Time.deltaTime; // calculate distance to move

			Vector3 checkPos;
			if (isNeedUseLocalCoords) {
				checkPos = transform.localPosition;
				transform.localPosition = Vector3.MoveTowards(checkPos, movePoint, step);
			} else {
				checkPos = transform.position;
				transform.position = Vector3.MoveTowards(checkPos, movePoint, step);
			}
			
			

			/*
			float dx = 0, dy = 0, dz = 0;
			if (movePoint.x < checkPos.x - 0.1) dx =-speed;
			else if (movePoint.x > checkPos.x + 0.1) dx = speed;

			if (movePoint.y < checkPos.y - 0.1) dy = speed;
			else if (movePoint.y > checkPos.y + 0.1) dy = -speed;

			if (movePoint.z < checkPos.z - 0.1) dz = speed;
			else if (movePoint.z > checkPos.z + 0.1) dz = +speed;

			transform.Translate(new Vector3(dx, dy, dz));
			*/

			if (Vector3.Distance(movePoint, transform.position) < 0.11) {
				transform.position = movePoint;
				isFinished = true;
			}
		}
	}

	public void moveToPoint(Vector3 _movePoint, float _speed, bool _useLocalCoords = true) {
		movePoint = _movePoint;
		speed = _speed;
		isNeedUseLocalCoords = _useLocalCoords;

		isFinished = false;
	}

	public void returnBack(float _speed) {
		moveToPoint(initPosition, _speed);
	}
}
