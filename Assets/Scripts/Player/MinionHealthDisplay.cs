using UnityEngine;
using UnityEngine.UI;

public class MinionHealthDisplay : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private Slider healthSlider;

    private Minion _targetMinion;

    private void Update()
    {
        if (_targetMinion != null)
        {
            UpdateHealthVisuals();
        }
    }

    public void Initialize(Minion minion, Sprite iconSprite)
    {
        _targetMinion = minion;

        if (portraitImage != null && iconSprite != null)
        {
            portraitImage.sprite = iconSprite;
        }

        UpdateHealthVisuals();
    }

    private void UpdateHealthVisuals()
    {
        if (_targetMinion == null) return;

        float pct = _targetMinion.PercentHealth;
        bool isAlive = _targetMinion.IsAlive;

        if (healthSlider != null)
            healthSlider.value = isAlive ? pct : 0f;

        if (portraitImage != null)
            portraitImage.color = isAlive ? Color.white : new Color(1.0f, 1.0f, 1.0f, 0.6f);
    }
}
