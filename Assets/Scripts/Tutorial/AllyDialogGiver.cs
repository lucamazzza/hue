using UnityEngine;

public class AllyDialogGiver : MonoBehaviour, IInteractable
{
    [Header("Dati Dialogo e Alleato")]
    [SerializeField] private DialogueSO dialogue;
    [SerializeField] private MinionStatsSO allyData;

    public void Interact(GameObject player)
    {
        DialogueController controller = DialogueController.Instance;

        if (controller != null && !controller.isBusy())
        {
            controller.OnDialogueEnd = UnlockAlly;
            controller.sendDialogue(player, dialogue.dialogueText, dialogue.dialogueName);
        }
        else if (controller != null && controller.isBusy())
        {
            controller.continueDialogue();
        }
    }

    private void UnlockAlly()
    {
        PartyManager partyManager = PartyManager.Instance;

        if (partyManager == null) return;
        bool unlocked = partyManager.UnlockMinion(allyData);

        Debug.Log(unlocked
            ? $"Ally {allyData.minionName} unlocked!"
            : $"Ally {allyData.minionName} was already unlocked.");
    }
}