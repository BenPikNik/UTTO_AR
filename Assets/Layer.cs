using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour {

	bool xRayView = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ToggleVisibility() {

		foreach (Transform child in transform) {
			child.gameObject.SetActive ( ! child.gameObject.activeInHierarchy);
		}

	}
		
	public void SetComplexity_1() {

		SetComplexity ("complexity_1");
	}

	public void SetComplexity_2() {

		SetComplexity ("complexity_2");
	}

	public void SetComplexity_3() {

		SetComplexity ("complexity_3");
	}

	public void ToggleXRay() {
		xRayView = ! xRayView;
		SetComplexity ( xRayView ? "xray_on" : "xray_off");
	}
		

	public void SetComplexity( string childName) {

		foreach (Transform child in transform) {
			child.gameObject.SetActive ( child.name == childName );
		}

	}


}
