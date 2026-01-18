using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Dialogue/Dialogue Data")]
public class DialogueSO : ScriptableObject
{
    public string dialogueName = "Name";
    [TextArea(3, 10)]
    public string[] dialogueText = { "Test" };
}
