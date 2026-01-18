using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Minion))]
public class EnemyAI : MonoBehaviour
{
    public EnemyDifficultySO difficulty;

    private Minion enemy;

    void Start()
    {
        enemy = GetComponent<Minion>();
    }

    public MinionMoveState DecideAction(List<Minion> playerParty, List<Minion> enemies)
    {
        if (enemy == null) return null;

        // Check if enemy life is less than defense threshold
        if (enemy.PercentHealth <= difficulty.healthThresholdDefense)
        {
            // Enemy defend itself
            return DefenceMove(null);
        }

        // Enemy randomly decide action chance
        if (Random.value < difficulty.randomActionChance)
        {
            return DecideActionRandomly(playerParty, enemies);
        }

        int totalWeigth = difficulty.attackWeight + difficulty.defenceWeight + difficulty.specialWeight;
        int randomValue = Random.Range(0, totalWeigth);

        if (randomValue < difficulty.attackWeight)
        {
            // Use Attack move
            return AttackMove(playerParty);
        }
        else if (randomValue < difficulty.attackWeight + difficulty.defenceWeight)
        {
            // Use Defence move
            return DefenceMove(enemies);
        }
        else
        {
            // Use Special move
            return SpecialMove(playerParty);
        }
    }

    private MinionMoveState DecideActionRandomly(List<Minion> playerParty, List<Minion> enemies)
    {
        int randomValue = Random.Range(0, 100);

        if (randomValue <= 40)
        {
            // Use Attack move
            return AttackMove(playerParty);
        }
        else if (randomValue <= 80)
        {
            // USe Defence move
            return DefenceMove(enemies);
        }
        else
        {
            // Use Special move
            return SpecialMove(playerParty);
        }
    }

    private MinionMoveState AttackMove(List<Minion> targets)
    {
        Minion target = TargetToHit(targets, TargetingStrategy.WeakestHealth);
        return new MinionMoveState(enemy, target, enemy.availableMoves[0]);
    }

    private MinionMoveState DefenceMove(List<Minion> targets)
    {
        Minion target = enemy;

        if (targets != null) // If is null, cure itself
        {
            target = TargetForSupport(targets);
        }

        return new MinionMoveState(enemy, target, enemy.availableMoves[1]);
    }

    private MinionMoveState SpecialMove(List<Minion> targets)
    {
        Minion target;

        if (difficulty.specialWeight > 50)
        {
            target = TargetToHit(targets, TargetingStrategy.StrongestAttacker);
        }
        else
        {
            target = TargetToHit(targets, TargetingStrategy.WeakestDefense);
        }

        return new MinionMoveState(enemy, target, enemy.availableMoves[2]);
    }

    private Minion TargetToHit(List<Minion> targets, TargetingStrategy strategy)
    {
        if (Random.value >= difficulty.exploitWeaknessChance)
        {
            // Random decide target
            return targets[Random.Range(0, targets.Count)];
        }

        // AI decide target
        Minion bestTarget = null;

        foreach (Minion target in targets)
        {
            if (!target.IsAlive) { continue; }

            if (bestTarget  == null)
            {
                bestTarget = target;
                continue;
            }

            bool isBetterTarget = false;

            switch (strategy)
            {
                case TargetingStrategy.WeakestHealth:
                    // Attack the closest to death
                    isBetterTarget = target.CurrentHealth < bestTarget.CurrentHealth;
                    break;

                case TargetingStrategy.WeakestDefense:
                    // Attack the weakest target
                    isBetterTarget = target.stats.defence < bestTarget.stats.defence;
                    break;

                case TargetingStrategy.StrongestAttacker:
                    // Prioritize threat elimination
                    isBetterTarget = target.stats.attack > bestTarget.stats.attack;
                    break;
            }

            if (isBetterTarget)
            {
                bestTarget = target;
            }
        }

        return bestTarget;
    }

    private Minion TargetForSupport(List<Minion> allies)
    {
        Minion emergencyTarget = null;
        float lowestHealthPercent = 1.0f;

        // Check if one ally is in danger and support him
        foreach (Minion ally in allies)
        {
            if (ally.IsAlive && 
                ally.PercentHealth < difficulty.healthThresholdDefense && 
                ally.PercentHealth < lowestHealthPercent)
            {
                lowestHealthPercent = ally.PercentHealth;
                emergencyTarget = ally;
            }
        }

        if (emergencyTarget != null)
        {
            return emergencyTarget;
        }

        if (Random.value < difficulty.exploitWeaknessChance)
        {
            // Enemy decide who support based on attack
            Minion bestBuffTarget = enemy;

            foreach (Minion ally in allies)
            {
                if (ally.IsAlive && ally.stats.attack > bestBuffTarget.stats.attack)
                {
                    bestBuffTarget = ally;
                }
            }

            return bestBuffTarget;
        }
        
        // Enemy support itself
        return enemy;
    }
}
