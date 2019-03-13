using UnityEngine;
//using System.IO.Ports;
//using System.Threading;
using System.Collections.Generic;
using System;
using System.Diagnostics;

using System.IO;

public class Sensor : MonoBehaviour
{
	public static Sensor _instance;

	string fileName_serialLog = "bluetoothLog.txt";


    private const float healthCheckPeriod = 2.0f;

    // works OK for spare PDL
    float topGain = 0.96f;

//    float iScale = 4096;
    //  Green stick electronics need less signal - Nick should really 
    //  fix this by changing the divider chain. This change reduces the 
    //  current by x10, a more reasonable range
 //   float iScaleSubsite = 4096f;
    //  PDL unless serial number 5 (see below)
    bool isSubsite = true;

    public List<Cable> cables;
    public GameObject bottom;
    public GameObject top;
    public GameObject nuller;
    public GameObject direction;
    public GameObject disconnected;

	public GameObject[] stickModels;

	public Transform screenRoot;

    private FakePdl4Screen screen;
//    private Thread serialReader;
    private volatile Boolean keepRunning = true;
    private volatile Boolean serialIsConnected = false;
    private Stopwatch stopWatch;
    private Stopwatch btHealthWatch;
    private float healthCheckDelay = 0.0f;
    

    private Queue<char> readBytes = new Queue<char>();
    private Queue<string> writeStrings = new Queue<string>();
    private float timeDepthWasPressed = 0;
    private float timePeakNullWasPressed = 0;
	private float timeFrequencyWasPressed = 0;

	public TextMesh debugText;


	bool initialisedFlag = false;

	StreamWriter writer = null;

	void Awake() {
		_instance = this;
	}

	// Use this for initialization
	void Initialise () {

//		topGain = ConfigLogic.GetFloat("topGain" );


//        screen = GetComponentInChildren<FakePdl4Screen>();
		/*
        if (serialReader == null)
        {
            serialReader = new Thread(new ThreadStart(SerialThreadLoop));
            serialReader.Start();
        }
        stopWatch = new Stopwatch();
        stopWatch.Start();
        btHealthWatch = new Stopwatch();
*/
		print ("Sensor : Start()");

		initialisedFlag = true;

	}

    void OnApplicationQuit()
    {
        keepRunning = false;
//        if (serialReader != null)
  //      {
    //        serialReader.Join(5000);
   //     }
    }

	public void ClearAllCalbes() {
		cables.Clear ();
	}


