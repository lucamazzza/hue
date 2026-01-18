using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleDialogueSender : MonoBehaviour, IInteractable
{
    [Header("Dati Dialogo e Mostro")]
    [SerializeField] private DialogueSO dialogue;
    [SerializeField] private MinionStatsSO monsterData;

    [Header("Configurazione Scena")]
    [Tooltip("Inserisci il nome esatto della scena di battaglia da caricare per questo mostro")]
    [SerializeField] private string battleSceneName;
    [Tooltip("Inserisci i mostri nemici per la battaglia")]
    [SerializeField] private List<MinionStatsSO> enemySquad;
    [Tooltip("Imposta la difficoltà della battaglia")]
    [SerializeField] private EnemyDifficultySO difficulty;
    [Tooltip("Boss Battle?")]
    [SerializeField] private bool isBossBattle = false;

    public void Interact(GameObject player)
    {
        DialogueController controller = DialogueController.Instance;

        if (controller != null && !controller.isBusy())
        {
            if (PartyManager.Instance != null && !PartyManager.Instance.IsPartyEmpty())
            {
                if (BattleManager.Instance != null)
                {
                    BattleManager.Instance.PrepareBattle(monsterData); //
                }
                
                controller.OnDialogueEnd = StartTransition;
            }
            else
            {
                controller.OnDialogueEnd = null; 
                Debug.LogWarning("Combattimento bloccato: Party vuoto.");
            }

            controller.sendDialogue(player, dialogue.dialogueText, dialogue.dialogueName);
        }
        else if (controller != null && controller.isBusy())
        {
            controller.continueDialogue();
        }
    }

    private void StartTransition()
    {
        if (PartyManager.Instance != null && PartyManager.Instance.IsPartyEmpty())
        {
            Debug.Log("Il party è vuoto, la scena di battaglia non verrà caricata.");
            return; 
        }

        if (!string.IsNullOrEmpty(battleSceneName))
        {
            // SceneManager.LoadScene(battleSceneName); //
            BattleLauncher.PrepareBattle(battleSceneName, monsterData, enemySquad, difficulty, isBossBattle);
        }
        else
        {
            Debug.LogError($"Attenzione: Nome scena non impostato su {gameObject.name}!");
        }
    }
}