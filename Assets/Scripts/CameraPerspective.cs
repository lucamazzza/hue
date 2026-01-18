using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPerspective : MonoBehaviour
{
    public InputActionReference changePerspectiveAction;

    public CinemachineCamera thirdPersonCamera;
    public CinemachineCamera firstPersonCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thirdPersonCamera.Priority = 2;
        firstPersonCamera.Priority = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (changePerspectiveAction.action.triggered)
        {
            if (thirdPersonCamera.Priority == 2)
            {
                thirdPersonCamera.Priority = 1;
                firstPersonCamera.Priority = 2;
                GetComponent<PlayerMovement>().stopMoving();
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in renderers)
                {
                    rend.enabled = false;
                }
            }
            else
            {
                thirdPersonCamera.Priority = 2;
                firstPersonCamera.Priority = 1;
                GetComponent<PlayerMovement>().startMoving();
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in renderers)
                {
                    rend.enabled = true;
                }
            }
        }
    }
}
