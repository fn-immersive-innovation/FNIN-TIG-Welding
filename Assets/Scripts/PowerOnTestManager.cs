using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class PowerOnTestManager : MonoBehaviour
{
    public PowerManager powerManager;
    private XRKnob powerKnob;
    private ObjectContactCheckCondition condition;

    // Start is called before the first frame update
    void Start()
    {
        powerKnob = GetComponent<XRKnob>();
        condition = GetComponent<ObjectContactCheckCondition>();
    }

    // Update is called once per frame
    void Update()
    {
        if(powerKnob.value == 1)
        {
            powerManager.IsPowerOn = true;
            condition.CheckCondition();
        }
        else
        {
            powerManager.IsPowerOn = false;
        }
    }
}
