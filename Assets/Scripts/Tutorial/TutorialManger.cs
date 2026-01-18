using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public Action OnTutorialDone;

    [SerializeField] private List<TutorialStep> tutorialSteps;
    private int currentStepIndex = 0;
    private bool waitingForAction = false;
    private bool tutorialActive = false;
    private GameObject playerRef;

    private DialogueController dialogueController;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dialogueController = DialogueController.Instance;
    }

    public void StartTutorial(GameObject player)
    {
        tutorialActive = true;
        playerRef = player;
        currentStepIndex = 0;
        ExecuteCurrentStep();
    }

    public void LoadAndStartTutorial(GameObject player, List<TutorialStep> steps)
    {
        tutorialSteps = steps;
        StartTutorial(player);
    }

    public bool IsTutorialActive()
    {
        return tutorialActive;
    }

    private void ExecuteCurrentStep()
    {
        if (dialogueController == null)
        {
            return;
        }

        if (currentStepIndex >= tutorialSteps.Count)
        {
            Debug.Log("Tutorial Completato!");
            tutorialActive = false;
            OnTutorialDone?.Invoke();
            return;
        }

        TutorialStep step = tutorialSteps[currentStepIndex];

        dialogueController.OnDialogueEnd += Advance;
        dialogueController.sendDialogue(playerRef, step.dialogueLines, step.speakerName);
    }

    private void OnStepDialogueFinished()
    {
        TutorialStep step = tutorialSteps[currentStepIndex];

        if (string.IsNullOrEmpty(step.actionRequired))
        {
            Advance();
        }
        else
        {
            waitingForAction = true;
            Debug.Log($"In attesa dell'azione: {step.actionRequired}");
        }
    }

    public void OnActionPerformed(string actionName)
    {
        if (waitingForAction && tutorialSteps[currentStepIndex].actionRequired == actionName)
        {
            waitingForAction = false;
            Advance();
        }
    }

    public void Advance()
    {
        if (dialogueController.isBusy())
        {
            dialogueController.continueDialogue();
            return;
        }

        currentStepIndex++;
        ExecuteCurrentStep();
    }
}