using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour {
    public float placedWhen;
    public float twistDistance = float.PositiveInfinity;
    public float uprightness = 0.0f;
    public bool usedPeakAndNull;
    public float minNull = float.PositiveInfinity;
    public float maxNull = float.NegativeInfinity;

    public float distanceFromCable(Transform cable)
    {
        Vector3 meInCableSpace = cable.InverseTransformPoint(transform.position);
        return Mathf.Abs(meInCableSpace.x);
    }
}
