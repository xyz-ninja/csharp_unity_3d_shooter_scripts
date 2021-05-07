using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFirearms : MonoBehaviour
{
	//public float damage = 21;
	//public float fireRate = 0.7f;
	//public float range = 14;
	//public float shotForce = 155;

	public Weapons.Weapon weapon; // current weapon

	// след от пули
	public GameObject visualShot;
	List<GameObject> activeVisualShots;
	float visualShotDissapearTime = 0.025f;
	float visualShotDissapearTimer;

	// частицы
	public GameObject particlesMuzzleFlash;
	public GameObject particlesObjectHit;
	public GameObject particlesBlood;

	public Transform bulletSpawn;
	public AudioClip shotSFX;

	public Camera _cam;

	// таймеры и тд
	float penetrationAdditShotsCount = 0;
	// задержка перед выстрелом
	float rateDelay;
	// перезарядка
	public float reloadDelay;
	public int curAmmo = 1;
	public int curAmmoClips = 1;

	Player player;

	void Start() {
		player = GetComponent<Player>();

		rateDelay = 0;

		activeVisualShots = new List<GameObject>();
		visualShotDissapearTimer = visualShotDissapearTime;
		//setCurrentWeapon(playerScript.weapons.GetWeapon(W_TYPE.PISTOL));
	}

	void Update() {
		if (activeVisualShots.Count > 0) {
			if (visualShotDissapearTimer < 0) {
				Destroy(activeVisualShots[0]);
				activeVisualShots.RemoveAt(0);
				visualShotDissapearTimer = visualShotDissapearTime;
			} else {
				visualShotDissapearTimer -= Time.deltaTime;
			}
		}

		if (weapon != null) {
			// ЕСЛИ ИДЁТ ПЕРЕЗАРЯДКА
			if (reloadDelay > 0) {
				reloadDelay -= Time.deltaTime;
				if (reloadDelay < 0) {
					curAmmo = weapon.ammoInClip;
					player.rightHand.GetComponent<MoveToPoint>().returnBack(5);
					//player.rightHand.GetComponent<Animator>().Play("showWeapon");
				}
			} else {
				player.rightHand.transform.rotation = player.cameraT.rotation;

				if (rateDelay <= 0) {
					

					if (weapon.isAuto && Input.GetButton("Fire1") || !weapon.isAuto && Input.GetButtonDown("Fire1")) {
						player.shakeCamera(0.1f, 1);
						Shoot();
						reloadTempVariables();
					}
				} else {
					rateDelay -= Time.deltaTime;
				}
			}
		}
	}

	public void setCurrentWeapon(Weapons.Weapon w) {
		weapon = w;

		rateDelay = w.fireRate;
		curAmmo = w.ammoInClip;
		curAmmoClips = w.maxAmmoClips;

		//Debug.Log("Новое оружие - " + w.name);
	}

	void Shoot() {
		if (curAmmo <= 0) {
			reload();
			return;
		} else {
			curAmmo -= 1;
		}

		for (int shotCount = 1; shotCount <= weapon.shotsPerAttack; shotCount++) {
			RaycastHit hit;

			GameObject newP = Instantiate(particlesMuzzleFlash, bulletSpawn.position, bulletSpawn.rotation);
			Destroy(newP, 1.0f);

			// СЛЕД ОТ ВЫСТРЕЛА
			GameObject visualShotObj = Instantiate(visualShot);
			LineRenderer lineRen = visualShotObj.GetComponent<LineRenderer>();

			Vector3 lineOutPos;
			Vector3 lineToPos;

			// разброс для картечи
			Vector3 rayTargetPos = player.aimTrans.forward;
			if (shotCount > 1) {
				rayTargetPos.x += Random.Range(-weapon.multipleShotsAccuracy, weapon.multipleShotsAccuracy);
				rayTargetPos.y += Random.Range(-weapon.multipleShotsAccuracy, weapon.multipleShotsAccuracy);
			}

			//if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, weapon.fireRange)) {
			if (Physics.Raycast(_cam.transform.position, rayTargetPos, out hit, weapon.fireRange)) {
				lineOutPos = bulletSpawn.position;
				lineToPos = hit.point;

				// эффект искр
				if (hit.transform.gameObject.tag == "Metal") {
					GameObject newObj = Instantiate(particlesObjectHit, hit.point, Quaternion.LookRotation(hit.normal));
					Destroy(newObj, 1.0f);
				}

				// UNIT HIT
				Unit hitUnit = hit.transform.gameObject.GetComponent<Unit>();
				if (hitUnit != null) {
					float curDamage = weapon.damage;

					Destroy(Instantiate(particlesBlood, hit.point, hit.collider.gameObject.transform.rotation), 3.0f);

					// if has ragdoll controller
					RagdollController ragdollController = hit.transform.gameObject.GetComponent<RagdollController>();
					CapsuleCollider colCaps = hit.transform.gameObject.GetComponent<CapsuleCollider>();

					bool isUnitLimbHitted = false; // попал ли выстрел в какую-нибудь конечность

					if (ragdollController != null) {
						if (colCaps != null) colCaps.enabled = false; // отключаем capsule col что бы проверить часть тела

						// создаём доп луч для коллизии рэгдолла
						RaycastHit additHit;
						// ЕСЛИ ДОП ЛУЧ ПОПАЛ В КОНЕЧНОСТЬ
						if (Physics.Raycast(hit.point, _cam.transform.forward, out additHit, 10)) {
							if (additHit.collider != null && additHit.transform.gameObject.GetComponent<CharacterJoint>() != null) {
								// HEADSHOT
								if (additHit.rigidbody.gameObject.tag == "UnitHead") {
									curDamage *= 1.5f;
									Debug.Log("Headshot!");
								}

								//Destroy(Instantiate(particlesBlood, additHit.point, additHit.collider.gameObject.transform.rotation), 3.0f);

								isUnitLimbHitted = true;
							}
						// ЕСЛИ ДОП ЛУЧ НИКУДА НЕ ПОПАЛ
						} else {
							
						}

						bool isUnitDead = hitUnit.dealDamage(curDamage);
						if (isUnitDead) {
							// включаем регдолл
							ragdollController.MakePhysical();

							// добавляем импульс в весь регдолл
							foreach (Rigidbody rb in ragdollController.allRigidBodies) {
								rb.AddForce(-hit.normal * weapon.shotForce * 10);
							}

							// добавляем импульс в поврежденную коненчость 
							if (isUnitLimbHitted) {
								additHit.rigidbody.AddForce(-hit.normal * weapon.shotForce * 30);
							}

						} else {
							colCaps.enabled = true; // включаем коллайдер капсулы обратно
						}
					}
				// OBJECTS HIT
				} else {
					if (hit.rigidbody != null) {
						hit.rigidbody.AddForce(-hit.normal * weapon.shotForce);
						
					}
				}
				//Debug.Log(hit.collider.gameObject.name);

			// DONT HIT ANYTHING
			} else {
				lineOutPos = bulletSpawn.position;
				lineToPos = rayTargetPos * 20 + _cam.transform.position;
			}

			lineRen.SetPosition(0, lineOutPos);
			lineRen.SetPosition(1, lineToPos);

			activeVisualShots.Add(visualShotObj);

			// ОТДАЧА
			player.rotateCameraXY(weapon.recoilX, weapon.recoilY);
		}
	}
	public void reload() {
		if (curAmmoClips > 0) curAmmoClips -= 1;

		reloadDelay = weapon.reloadTime;
		Vector3 moveHandPos = player.rightHand.transform.localPosition;
		moveHandPos.x += 0.3f;
		moveHandPos.y -= 1.2f;
		moveHandPos.z -= 0.45f;
		player.rightHand.GetComponent<MoveToPoint>().moveToPoint(moveHandPos, 5);
		//player.rightHand.GetComponent<Animator>().Play("hideWeapon");
	}

	// перезагружает временные значения, таймеры, penetration и тд.
	void reloadTempVariables() {
		rateDelay = weapon.fireRate;
	}
}
