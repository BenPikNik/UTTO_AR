using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarBar : MonoBehaviour {
    public Image fill;
    private float _value;

    public float value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            if (fill != null)
            {
                fill.fillAmount = value;
            }
        }
    }

	// Use this for initialization
	void Start () {
        if (fill != null)
        {
            fill.type = Image.Type.Filled;
            fill.fillAmount = value;
        }
	}
}
