using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PartySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public int index;
    public bool isPartySlot;

    private MinionStatsSO _data;
    private PartyHUDController _controller;

    public void Setup(MinionStatsSO data, int idx, PartyHUDController controller)
    {
        _data = data;
        index = idx;
        _controller = controller;

        if (iconImage == null) return;

        var hasData = _data != null;
        iconImage.sprite = hasData ? _data.minionIcon : null;
        iconImage.color = new Color(1, 1, 1, hasData ? 1f : 0f);
        iconImage.enabled = hasData;

        if (nameText != null)
            nameText.text = hasData ? _data.minionName : "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_controller != null)
            {
                _controller.OnSlotSelected(this);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_data != null && ColoringSystemManager.Instance != null)
            {
                ColoringSystemManager.Instance.OpenColoringMenu(_data);
            }
        }
    }

    public void SetHighlight(bool active)
    {
        if (iconImage != null && _data != null)
            iconImage.color = active ? Color.yellow : Color.white;
    }

    public MinionStatsSO GetData() => _data;
}