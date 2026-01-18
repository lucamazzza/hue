using UnityEngine;

public class MinionMoveState
{
    public Minion Attacker;
    public Minion Target;
    public MinionStatsSO BaseStats;
    public MoveSO ActiveMove;

    public float Attack 
    {
        get
        {
            return BaseStats.attack + ActiveMove.attack;
        }
    }
    public float Defence
    {
        get
        {
            return BaseStats.defence + ActiveMove.defence;
        }
    }
    public float Luck
    {
        get
        {
            return BaseStats.luck + ActiveMove.luck; 
        }

    }
    public float Speed 
    {
        get
        {
            return BaseStats.speed + ActiveMove.speed;
        }
    }

    public MinionMoveState(Minion attacker, Minion target, MoveSO move)
    {
        Attacker = attacker;
        Target = target;
        BaseStats = attacker.stats;
        ActiveMove = move;
    }
}
