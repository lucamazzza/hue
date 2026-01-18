using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference interactAction;
    [SerializeField] private float interactionRange = 5f;

    [Header("UI Settings")]
    [Tooltip("Drag your 'Press E' UI GameObject here")]
    public GameObject interactionPromptUI;

    Collider closestInteractable;
    private IInteractable currentInteractableComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactAction.action.performed += OnInteractPerformed;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange, 1 << 6);
        if (hitColliders.Length == 0)
            closestInteractable = null;
        else
        {
            foreach (Collider collider in hitColliders)
            {
                if (closestInteractable == null)
                    closestInteractable = collider;
                else
                {
                    float distance1 = (collider.transform.position - transform.position).magnitude;
                    float distance2 = (closestInteractable.transform.position - transform.position).magnitude;

                    if (distance1 < distance2)
                        closestInteractable = collider;
                }
            }
        }

        if (closestInteractable != null && closestInteractable.GetComponent<IInteractable>() != null)
        {
            currentInteractableComponent = closestInteractable.GetComponent<IInteractable>();

            if (interactionPromptUI != null)
                interactionPromptUI.SetActive(true);
        }
        else
        {
            currentInteractableComponent = null;

            if (interactionPromptUI != null)
                interactionPromptUI.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (closestInteractable != null)
        {
            closestInteractable.GetComponent<IInteractable>().Interact(transform.gameObject);
        }
    }
}
