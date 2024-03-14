using UnityEngine;
using UnityEngine.Events;

public class PowerManager : MonoBehaviour
{
    [SerializeField] private int m_NumberofItems = 1;
    [SerializeField] private int m_CurrentItem = 0;
    public float weldVoltage = 0;
    [SerializeField] private bool _isPowerOn = false;
    public WeldingMachine weldingMachine;

    public UnityEvent PowerOn;
    public UnityEvent PowerOff;
    public UnityEvent onChecklistComplete;

    public bool canWeld
    {
        get { return _isPowerOn; }
    }

    // Public property to get or set the power state
    public bool IsPowerOn
    {
        get { return _isPowerOn; }
        set
        {
            if (_isPowerOn != value)
            {
                _isPowerOn = value;
                if (_isPowerOn)
                {
                    PowerOn.Invoke();
                }
                else
                {
                    PowerOff.Invoke();
                }
            }
        }
    }

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
}
