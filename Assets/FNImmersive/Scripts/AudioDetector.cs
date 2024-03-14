using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDetector : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_InformationAudioClip;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        m_AudioSource.clip = m_InformationAudioClip;
        m_AudioSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        m_AudioSource.Pause();
    }
}
