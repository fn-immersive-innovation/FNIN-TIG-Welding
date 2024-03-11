using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class MachineAudioController : MonoBehaviour
{
    [SerializeField] private XRKnob m_Knob = null;
    [SerializeField] private AudioSource m_MachineAudio = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_MachineAudio || !m_Knob) return;

        m_MachineAudio.pitch = m_Knob.value + 1;
    }
}
