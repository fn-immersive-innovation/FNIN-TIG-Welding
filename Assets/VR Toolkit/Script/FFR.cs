using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.Oculus;

public class FFR : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Utils.SetFoveationLevel(2);
        Utils.EnableDynamicFFR(true);
    }

    // Update is called once per frame
}
