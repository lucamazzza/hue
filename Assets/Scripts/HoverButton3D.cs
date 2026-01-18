using UnityEngine;
using UnityEngine.InputSystem;

public class HoverButton3D : MonoBehaviour
{
    private Vector3 originalScale;
    private bool isHovered = false;

    [Header("Animazione Hover")]
    public float hoverScale = 1.1f;
    public float animationSpeed = 8f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (Mouse.current == null || Camera.main == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // Raycast per hover
        if (Physics.Raycast(ray, out RaycastHit hit))
            isHovered = (hit.collider.gameObject == gameObject);
        else
            isHovered = false;

        // Animazione scala
        Vector3 targetScale = isHovered ? originalScale * hoverScale : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);

        // Click
        if (isHovered && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Hai cliccato sul pulsante: " + gameObject.name);
            // Qui puoi chiamare le azioni attacca/difendi/special/scappa
        }
    }
}
