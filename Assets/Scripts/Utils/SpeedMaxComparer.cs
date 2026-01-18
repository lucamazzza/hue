using UnityEngine;
using System.Collections.Generic;

public class SpeedMaxComparer : IComparer<MinionMoveState>
{
    public int Compare(MinionMoveState x, MinionMoveState y) 
    {
        return y.Speed.CompareTo(x.Speed);
    }
}