    // Update is called once per frame
    void Update () {

		if (!initialisedFlag) {

//			if (MultiSceneLogic.connectionsValid) {
				Initialise ();
//			} 

			return;
		}

        float bottomFieldStrength = fieldStrength(fieldAt(bottom.transform.position), bottom.transform);
        float topFieldStrength = fieldStrength(fieldAt(top.transform.position), top.transform);
        float nullerFieldStrength = fieldStrength(fieldAt(nuller.transform.position), nuller.transform);
        float directionFieldStrength = fieldStrength(fieldAt(direction.transform.position), direction.transform);

        float compassBearing = -Mathf.Atan2(directionFieldStrength, bottomFieldStrength) * Mathf.Rad2Deg;
        float nullForFakeScreen = topFieldStrength > 0 ? nullerFieldStrength : -nullerFieldStrength;
/*
		if (MainLogic._instance.transmitterFrequencyIndex != MainLogic._instance.locatorFrequencyIndex ) {
			compassBearing = 0.0f;
			nullForFakeScreen = 0.0f;
			nullerFieldStrength *= 0.001f;
			topFieldStrength *= 0.001f;
			bottomFieldStrength *= 0.001f;
			directionFieldStrength *= 0.001f;
		}
*/
		if (screen == null) {
			screen = GetComponentInChildren<FakePdl4Screen> ();
		}

		if (screen != null) {
			screen.updateScreen ((int)(200.0f * (bottomFieldStrength - topFieldStrength)), (int)(200.0f * nullForFakeScreen), (int)compassBearing);
		}

		string debugString = "upd, ";

		if (disconnected != null ) {
			disconnected.SetActive (!serialIsConnected);
		}
        if (serialIsConnected && stopWatch.ElapsedMilliseconds >= 30)
        {
			debugString += "con, ";

            if (healthCheckDelay < healthCheckPeriod)
            {
                healthCheckDelay += Time.deltaTime;
				debugString += "hde, ";
            }
            else if (!btHealthWatch.IsRunning)
            {
				debugString += "spng, ";
                sendPing();
            }
            lock (readBytes)
            {
                if (readBytes.Count != 0)
                {
                    char read = readBytes.Dequeue();
                    switch (read)
                    {
                        case 'd':
						debugString += "'d', ";
                            depthPressed();
                            break;
                        case 'p':
						debugString += "'p', ";
                            peakNullPressed();
                            break;
					case 'f':
						debugString += "'f', ";
						frequencyPressed();
						break;
                    }
                }
            }

            if (isSubsite)
            {
                //  Found to need more signal on site in St Ives - this increase
                //  should make it respond better
//				directionFieldStrength *= ( ConfigLogic.GetFloat("iScaleSubsite" ) * ConfigLogic.GetFloat("subsite_directionFieldStrength_scaler") + ConfigLogic.GetFloat("subsite_directionFieldStrength_offset") );
//				nullerFieldStrength *= ( ConfigLogic.GetFloat("iScaleSubsite" ) * ConfigLogic.GetFloat("subsite_nullerFieldStrength_scaler") + ConfigLogic.GetFloat("subsite_nullerFieldStrength_offset") );
//				bottomFieldStrength *= ( ConfigLogic.GetFloat("iScaleSubsite" ) * ConfigLogic.GetFloat("subsite_bottomFieldStrength_scaler") + ConfigLogic.GetFloat("subsite_bottomFieldStrength_offset") );
//				topFieldStrength *= ( ConfigLogic.GetFloat("iScaleSubsite" ) * ConfigLogic.GetFloat("subsite_topFieldStrength_scaler") + ConfigLogic.GetFloat("subsite_topFieldStrength_offset") );
            }
            else
            {
                //  Experimentally found figures for PDL
//				directionFieldStrength *= ( ConfigLogic.GetFloat("iScale" ) * ConfigLogic.GetFloat("directionFieldStrength_scaler") + ConfigLogic.GetFloat("directionFieldStrength_offset") );
//				nullerFieldStrength *= ( ConfigLogic.GetFloat("iScale" ) * ConfigLogic.GetFloat("nullerFieldStrength_scaler") + ConfigLogic.GetFloat("nullerFieldStrength_offset") );
//				bottomFieldStrength *= ( ConfigLogic.GetFloat("iScale" ) * ConfigLogic.GetFloat("bottomFieldStrength_scaler") + ConfigLogic.GetFloat("bottomFieldStrength_offset") );
//				topFieldStrength *= ( ConfigLogic.GetFloat("iScale" ) * ConfigLogic.GetFloat("topFieldStrength_scaler") + ConfigLogic.GetFloat("topFieldStrength_offset") );
            }

			// apply transmitter power scale factor
//			float transmitterPowerScaleFactor = ConfigLogic.GetFloat( "transmitter_power_scale_factor_" + MainLogic._instance.transmitterPowerIndex );
			float transmitterPowerScaleFactor = 1.0f;
			nullerFieldStrength *= transmitterPowerScaleFactor;
			topFieldStrength *= transmitterPowerScaleFactor;
			bottomFieldStrength *= transmitterPowerScaleFactor;
			directionFieldStrength *= transmitterPowerScaleFactor;

//            sendSigned(nullerFieldStrength, 'c');
//            sendSigned(topFieldStrength, 'a');
//            sendSigned(bottomFieldStrength, 'b');
//            sendSigned(directionFieldStrength, 'd');
            

//			print ("Sensor : " + nullerFieldStrength + ", " + topFieldStrength + ", " + bottomFieldStrength + ", " + directionFieldStrength);


            stopWatch.Stop();
            stopWatch.Reset();
            stopWatch.Start();
        }

//		debugText.text = debugString;
	}

    public void depthPressed()
    {
        float time = Time.time;
        if (time - timeDepthWasPressed < FakePdl4Screen.debounceTime)
        {
            return;
        }
        if (time - timeDepthWasPressed < FakePdl4Screen.depthShowTime)
        {
//            StateMachine.instance.placeMarker();
        }
        timeDepthWasPressed = time;

        float bottomFieldStrength = Mathf.Abs(fieldStrength(fieldAt(bottom.transform.position), bottom.transform));
        float topFieldStrength = Mathf.Abs(fieldStrength(fieldAt(top.transform.position), top.transform));
        float bottomToTop = Vector3.Distance(bottom.transform.position, top.transform.position);
        float computedDepth = bottomToTop * topFieldStrength / (bottomFieldStrength - topFieldStrength);
        /* subtract three inches for the distance from the bottom antenna to the tip */
        screen.showDepth(computedDepth - .0762f);
    }

    public void sendPing()
    {
        writeStrings.Enqueue(".");
    }

    public void pongReceived()
    {
        var echoTime = btHealthWatch.ElapsedMilliseconds;
        btHealthWatch.Stop();
        btHealthWatch.Reset();
        healthCheckDelay = 0.0f;
        UnityEngine.Debug.Log(String.Format("Bluetooth RTT: {0}ms", echoTime));
    }

	public void sendQuestionMark()
	{
		writeStrings.Enqueue("?");
		print ("Sent QuestionMark over Bluetooth");
	}

    public void peakNullPressed()
    {

        float time = Time.time;
        if (time - timePeakNullWasPressed < FakePdl4Screen.debounceTime)
        {
            return;
        }
        timePeakNullWasPressed = time;
//        StateMachine.instance.switchedPeakNull();
        screen.switchPeakNullMode();
    }

