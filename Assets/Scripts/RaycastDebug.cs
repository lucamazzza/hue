using UnityEngine;
using UnityEngine.InputSystem; // <- nuovo Input System

public class RaycastDebug : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Debug.DrawRay(ray.origin, ray.direction * 50f, Color.red);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
                Debug.Log("Colpito: " + hit.collider.gameObject.name);
            else
                Debug.Log("Nessun oggetto colpito");
        }
    }
}
