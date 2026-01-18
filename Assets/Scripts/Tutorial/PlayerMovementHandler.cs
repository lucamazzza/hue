using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementHandler : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference runAction;
    public InputActionReference jumpAction;

    private TutorialManager tutorialManager;

    private void Start()
    {
        tutorialManager = TutorialManager.Instance;
    }

    private void Update()
    {
        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();
        if (inputVector.magnitude > 0.1f)
        {
            tutorialManager.OnActionPerformed("Move");
        }

        if (runAction.action.IsPressed())
        {
            tutorialManager.OnActionPerformed("Run");
        }

        if (jumpAction.action.WasPerformedThisFrame())
        {
            tutorialManager.OnActionPerformed("Jump");
        }
    }
}