using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using TMPro;

public class ControlsPower : MonoBehaviour
{
    [SerializeField] private GameplayManager m_AppManager;
    [SerializeField] private XRKnob m_XRKnob;

    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private float m_MinVolt = 33;
    [SerializeField] private float m_MaxVolt = 110;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_AppManager.engineOn) return;

        var val = m_XRKnob.value;

        float new_val = Mathf.Lerp(m_MinVolt, m_MaxVolt, val);

        m_Text.text = new_val.ToString("0.##");

        m_AppManager.weldVoltage = val;
    }
}
