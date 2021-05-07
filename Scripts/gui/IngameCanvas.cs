using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameCanvas : MonoBehaviour
{
	public Player player;

	public GameObject fpsLabel;
	public GameObject weaponLabel;
	public GameObject ammoLabel;

	float deltaTime;

	

	void Start() {
	}

	void Update() {
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float fps = 1.0f / deltaTime;
		fpsLabel.GetComponent<Text>().text = "FPS " + Mathf.Ceil(fps).ToString();

		Text weaponLabelText = weaponLabel.GetComponent<Text>();
		Text ammoLabelText = ammoLabel.GetComponent<Text>();
		if (player == null || player.plUnit == null || player.wf == null || player.wf.weapon == null) {
			weaponLabelText.text = "Без оружия";
			ammoLabelText.text = "...";
		} else {
			weaponLabelText.text = player.wf.weapon.name;
			ammoLabelText.text = player.wf.curAmmo.ToString() + " | " + player.wf.curAmmoClips.ToString() + " m.";
		}
		
	}
}
