//using Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour {
    public float current = 1.0f;
//    public Difficulty difficulty;
	public string difficulty;
    public bool isTarget;

	public void SetMaterial( Material newMaterial ) {
		transform.Find ("Cylinder").GetComponent< Renderer >().material = newMaterial;
	}

}
