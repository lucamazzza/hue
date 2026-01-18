using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TutorialTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private List<TutorialStep> tutorialSteps;
    [SerializeField] private float delay = 0.5f;

    private TutorialManager tutorialManager;
    private bool tutorialStarted = false;

    private void Start()
    {
        tutorialManager = TutorialManager.Instance;
    }

    private void OnTriggerStay(Collider other)
    {
        if (tutorialStarted || !other.CompareTag("Player")) return;

        PlayerMovement mov = other.GetComponent<PlayerMovement>();

        if (mov != null && !tutorialStarted)
        {
            tutorialStarted = true;
            StartCoroutine(StartTutorialWithDelay(other.gameObject));
        }
    }

    public void Interact(GameObject interactor)
    {
        if (!tutorialStarted || tutorialManager == null) return;

        tutorialManager.Advance();
    }

    private IEnumerator StartTutorialWithDelay(GameObject player)
    {
        yield return new WaitForSeconds(delay);

        if (tutorialManager != null)
        {
            tutorialManager.LoadAndStartTutorial(player, tutorialSteps);
            tutorialManager.OnTutorialDone = () => Destroy(gameObject, 0.1f);

            if (GetComponent<MeshRenderer>()) GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            Debug.LogError("TutorialManager not found in scene!");
            tutorialStarted = false;
        }
    }
}