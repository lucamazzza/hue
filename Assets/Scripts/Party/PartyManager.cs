using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance { get; private set; }

    public List<MinionStatsSO> startingMinionAssets = new();
    public MinionStatsSO[] unlockedMinions = new MinionStatsSO[15];
    public MinionStatsSO[] partyMinions = new MinionStatsSO[4];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMinions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeMinions()
    {
        for (var i = 0; i < unlockedMinions.Length; i++)
        {
            unlockedMinions[i] = (i < startingMinionAssets.Count) ? startingMinionAssets[i] : null;
        }
    }

    public void SwapBetweenLists(bool fromParty, int fromIdx, bool toParty, int toIdx)
    {
        var source = fromParty ? partyMinions[fromIdx] : unlockedMinions[fromIdx];
        var target = toParty ? partyMinions[toIdx] : unlockedMinions[toIdx];

        if (fromParty) partyMinions[fromIdx] = target;
        else unlockedMinions[fromIdx] = target;

        if (toParty) partyMinions[toIdx] = source;
        else unlockedMinions[toIdx] = source;
    }

    public bool IsPartyEmpty()
    {
        foreach (var minion in partyMinions)
        {
            if (minion != null) return false;
        }
        return true;
    }

    public bool UnlockMinion(MinionStatsSO minion) 
    {
        if (minion == null || IsMinionUnlocked(minion)) return false;

        for (var i = 0; i < unlockedMinions.Length; i++)
        {
            if (unlockedMinions[i] == null)
            {
                unlockedMinions[i] = minion;
                Debug.Log($"Unlocked new minion: {minion.minionName} at slot {i}");
                return true;
            }
        }

        Debug.LogWarning("No room left in unlockedMinions array!");
        return false;
    }

    private bool IsMinionUnlocked(MinionStatsSO minion)
    {
        foreach (var m in unlockedMinions) if (m == minion) return true;
        foreach (var m in partyMinions) if (m == minion) return true;
        return false;
    }
}