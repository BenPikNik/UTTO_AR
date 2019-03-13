using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamPlotAnimator : MonoBehaviour
{
    public Texture2D[] Frames;
    public float FPS;

    private MeshRenderer renderer;

	// Use this for initialization
	void Start ()
    {
        renderer = gameObject.GetComponentInChildren<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        int index = (int)(Time.time * FPS);
        index = index % Frames.Length;
        renderer.material.mainTexture = Frames[index];
    }
}
