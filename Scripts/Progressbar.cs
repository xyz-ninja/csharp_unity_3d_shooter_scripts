using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progressbar : MonoBehaviour
{
	public GameObject bar;
	Vector3 initBarScale;

	public float maxPoints;
	public float curPoints;

	void Start() {
		initBarScale = bar.transform.localScale;

		if (maxPoints != default(float) && curPoints != default(float)) setupProgressbar(maxPoints, curPoints);
	}

	void Update() {
		
	}

	public void setupProgressbar(float _maxPoints, float _curPoints = -1) {
		maxPoints = _maxPoints;

		if (_curPoints == -1) curPoints = maxPoints;
		else curPoints = _curPoints;

		updateProgressbar(maxPoints);
	}

	public void updateProgressbar(float points) {
		//var coef = points * 100.0 / init_points
		//var change_percents = coef / 100.0 # 100 => 1.0
		//var bar_region_rect = bar.get_region_rect()
		//bar_region_rect.size.width = init_bar_width * change_percents
		if (maxPoints == 0) {
			Debug.LogError("EBAT");
			return;
		}
		curPoints = points;

		float coef = points * 100.0f / maxPoints;
		float changePercents = coef / 100.0f; // 100 => 1.0

		if (changePercents == 0) changePercents = 0.1f;

		float newXScale = initBarScale.x * changePercents;

		if (newXScale > 0) {
			bar.transform.localScale = new Vector3(
				newXScale,
				bar.transform.localScale.y,
				bar.transform.localScale.z
			); 
		}
	}
}
