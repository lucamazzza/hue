using UnityEngine;
using UnityEngine.UI;

class KeyHandler : MonoBehaviour
{
    [SerializeField] private KeyCode actionKey;
    [Range(0f, 1f)]
    [SerializeField] private float transparencyAmount = 0.6f;

    private Image keyImage;
    private Color originalColor;

    private void Start()
    {
        keyImage = GetComponent<Image>();

        if (keyImage != null)
        {
            originalColor = keyImage.color;
        }
    }

    private void Update()
    {
        if (keyImage == null)
            return;

        if (Input.GetKeyDown(actionKey))
        {
            SetAlpha(transparencyAmount);
            Debug.Log($"Action key {actionKey} pressed (Held).");
        }

        if (Input.GetKeyUp(actionKey))
        {
            SetAlpha(originalColor.a);
            Debug.Log($"Action key {actionKey} released.");
        }
    }

    private void SetAlpha(float alpha)
    {
        Color tempColor = keyImage.color;
        tempColor.a = alpha;
        keyImage.color = tempColor;
    }
}
