using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class LocatorStickControl : MonoBehaviour {

	public string[] stickNames;
	public Sprite[] stickImages;

	public Text stickName;
	public Image stickImage;

	public int stickIndex = 0;

	public string deviceType = "locator";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void NextStick() {
		stickIndex = (stickIndex + 1) % stickNames.Length;

		stickName.text = stickNames [stickIndex];
		stickImage.sprite = stickImages [stickIndex ];

		if (deviceType == "locator") {
			Sensor._instance.SetStickModel (stickIndex);

//			StateMachine.instance.locatorTrackerSettings.LoadSettingsForStick ();
		}

		if (deviceType == "transmitter") {
//			TransmitterObject._instance.SetModel (stickIndex);
		}

	}
}
