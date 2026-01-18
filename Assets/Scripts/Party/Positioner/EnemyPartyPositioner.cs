using System.Collections.Generic;
using UnityEngine;

public class EnemyPartyPositioner : PartyPositioner
{
    private EnemyDifficultySO difficulty;

    protected override void InitializeParty()
    {
        List<MinionStatsSO> enemySquad = BattleLauncher.enemySquad;
        difficulty = BattleLauncher.difficulty;

        if (enemySquad == null || enemySquad.Count == 0 || difficulty == null)
        {
            Debug.LogWarning("Enemy party is not valid!");
            return;
        }

        _partyMembers.AddRange(enemySquad);
    }

    protected override void ConfigureMinion(GameObject minionObj, Minion minion, int index)
    {
        minionObj.name = $"Enemy_{index}";
        minion.SetOwnership(false);
        minion.stats = _partyMembers[index];

        minionObj.AddComponent<EnemyAI>().difficulty = difficulty;
    }
}