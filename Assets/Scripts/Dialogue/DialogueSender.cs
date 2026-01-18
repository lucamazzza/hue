using UnityEngine;

public class DialogueSender : MonoBehaviour, IInteractable
{
    [SerializeField]
    public DialogueSO dialogue;

    public void Interact(GameObject player)
    {
        DialogueController controller = DialogueController.Instance;

        if (controller != null)
        {
            if (controller.isBusy())
                controller.continueDialogue();
            else
                controller.sendDialogue(player, dialogue.dialogueText, dialogue.dialogueName);
        }
    }
}
