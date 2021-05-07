using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
	public float mouseSensitivityX = 1.0f;
	public float mouseSensitivityY = 1.0f;

	public float walkSpeed = 10.0f;
	public float runSpeed = 13.0f;
	bool isRunning = false;
	float runEnergyCost = 15f;

	float cameraShakeTime = 0.2f, cameraShakeTimer = 0.2f;
	float cameraShakeDuration;
	float cameraShakeAmount;
	public float walkCameraShakeSpeedMul = 2.0f;
	public float walkCameraShakeRange = 0.85f;
	float walkCameraGivenVal = 0;

	public Weapons weapons;
	public Weapons.Weapon currentWeapon;
	public WeaponFirearms wf;
	public GameObject rightHand;

	List<Weapons.Weapon> allWeapons;
	int curAllWeaponsSelectedIndex = 0;

	public Transform aimTrans; // точка в которую будут лететь выстрелы

	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;

	public GameObject cameraHandler;
	Vector3 cameraHandlerInitPos;
	Vector3 cameraHandlerInitRotate;
	public GameObject camera;
	public Transform cameraT;

	public Unit plUnit;
	public float maxEnergy = 100.0f;
	public float curEnergy = 100.0f;

	float additCameraX = 0;
	float additCameraY = 0;

	float verticalLookRotation;

	Rigidbody rigidbodyR;

	public float jumpForce = 250.0f;
	public LayerMask groundedMask;

	bool grounded;
	bool cursorVisible;

	// progressbars
	public GameObject progressHealth;
	public GameObject progressEnergy;

	CapsuleCollider capCol;

	// Use this for initialization
	void Start() {
		weapons = GetComponent<Weapons>();

		cameraT = camera.transform;
		cameraHandlerInitPos = cameraHandler.transform.localPosition;
		cameraHandlerInitRotate = cameraHandler.transform.localEulerAngles;

		rigidbodyR = GetComponent<Rigidbody>();
		LockMouse();

		allWeapons = new List<Weapons.Weapon>();

		foreach (W_TYPE wType in System.Enum.GetValues(typeof(W_TYPE))) {
			//Debug.Log(wType);
			allWeapons.Add(weapons.GetWeapon(wType));
		}

		wf = GetComponent<WeaponFirearms>();
		wf.setCurrentWeapon(weapons.GetWeapon(W_TYPE.PISTOL));

		plUnit = GetComponent<Unit>();

		progressHealth.GetComponent<Progressbar>().setupProgressbar(plUnit.maxHealth);
		progressEnergy.GetComponent<Progressbar>().setupProgressbar(maxEnergy);

		capCol = GetComponent<CapsuleCollider>();
	}

	// Update is called once per frame
	void Update() {

		// CAMERA ROTATION

		// настраивается отдача оружия по X
		if (additCameraX > 0 && Random.Range(0, 10) < 5) {
			// иногда меняет сторону отдачи оружия по x
			additCameraX *= -1;
			// а так же его силу
			additCameraX *= Random.Range(0.5f, 1.0f);
		}

		float transfromYVal = 1 * Input.GetAxis("Mouse X") * mouseSensitivityX + additCameraX * Time.deltaTime;
		transform.Rotate(new Vector3(0, transfromYVal, 0));

		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY + additCameraY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);

		float cameraXVal = -1 * verticalLookRotation - additCameraY * Time.deltaTime;
		cameraT.localEulerAngles = new Vector3(cameraXVal, cameraT.localEulerAngles.y, 0);

		additCameraX = 0; additCameraY = 0;

		Animator rHandAnim = rightHand.GetComponent<Animator>();

		// CAMERA SHAKE
		if (cameraShakeDuration > 0) {
			cameraHandler.transform.localPosition = cameraHandlerInitPos + Random.insideUnitSphere / 50 * cameraShakeAmount;
			cameraShakeDuration -= Time.deltaTime * 1.0f;
		} else {
			cameraShakeDuration = 0.0f;
			cameraHandler.transform.localPosition = cameraHandlerInitPos;
		}

		/*
		cameraHandler.transform.localPosition = new Vector3(
			cameraHandlerInitPos.x,
			cameraHandlerInitPos.y + walkCameraShakingRange * Mathf.Cos(cameraShakeGainVal),
			cameraHandlerInitPos.z
		);
		*/

		// movement
		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

		float curMovementSpeed;
		if (Input.GetKey(KeyCode.LeftShift) && curEnergy > 0) {
			curEnergy -= runEnergyCost * Time.deltaTime;
			curMovementSpeed = runSpeed;
			isRunning = true;
		} else {
			if (curEnergy < maxEnergy) curEnergy += runEnergyCost / 2.2f * Time.deltaTime;
			curMovementSpeed = walkSpeed;
			isRunning = false;
		}

		Vector3 targetMoveAmount = moveDir * curMovementSpeed;

		moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

		if (moveAmount.x > 0 || moveAmount.z > 0) {
			walkCameraGivenVal += Time.deltaTime * walkCameraShakeSpeedMul;

			float shakeVal;
			if (isRunning) shakeVal = cameraHandlerInitRotate.x + (walkCameraShakeRange * 1.5f) * Mathf.Cos(walkCameraGivenVal);
			else shakeVal = cameraHandlerInitRotate.x + walkCameraShakeRange * Mathf.Cos(walkCameraGivenVal);

			cameraHandler.transform.localEulerAngles = new Vector3(
				shakeVal,
				cameraHandlerInitRotate.y,
				cameraHandlerInitRotate.z
			);

			/*
			cameraShakeTimer -= Time.deltaTime;
			if (cameraShakeTimer <= 0) {
				cameraShakeTimer = cameraShakeTime;
			}*/
		} else {
			cameraHandler.transform.localEulerAngles = cameraHandlerInitRotate;
		}

		// jump
		if (Input.GetButtonDown("Jump")) {
			if (grounded) {
				rigidbodyR.AddForce(transform.up * jumpForce);
			}
		}

		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask)) {
			grounded = true;
		} else {
			grounded = false;
		}

		/* Lock/unlock mouse on click */
		if (Input.GetButton("Cancel")) {
			if (!cursorVisible) {
				UnlockMouse();
			} else {
				LockMouse();
			}
		}

		if (Input.GetKeyDown(KeyCode.Return)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		// WEAPON TEST
		if (Input.GetKeyDown(KeyCode.R)) {
			wf.reload();
		}

		if (Input.GetKeyDown(KeyCode.E)) {
			curAllWeaponsSelectedIndex += 1;
			if (curAllWeaponsSelectedIndex > allWeapons.Count - 1) curAllWeaponsSelectedIndex = 0;
			wf.setCurrentWeapon(allWeapons[curAllWeaponsSelectedIndex]);

		} else if (Input.GetKeyDown(KeyCode.Q)) {
			curAllWeaponsSelectedIndex -= 1;
			if (curAllWeaponsSelectedIndex < 0) curAllWeaponsSelectedIndex = allWeapons.Count - 1;
			wf.setCurrentWeapon(allWeapons[curAllWeaponsSelectedIndex]);

		}

		if (plUnit != null) {
			progressHealth.GetComponent<Progressbar>().updateProgressbar(plUnit.health);
			progressEnergy.GetComponent<Progressbar>().updateProgressbar(curEnergy);
		}
	}

	void FixedUpdate() {
		if (rigidbodyR != null)
			rigidbodyR.MovePosition(rigidbodyR.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}

	void UnlockMouse() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		cursorVisible = true;
	}

	void LockMouse() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		cursorVisible = false;
	}

	public void rotateCameraXY(float rotX, float rotY) {
		additCameraX = rotX; additCameraY = rotY;
		//transform.Rotate(new Vector3(-rotX, 0, 0));
		//Vector3 prevAngles = cameraT.localEulerAngles;
		//cameraT.localEulerAngles = new Vector3(prevAngles.x + rotX, prevAngles.y + rotY, prevAngles.z);

		//Vector3 newRotation = new Vector3(rotX, rotY, cameraT.rotation.z);
		//Quaternion.Slerp(cameraT.rotation, newRotation, 0.05);
		//cameraT.rotation.x = 10;
		//cameraT.rotation = new Quaternion(cameraT.rotation.x + rotX, cameraT.rotation.y, cameraT.rotation.z);
		//(new Vector3(rotX, rotY, cameraT.rotation.z));
	}

	public void shakeCamera(float duration, float amount) {
		cameraShakeDuration = duration;
		cameraShakeAmount = amount;
	}

	
}