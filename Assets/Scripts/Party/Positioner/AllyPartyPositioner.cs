using UnityEngine;

public class AllyPartyPositioner : PartyPositioner
{
    private PartyManager _partyManager;

    protected override void InitializeParty()
    {
        _partyManager = PartyManager.Instance;

        if (_partyManager == null)
        {
            Debug.LogError("PartyManager instance not found!");
            return;
        }

        _partyMembers.AddRange(_partyManager.partyMinions);
        _partyMembers.RemoveAll(minion => minion == null);

        if (_partyMembers.Count == 0)
        {
            Debug.LogWarning("Ally party is empty!");
        }
    }

    protected override void ConfigureMinion(GameObject minionObj, Minion minion, int index)
    {
        minionObj.name = $"Ally_{index}";
        minion.SetOwnership(true);
        
        minion.stats = _partyMembers[index];

        if (minion.stats != null && minion.stats.savedCustomTexture != null)
        {
            ApplyCustomTexture(minionObj, minion.stats.savedCustomTexture);
        }
    }

    private void ApplyCustomTexture(GameObject minionObj, Texture2D customTexture)
    {
        Transform drawingTransform = minionObj.transform.Find("Drawing");

        if (drawingTransform != null)
        {
            Renderer targetRenderer = drawingTransform.GetComponent<Renderer>();
            if (targetRenderer != null)
            {
                targetRenderer.material.mainTexture = customTexture;
                Debug.Log($"Texture personalizzata applicata correttamente a {minionObj.name}");
            }
        }
        else
        {
            Debug.LogWarning($"Impossibile trovare il figlio 'Drawing' su {minionObj.name}. La texture non verrà applicata.");
        }
    }
}