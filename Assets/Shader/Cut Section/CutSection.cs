using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSection : MonoBehaviour
{
    [SerializeField] private Material mat;
    private Vector3 pos;
    void Update()
    {
        //var newFlow = mat.GetFloat("_flow") + 0.001f;
        //mat.SetFloat("_flow", newFlow);
        pos = mat.GetVector("_position");
        Debug.Log(pos);
    }
    
}
