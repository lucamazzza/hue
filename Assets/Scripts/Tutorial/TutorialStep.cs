using UnityEngine;

[CreateAssetMenu(fileName = "NewTutorialStep", menuName = "Tutorial/Step")]
public class TutorialStep : ScriptableObject
{
    public string speakerName;
    [TextArea] public string[] dialogueLines;

    public bool pauseGame = true;
    public string actionRequired;
}
