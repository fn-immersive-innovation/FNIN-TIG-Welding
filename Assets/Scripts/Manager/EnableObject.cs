using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnableObject : MonoBehaviour
{
    public EnumClass.InstructionsID InstructionID;
    public InstructionManager instructionManager;
    public GameObject objectToEnable;
    public bool activateUI;

    // Update is called once per frame
    void Update()
    {
        if (activateUI)
        {
            if (instructionManager.currentInstructionID == InstructionID)
            {
                objectToEnable.SetActive(true);
            }
        }
    }
}
