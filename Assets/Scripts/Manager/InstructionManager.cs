using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class InstructionManager : MonoBehaviour
{
    public static InstructionManager Instance { get; private set; }

    public List<InstructionData> instructionDatas = new List<InstructionData>();
    public EnumClass.InstructionsID currentInstructionID = EnumClass.InstructionsID.Step1;
    public EnumClass.InstructionsID endInstructionID;
    private AudioSource audioSource;
    public AudioClip taskCompletedClip, congratulationAudio;
    public Button nextButton;

    [HideInInspector]
    public List<GameObject> allSelectedObject = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SetUpBaseConditionStructure();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (instructionDatas.Count == 0) return;
        EventManager.Instance.StartExperiment += InitializeCoroutine;
        EventManager.Instance.OnChangeInstruction += ActivateInstruction;
        nextButton.onClick.AddListener(() => ActivateInstruction());
    }

    private void OnDisable()
    {
        EventManager.Instance.OnChangeInstruction -= ActivateInstruction;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnChangeInstruction -= ActivateInstruction;
    }

    public void StartExperiment()
    {
        EventManager.Instance.StartExperiment?.Invoke();
    }

    private void SetUpBaseConditionStructure()
    {
        for (int i = 0; i < instructionDatas.Count; i++)
        {
            instructionDatas[i].instructionsID = (EnumClass.InstructionsID)i;
        }
    }

    public void AddConditions(Conditions condition)
    {
        for (int i = 0; i < instructionDatas.Count; i++)
        {
            if (condition.instructionID == instructionDatas[i].instructionsID)
            {
                if (!instructionDatas[i].conditions.Contains(condition))
                {
                    instructionDatas[i].conditions.Add(condition);

                    if (instructionDatas[i].instructionsID == currentInstructionID)
                    {
                        condition.isActive = true;
                    }
                }
            }
        }
    }

    void InitializeCoroutine()
    {
        StartCoroutine(DelayInstruction());
    }

    IEnumerator DelayInstruction()
    {
        yield return new WaitForSeconds(0.1f);
        InitializeInstruction();
    }

    public void InitializeInstruction()
    {
        foreach (var item in instructionDatas[(int)currentInstructionID].conditions)
        {
            item.isActive = true;
        }

        EventManager.Instance.InstructionChange(instructionDatas[(int)currentInstructionID].procedureData);
        PlayInstructionAudio();
    }

    private void PlaySubInstructionsAudio()
    {
        if (audioSource != null && taskCompletedClip != null)
        {
            audioSource.clip = taskCompletedClip;
            audioSource.Play();
        }
    }
    
    public void ActivateInstruction()
    {
        PlaySubInstructionsAudio();
        if (!instructionDatas[(int)currentInstructionID].AllConditionsFinished())
            return;

        currentInstructionID++;
        Debug.Log("new instruction"); 
        
        if (currentInstructionID == endInstructionID)
        {
            EventManager.Instance.InstructionEnd();
            Debug.Log("finished");

            return;
        }

        foreach (var item in instructionDatas[(int)currentInstructionID].conditions)
        {
            item.isActive = true;
        }
        EventManager.Instance.InstructionChange(instructionDatas[(int)currentInstructionID].procedureData);
        PlayInstructionAudio();
    }

    private void PlayInstructionAudio()
    {
        if (instructionDatas[(int)currentInstructionID].instructionAudio != null)
        {
            audioSource.clip = instructionDatas[(int)currentInstructionID].instructionAudio;
            audioSource.Play();
        }
        else
            return;
    }

    private void Update()
    {
        if (currentInstructionID != endInstructionID)
        {
            if (instructionDatas[(int)currentInstructionID].canByPass)
            {
                ActivateNextButton(true);
            }
            else
            {
                ActivateNextButton(false);
            }
        }
        else
        {
            PlayCongratulationAudio();
        }
    }

    private void ActivateNextButton(bool value)
    {
        nextButton.gameObject.SetActive(value);
    }

    private void PlayCongratulationAudio()
    {
        if (audioSource != null && congratulationAudio != null)
        {
            audioSource.clip = congratulationAudio;
            audioSource.Play();
        }
    }

}

[System.Serializable]
public class InstructionData
{
    public EnumClass.InstructionsID instructionsID;
    public List<Conditions> conditions = new List<Conditions>();
    public AudioClip instructionAudio;
    [TextArea]
    public string procedureData;
    public bool canByPass;

    public bool AllConditionsFinished()
    {
        int count = 0;

        foreach (var item in conditions)
        {
            if (item.isFinished)
                count++;
        }

        if (count == conditions.Count)
            return true;
        else
            return false;
    }
}