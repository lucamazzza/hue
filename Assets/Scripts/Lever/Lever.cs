using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] private Trapdoor trapdoor;
    [SerializeField] private Animator leverAnimator;

    private bool isActivated = false;

    public void Interact(GameObject player)
    {
        Debug.Log("Player interacted with the lever.");

        if (!isActivated)
        {
            ActivateLever();
        }
    }

    private void ActivateLever()
    {
        isActivated = true;
        // transform.Rotate(rotationAngle, 0, 0);
        leverAnimator.SetTrigger("PullLever");
        trapdoor.Open();
    }
}