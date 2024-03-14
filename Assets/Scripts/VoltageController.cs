using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;

public class VoltageController : MonoBehaviour
{
    public XRKnob myKnob;
    public PowerManager powerManager;
    public float minVoltage = 0.0f;
    public float maxVoltage = 100.0f;
    public float currentVoltage;
    public TMP_Text voltageDisplay;

    private void Start()
    {
        powerManager = GetComponent<PowerManager>();
    }

    void Update()
    {
        // Map the knob's value to the voltage range
        currentVoltage = myKnob.value * (maxVoltage - minVoltage) + minVoltage;

        // Update the voltage display
        voltageDisplay.text = currentVoltage.ToString("F0");
        var val = myKnob.value;
        powerManager.weldVoltage = val;
    }
}