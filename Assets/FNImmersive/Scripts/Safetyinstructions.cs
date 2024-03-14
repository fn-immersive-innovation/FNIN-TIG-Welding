using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Safetyinstructions : MonoBehaviour
{
    [SerializeField] List<AudioClip> m_SafetyTutorialSequence;
    [SerializeField] AudioSource m_SafetyTutorialSource;

    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_SafetyTutorialSource.isPlaying)
        {
            if (count + 1 > m_SafetyTutorialSequence.Count) return;

            m_SafetyTutorialSource.clip = m_SafetyTutorialSequence[count];
            m_SafetyTutorialSource.Play();
           // m_SafetyTutorialSource.PlayOneShot(m_SafetyTutorialSequence[count]);

            count++;
        }
    }
}
