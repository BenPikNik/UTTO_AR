using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLogic : MonoBehaviour {

	public static MainLogic _instance;

	public GameObject locatorModel;
	public GameObject menuMain;
	public GameObject menuLocator;
	public Camera cameraAR;

	public Camera VR_cameraL;
	public Camera VR_cameraR;
	public GameObject VR_world;

	public GameObject screen_MainMenu;
	public GameObject screen_ARView;

	public GameObject ARViewMenu;


	public GameObject infrastructure;

	public GameObject earth;
	public GameObject EMFields;

	public string stateName = "";

	public static bool markupFlag = false;
	public GameObject menuMarkup;

	void Awake() {
		_instance = this;
		cameraAR.enabled = false;
	}

	// Use this for initialization
	void Start () {
//		Button_hideLocator ();		
	}
	
	// Update is called once per frame
	void Update () {

		cameraAR.enabled = true;

		if (stateName == "VR") {
			if (Input.anyKeyDown) {
				Button_hideLocator ();
			}
		}

		if (stateName == "MainMenu") {
			screen_MainMenu.SetActive( true );
			screen_ARView.SetActive( false );
		}

		if (stateName == "ARView") {
			screen_MainMenu.SetActive( false );
			screen_ARView.SetActive( true );
		}

	}

//	public void ToggleDisplay() {
//		locatorModel.SetActive ( ! locatorModel.activeInHierarchy);
//	}

	public void Button_gotoARView() {
		stateName = "ARView";
	}
	public void Button_gotoMainMenu() {
		stateName = "MainMenu";
	}

	public void Button_toggleEarth() {
		earth.SetActive( ! earth.activeInHierarchy );
	}

	public void Button_toggleEMFields() {
		EMFields.SetActive( ! EMFields.activeInHierarchy );
	}

	public void Button_recalibrate() {
//		EMFields.SetActive( ! EMFields.activeInHierarchy );
		infrastructure.transform.position = new Vector3(
			cameraAR.transform.position.x,
			infrastructure.transform.position.y,
			cameraAR.transform.position.z
		);

		infrastructure.transform.eulerAngles = new Vector3(
			0.0f,
			cameraAR.transform.eulerAngles.y + 90.0f,
			0.0f
		);
	}


	public void Button_showLocator() {
//		menuMain.SetActive ( false );
//		VR_cameraL.enabled = false;
//		VR_cameraR.enabled = false;
//		VR_world.SetActive ( false );

//		locatorModel.SetActive ( true );
//		menuLocator.SetActive ( true );
//		cameraAR.enabled = true;

		locatorModel.transform.localPosition = new Vector3 (
			locatorModel.transform.localPosition.x,
			locatorModel.transform.localPosition.y,
			0.121f
		);

//		stateName = "locator";

		locatorModel.SetActive( ! locatorModel.activeInHierarchy );
		menuLocator.SetActive( locatorModel.activeInHierarchy );
		ARViewMenu.SetActive( ! locatorModel.activeInHierarchy );

	}

	public void Button_toggleMarkup() {

		markupFlag = ! markupFlag;

		menuMarkup.SetActive( markupFlag );
		ARViewMenu.SetActive( ! markupFlag );

		if( markupFlag ) {
			UnityEngine.XR.iOS.UnityARHitTestExample.newMarkFlag = false;
		}
	}

	public void Button_hideLocator() {
		locatorModel.SetActive ( false );
		menuLocator.SetActive ( false );
		VR_cameraL.enabled = false;
		VR_cameraR.enabled = false;
		VR_world.SetActive ( false );

		menuMain.SetActive ( true );
		cameraAR.enabled = true;

		locatorModel.transform.localPosition = new Vector3 (
			locatorModel.transform.localPosition.x,
			locatorModel.transform.localPosition.y,
			0.121f
		);


		stateName = "noLocator";

	
	}

	public void Button_enableVR_noLocator() {
		locatorModel.SetActive ( false );
		menuLocator.SetActive ( false );
		menuMain.SetActive ( false );
		cameraAR.enabled = false;

		VR_cameraL.enabled = true;
		VR_cameraR.enabled = true;
		VR_world.SetActive ( true );

		stateName = "VR";

	}

	public void Button_enableVR_withLocator() {
		locatorModel.SetActive ( true );
		menuLocator.SetActive ( false );
		menuMain.SetActive ( false );
		cameraAR.enabled = false;

		VR_cameraL.enabled = true;
		VR_cameraR.enabled = true;
		VR_world.SetActive ( true );

		locatorModel.transform.localPosition = new Vector3 (
			locatorModel.transform.localPosition.x,
			locatorModel.transform.localPosition.y,
			0.4f
		);

		stateName = "VR";

	}

}