	public void frequencyPressed()
	{
		float time = Time.time;
		if (time - timeFrequencyWasPressed < FakePdl4Screen.debounceTime)
		{
			return;
		}
		timeFrequencyWasPressed = time;
//		MainLogic._instance.LocatorFrequencyUp ();
	}

    private void sendSigned(float value, char channel)
    {
        value = Mathf.Clamp(value, -32767, 32767);
        lock (writeStrings)
        {
            writeStrings.Enqueue(Mathf.RoundToInt(value).ToString() + channel);
        }
    }

    Vector3 fieldAt(Vector3 position)
    {
        Vector3 result = Vector3.zero;
        foreach (Cable cable in cables)
        {
            Vector3 positionInCableSpace = cable.transform.InverseTransformPoint(position);
            positionInCableSpace.y = 0;
            /* Normalize the cross product by dividing by the magnitude, then do the 1/r falloff. */
            Vector3 fieldInCableSpace = cable.current * Vector3.Cross(Vector3.up, positionInCableSpace) / (positionInCableSpace.sqrMagnitude);
            result += cable.transform.TransformDirection(fieldInCableSpace);
        }
        return result;
    }

    float fieldStrength(Vector3 field, Transform coil)
    {
        Vector3 coilDirection = coil.TransformDirection(Vector3.right);
        float result = Vector3.Dot(field, coilDirection);
        return result;
    }

    private void SetStickId(string idString)
    {
        int id = -1;
        int.TryParse(idString, out id);
        switch (id)
        {
            //  Original PDL (Yellow sticks) used on CGA demo 2017
			case 0:
//				topGain = ConfigLogic.GetFloat("stick_0_topGain" );
//				isSubsite = ConfigLogic.GetBool("stick_0_isSubsite" );
				UnityEngine.Debug.Log(String.Format("Found calibration for stick ID '{0}'.", idString));
			break;
			//  Original PDL (Yellow sticks) used on CGA demo 2017
            case 1:
                topGain = 0.965f;
//				topGain = ConfigLogic.GetFloat("stick_1_topGain" );
                isSubsite = false;
//				isSubsite = ConfigLogic.GetBool("stick_1_isSubsite" );
                UnityEngine.Debug.Log(String.Format("Found calibration for stick ID '{0}'.", idString));
                break;
            //  Original PDL (Yellow sticks) used on CGA demo 2017
            case 2:
                topGain = 0.955f;
//			topGain = ConfigLogic.GetFloat("stick_2_topGain" );
                isSubsite = false;
//			isSubsite = ConfigLogic.GetBool("stick_2_isSubsite" );
                UnityEngine.Debug.Log(String.Format("Found calibration for stick ID '{0}'.", idString));
                break;
            case 5:
                //  Calibrated by hand - see P1947-WM-013 v0.6
                //  But didn't seem to work - try 0.96
                topGain = 0.935f;
//			topGain = ConfigLogic.GetFloat("stick_5_topGain" );
                isSubsite = true;
//			isSubsite = ConfigLogic.GetBool("stick_5_isSubsite" );
                UnityEngine.Debug.Log(String.Format("Found calibration for stick ID '{0}'.", idString));
                break;
            case 6:
                //  Calibrated by Nick - see change notes -003 and -004 (29-11-17)
                topGain = 0.990f;
//			topGain = ConfigLogic.GetFloat("stick_6_topGain" );
                isSubsite = true;
//			isSubsite = ConfigLogic.GetBool("stick_6_isSubsite" );
                UnityEngine.Debug.Log(String.Format("Found calibration for stick ID '{0}'.", idString));
                break;
            default:
                //  Input the values for the only calibrated subsite stick
                //  We assume an unknown stick is subsite
                UnityEngine.Debug.Log(String.Format("Unknown stick ID '{0}', using default calibration.", idString));
                topGain = 0.935f; //1.00f;
//			topGain = ConfigLogic.GetFloat("stick_default_topGain" );
                isSubsite = true;
//			isSubsite = ConfigLogic.GetBool("stick_default_isSubsite" );
                break;
        }
    }



	void CreateBTLogFile() {
		if (writer != null) {
			writer.Close ();
		}

		writer = File.CreateText (fileName_serialLog);
	}

	public void FlushBTLog() {
		CreateBTLogFile ();
		print ("Flushed Bluetooth Log");
	}

	public void SetStickModel( int stickIndex ) {

		for (int i = 0; i < stickModels.Length; i++) {
			stickModels [i].SetActive (i == stickIndex);

			if (i == stickIndex) {
				screenRoot.parent = stickModels [i].transform.Find ("screenPosition");
				screenRoot.localScale = Vector3.one;
				screenRoot.localPosition = Vector3.zero;
				screenRoot.localEulerAngles = Vector3.zero;

			}
		}


	}
}
