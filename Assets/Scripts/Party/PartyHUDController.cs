using UnityEngine;

public class PartyHUDController : MonoBehaviour
{
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private PartySlotUI[] reserveSlots;
    [SerializeField] private PartySlotUI[] partySlots;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip selectSound;

    private PartySlotUI _firstSelectedSlot;

    void Start()
    {
        RefreshUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            var isOpening = !hudPanel.activeSelf;
            if (isOpening)
                SoundFXManager.instance.PlaySoundFXClip(openSound, transform, 1.0f);
            else
                SoundFXManager.instance.PlaySoundFXClip(closeSound, transform, 1.0f);
            hudPanel.SetActive(isOpening);
            if (isOpening) RefreshUI();
            else _firstSelectedSlot = null;
        }
    }

    public void RefreshUI()
    {
        var pm = PartyManager.Instance;

        for (var i = 0; i < reserveSlots.Length; i++)
        {
            reserveSlots[i].Setup(pm.unlockedMinions[i], i, this);
            reserveSlots[i].SetHighlight(false);
        }

        for (var i = 0; i < partySlots.Length; i++)
        {
            partySlots[i].Setup(pm.partyMinions[i], i, this);
            partySlots[i].SetHighlight(false);
        }
    }

    public void OnSlotSelected(PartySlotUI clickedSlot)
    {
        if (_firstSelectedSlot == null)
        {
            if (clickedSlot.GetData() == null) return;
            _firstSelectedSlot = clickedSlot;
            _firstSelectedSlot.SetHighlight(true);
        }
        else
        {
            PartyManager.Instance.SwapBetweenLists(
                _firstSelectedSlot.isPartySlot, _firstSelectedSlot.index,
                clickedSlot.isPartySlot, clickedSlot.index
            );

            _firstSelectedSlot = null;
            RefreshUI();
        }
        SoundFXManager.instance.PlaySoundFXClip(selectSound, transform, 1.0f);
    }
}
