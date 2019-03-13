using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMagneticFieldLines : MonoBehaviour {

    public List<GameObject> magneticFields;

    private void OnEnable()
    {
        foreach(GameObject field in magneticFields)
        {
			if (field != null) {
				field.SetActive (false);
			}
        }
    }
}
