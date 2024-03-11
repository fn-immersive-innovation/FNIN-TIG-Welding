using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private int m_NumberofItems = 1;
    [SerializeField] private int m_CurrentItem = 0;

    [SerializeField] private int m_NumberofWeldSteps = 2;
    [SerializeField] private int m_CurrentWeldStep = 0;

    public bool shouldWeld = false;
    public bool engineOn = false;
    public float weldVoltage = 0;

    public bool canWeld
    {
       // get { return shouldWeld && engineOn; }
        get { return   engineOn; }
    }

    public UnityEvent onChecklistComplete;
    public UnityEvent onWeldRequiredStepComplete;

    public UnityEvent EngineOn;
    public UnityEvent EngineOff;

    public void PickUpItems()
    {
        m_CurrentItem++;

        if (m_CurrentItem > m_NumberofItems)
        {
            return;
        }

        if (m_CurrentItem == m_NumberofItems)
        {
            onChecklistComplete.Invoke();
        }

    }

    public void DropPickUpItems()
    {
        m_CurrentItem--;

        if (m_CurrentItem <= 0)
        {
            return;
        }
    }

    public void IncrementWeldingStep()
    {
        m_CurrentWeldStep++;

        if (m_CurrentWeldStep > m_NumberofWeldSteps)
        {
            return;
        }

        if (m_CurrentWeldStep == m_NumberofWeldSteps)
        {
            shouldWeld = true;
            onWeldRequiredStepComplete.Invoke();
        }
    }

    public void DecrementWeldingStep()
    {
        m_CurrentWeldStep--;

        if (m_CurrentWeldStep <= 0)
        {
            return;
        }

        shouldWeld = false;
    }

    public void MachineToggle()
    {
        engineOn = !engineOn;

        if (engineOn)
        {
            EngineOn.Invoke();
        }
        else
        {
            EngineOff.Invoke();
        }
    }

    public void LoadScene(string level)
    {
        SceneManager.LoadScene(level);
    }
}
