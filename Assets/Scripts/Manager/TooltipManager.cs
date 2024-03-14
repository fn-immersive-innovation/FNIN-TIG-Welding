using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public EnumClass.InstructionsID InstructionID;
    public InstructionManager instructionManager;
    public TextMeshProUGUI tooltipText;
    public string tooltipName;
    public GameObject tooltipObject;
    public bool changeText, activateUI;

    // Update is called once per frame
    void Update()
    {
        if (changeText)
        {
            if (instructionManager.currentInstructionID == InstructionID)
            {
                tooltipText.text = tooltipName;
            }
        }

        if (activateUI)
        {
            if (instructionManager.currentInstructionID == InstructionID)
            {
                tooltipObject.SetActive(true);
            }
            else
            {
                tooltipObject.SetActive(false);
            }
        }
    }

    public void DeactivateUI()
    {
        activateUI = false;
    }
}
