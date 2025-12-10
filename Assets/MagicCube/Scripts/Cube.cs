using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public Vector3 offset;

    public void RefreshOffset()
    {
        Vector3 v3 = transform.localPosition;
        v3 = new Vector3(Mathf.RoundToInt(v3.x), Mathf.RoundToInt(v3.y), Mathf.RoundToInt(v3.z));
        transform.localPosition = v3;
        offset = v3;
    }
}
