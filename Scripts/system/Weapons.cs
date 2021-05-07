using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum W_TYPE
{
	// пистолеты
	PISTOL,	REVOLVER_STANDARD, REVOLVER_MAGNUM, DESERT_EAGLE, 
	// автоматы
	USI,
	// дробовики
	WINCHECTER, SAWED_OFF_SHOTGUN,
	// винтовки
	AK47, M14_EBR
}


public class Weapons : MonoBehaviour
{
	public Weapons() {}
	public class Weapon
	{
		Weapons weapons;
		W_TYPE wType;

		public string name = "Безымянное оружие";

		public float damage;
		public float fireRate;
		public float fireRange;
		public float shotForce = 155;

		public float penetration = 0; // пробитие 0 - может убить только одного юнита
		public float shotsPerAttack = 1;
		public float multipleShotsAccuracy = 1; // разрбос при стрельбе из дробовика

		public int ammoInClip = 1;  // патронов в обойме
		public int maxAmmoClips = 1;    // максимальное количество обойм
		public float reloadTime = 1.0f;

		// отдача
		public float recoilX;
		public float recoilY;

		public bool isAuto = false;

		public Weapon(W_TYPE wType) {
			//weapons = weapons;

			if (wType == W_TYPE.PISTOL) {
				name = "Пистолет Глок 22";
				damage = 1;
				fireRate = 0.09f;
				fireRange = 15;
				shotForce = 125;
				isAuto = false;
				recoilX = 0.5f;
				recoilY = 1.9f;

				ammoInClip = 20;
				maxAmmoClips = 5;
				reloadTime = 2.1f;

			} else if (wType == W_TYPE.REVOLVER_STANDARD) {
				name = "Револьвер Стандард";
				damage = 1.55f;
				fireRate = 0.16f;
				fireRange = 17;
				shotForce = 150;
				isAuto = false;
				recoilX = 3f;
				recoilY = 4f;

				ammoInClip = 8;
				maxAmmoClips = 11;
				reloadTime = 1.9f;

			} else if (wType == W_TYPE.REVOLVER_MAGNUM) {
				name = "Револьвер Магнум";
				damage = 3.2f;
				fireRate = 0.35f;
				fireRange = 14;
				shotForce = 210;
				isAuto = false;
				recoilX = 3f;
				recoilY = 6f;
				penetration = 2;

				ammoInClip = 5;
				maxAmmoClips = 14;
				reloadTime = 2.2f;

			} else if (wType == W_TYPE.DESERT_EAGLE) {
				name = "Дезерт Игл";
				damage = 2.4f;
				fireRate = 0.2f;
				fireRange = 12;
				shotForce = 200;
				isAuto = false;
				recoilX = 3f;
				recoilY = 5.5f;
				penetration = 2;

				ammoInClip = 7;
				maxAmmoClips = 12;
				reloadTime = 1.4f;

			} else if (wType == W_TYPE.USI) {
				name = "Автомат УЗИ";
				damage = 0.55f;
				fireRate = 0.05f;
				fireRange = 13;
				shotForce = 120;
				isAuto = true;
				recoilX = 5f;
				recoilY = 2f;

				ammoInClip = 35;
				maxAmmoClips = 8;
				reloadTime = 2.6f;

			} else if (wType == W_TYPE.WINCHECTER) {
				name = "Винчестер";
				damage = 0.7f;
				fireRate = 0.3f;
				fireRange = 13.5f;
				shotForce = 150;
				isAuto = false;
				shotsPerAttack = 5;
				multipleShotsAccuracy = 0.21f;
				recoilX = 4f;
				recoilY = 7f;

				ammoInClip = 5;
				maxAmmoClips = 10;
				reloadTime = 3.1f;
			} else if (wType == W_TYPE.SAWED_OFF_SHOTGUN) {
				name = "Обрез";
				damage = 0.85f;
				fireRate = 0.25f;
				fireRange = 11.5f;
				shotForce = 170;
				isAuto = true;
				shotsPerAttack = 8;
				multipleShotsAccuracy = 0.4f;
				recoilX = 6.2f;
				recoilY = 8.2f;

				ammoInClip = 2;
				maxAmmoClips = 18;
				reloadTime = 1.05f;

			} else if (wType == W_TYPE.AK47) {
				name = "АК 47";
				damage = 1.5f;
				fireRate = 0.12f;
				fireRange = 16;
				shotForce = 170;
				isAuto = true;
				recoilX = 2f;
				recoilY = 3.3f;

				ammoInClip = 30;
				maxAmmoClips = 6;
				reloadTime = 3.6f;

			} else if (wType == W_TYPE.M14_EBR) {
				name = "M14 EBR";
				damage = 1.25f;
				fireRate = 0.08f;
				fireRange = 16;
				shotForce = 160;
				isAuto = true;
				recoilX = 0.6f;
				recoilY = 3.9f;

				ammoInClip = 25;
				maxAmmoClips = 8;
				reloadTime = 1.6f;
			}
		}
	}

	public Weapon GetWeapon(W_TYPE wType) {
		//return new Weapon(this, wType);
		return new Weapon(wType);
	}
}


