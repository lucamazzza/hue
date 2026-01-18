using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(0)]
public class TutorialCombatManager : CombatManager
{
    private TutorialManager tutorialManager;

    [Header("Input Actions")]
    public InputActionReference interactAction;

    [Header("Tutorial Combat Manager Settings")]
    [SerializeField] private List<TutorialStep> startTutorialStep;
    [SerializeField] private List<TutorialStep> winTutorialStep;
    [SerializeField] private List<TutorialStep> loseTutorialStep;

    protected override void Start()
    {
        tutorialManager = TutorialManager.Instance;

        tutorialManager.LoadAndStartTutorial(null, startTutorialStep);

        if (interactAction != null)
        {
            interactAction.action.performed += OnInteractPerformed;
        }

        base.Start();
    }

    protected override void BattleWon()
    {
        HandleOnBattleWon();
        tutorialManager.OnTutorialDone = () => base.BattleWon();
    }

    protected override void BattleLost()
    {
        HandleOnBattleLost();
    }

    protected override void GiveReward()
    {
        // No rewards in the tutorial
    }

    private void HandleOnBattleWon()
    {
        tutorialManager.LoadAndStartTutorial(null, winTutorialStep);
    }

    private void HandleOnBattleLost()
    {
        tutorialManager.LoadAndStartTutorial(null, loseTutorialStep);
        tutorialManager.OnTutorialDone = () =>
        {
            RefreshMinionList();

            foreach (var minion in _allMinions)
            {
                minion.Revive();
                Debug.Log($"Reviving minion: {minion.name}, {minion.IsAlive}");
            }
        };
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (tutorialManager.IsTutorialActive())
        {
            tutorialManager.Advance();
        }
    }
}